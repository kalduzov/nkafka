param(
    [string[]]$KafkaVersions = @("3.7.1", "3.8.0", "3.9.1"),
    [string]$TargetFramework = "net9.0",
    [switch]$KeepEnvironment
)

$ErrorActionPreference = "Stop"

$profileDirectory = $PSScriptRoot
$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $profileDirectory "..\..\..\.."))
$composeFile = Join-Path $profileDirectory "docker-compose.yml"
$bootstrapServers = "localhost:29092"

function Get-ProjectPathForKafkaVersion {
    param([string]$KafkaVersion)

    if ($KafkaVersion.StartsWith("3.7.")) {
        return Join-Path $repositoryRoot "tests\integration\NKafka.IntegrationTests.Kafka_3_7\NKafka.IntegrationTests.Kafka_3_7.csproj"
    }

    if ($KafkaVersion.StartsWith("3.8.")) {
        return Join-Path $repositoryRoot "tests\integration\NKafka.IntegrationTests.Kafka_3_8\NKafka.IntegrationTests.Kafka_3_8.csproj"
    }

    if ($KafkaVersion.StartsWith("3.9.")) {
        return Join-Path $repositoryRoot "tests\integration\NKafka.IntegrationTests.Kafka_3_9\NKafka.IntegrationTests.Kafka_3_9.csproj"
    }

    throw "No version-specific E2E assembly is configured for Kafka version '$KafkaVersion'."
}

function Invoke-Compose {
    param(
        [string]$ProjectName,
        [string[]]$Arguments,
        [hashtable]$EnvironmentVariables
    )

    $previousValues = @{}
    foreach ($key in $EnvironmentVariables.Keys) {
        $previousValues[$key] = [Environment]::GetEnvironmentVariable($key, "Process")
        [Environment]::SetEnvironmentVariable($key, $EnvironmentVariables[$key], "Process")
    }

    try {
        & docker compose --project-name $ProjectName -f $composeFile @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "docker compose command failed for project '$ProjectName'."
        }
    }
    finally {
        foreach ($key in $EnvironmentVariables.Keys) {
            [Environment]::SetEnvironmentVariable($key, $previousValues[$key], "Process")
        }
    }
}

function Wait-KafkaReady {
    param(
        [string]$TcpHost,
        [int]$Port,
        [int]$TimeoutSeconds = 120
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        try {
            $client = [System.Net.Sockets.TcpClient]::new()
            try {
                $connectTask = $client.ConnectAsync($TcpHost, $Port)
                if (-not $connectTask.Wait([TimeSpan]::FromSeconds(2))) {
                    throw "Timed out while connecting to ${TcpHost}:${Port}."
                }
            }
            finally {
                $client.Dispose()
            }

            return
        }
        catch {
            Start-Sleep -Seconds 3
        }
    }

    throw "Kafka broker did not become ready on ${TcpHost}:${Port} within $TimeoutSeconds seconds."
}

function Invoke-ApiVersionsTestRun {
    param(
        [string]$KafkaVersion,
        [string]$ProjectPath,
        [int]$MaxAttempts = 5
    )

    $previousValues = @{
        NKAFKA_E2E_ENABLED          = [Environment]::GetEnvironmentVariable("NKAFKA_E2E_ENABLED", "Process")
        NKAFKA_E2E_TOPOLOGY_MODE    = [Environment]::GetEnvironmentVariable("NKAFKA_E2E_TOPOLOGY_MODE", "Process")
        NKAFKA_E2E_SECURITY_PROFILE = [Environment]::GetEnvironmentVariable("NKAFKA_E2E_SECURITY_PROFILE", "Process")
        NKAFKA_E2E_KAFKA_VERSION    = [Environment]::GetEnvironmentVariable("NKAFKA_E2E_KAFKA_VERSION", "Process")
        NKAFKA_E2E_BOOTSTRAP_SERVERS = [Environment]::GetEnvironmentVariable("NKAFKA_E2E_BOOTSTRAP_SERVERS", "Process")
    }

    try {
        # The E2E harness selects concrete scenarios from environment so one script can
        # drive the same broker-backed flow across multiple Kafka version lines.
        [Environment]::SetEnvironmentVariable("NKAFKA_E2E_ENABLED", "true", "Process")
        [Environment]::SetEnvironmentVariable("NKAFKA_E2E_TOPOLOGY_MODE", "kraft", "Process")
        [Environment]::SetEnvironmentVariable("NKAFKA_E2E_SECURITY_PROFILE", "plaintext", "Process")
        [Environment]::SetEnvironmentVariable("NKAFKA_E2E_KAFKA_VERSION", $KafkaVersion, "Process")
        [Environment]::SetEnvironmentVariable("NKAFKA_E2E_BOOTSTRAP_SERVERS", $bootstrapServers, "Process")

        for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++) {
            & dotnet test $ProjectPath -f $TargetFramework --filter "FullyQualifiedName~ApiVersionsE2ETests"
            if ($LASTEXITCODE -eq 0) {
                return
            }

            if ($attempt -eq $MaxAttempts) {
                throw "ApiVersions E2E tests failed for Kafka version '$KafkaVersion' after $MaxAttempts attempts."
            }

            Start-Sleep -Seconds 5
        }
    }
    finally {
        foreach ($key in $previousValues.Keys) {
            [Environment]::SetEnvironmentVariable($key, $previousValues[$key], "Process")
        }
    }
}

foreach ($kafkaVersion in $KafkaVersions) {
    $versionToken = $kafkaVersion.Replace(".", "-")
    $projectName = "nkafka-e2e-kraft-plaintext-$versionToken"
    $containerName = "nkafka-e2e-kraft-plaintext-$versionToken"
    $projectPath = Get-ProjectPathForKafkaVersion -KafkaVersion $kafkaVersion

    $composeEnvironment = @{
        KAFKA_VERSION        = $kafkaVersion
        KAFKA_CONTAINER_NAME = $containerName
        KAFKA_EXTERNAL_PORT  = "29092"
        KAFKA_ADVERTISED_HOST = "localhost"
    }

    Write-Host ""
    Write-Host "=== Kafka $kafkaVersion / ApiVersions E2E ===" -ForegroundColor Cyan

    try {
        Invoke-Compose -ProjectName $projectName -Arguments @("down", "-v", "--remove-orphans") -EnvironmentVariables $composeEnvironment
    }
    catch {
        # A missing previous stack should not stop the first run for a new version token.
    }

    try {
        Invoke-Compose -ProjectName $projectName -Arguments @("up", "-d") -EnvironmentVariables $composeEnvironment
        Wait-KafkaReady -TcpHost "localhost" -Port 29092
        Invoke-ApiVersionsTestRun -KafkaVersion $kafkaVersion -ProjectPath $projectPath
    }
    finally {
        if (-not $KeepEnvironment) {
            Invoke-Compose -ProjectName $projectName -Arguments @("down", "-v", "--remove-orphans") -EnvironmentVariables $composeEnvironment
        }
    }
}
