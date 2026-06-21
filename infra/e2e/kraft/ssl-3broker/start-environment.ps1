param(
    [string]$KafkaVersion = "3.9.1",
    [string]$TopicName = "test_topic",
    [string]$AdvertisedHost = "localhost",
    [string]$SslStorePassword = "changeit",
    [string]$ClusterId = "4dYfr59lTFyBKgAAfge3lg"
)

$ErrorActionPreference = "Stop"

$profileDirectory = $PSScriptRoot
$composeFile = Join-Path $profileDirectory "docker-compose.yml"
$scriptsDirectory = Join-Path $profileDirectory "..\..\scripts"
$sslScriptPath = Join-Path $scriptsDirectory "ensure-ssl-certs.ps1"
$truststorePath = Join-Path $profileDirectory "certs\client.truststore.p12"
$projectName = "nkafka-e2e-kraft-ssl-3broker"
$brokerPorts = @(29091, 29092, 29093)
$brokerContainerNames = @(
    "nkafka-e2e-kraft-ssl-3broker-broker-1",
    "nkafka-e2e-kraft-ssl-3broker-broker-2",
    "nkafka-e2e-kraft-ssl-3broker-broker-3"
)
$topicReplicaAssignment = "1:2:3,2:3:1,3:1:2,1:3:2,2:1:3,3:2:1,1:2:3,2:3:1,3:1:2"

function Invoke-Compose {
    param(
        [string[]]$Arguments,
        [hashtable]$EnvironmentVariables
    )

    $previousValues = @{}
    foreach ($key in $EnvironmentVariables.Keys) {
        $previousValues[$key] = [Environment]::GetEnvironmentVariable($key, "Process")
        [Environment]::SetEnvironmentVariable($key, $EnvironmentVariables[$key], "Process")
    }

    try {
        & docker compose --project-name $projectName -f $composeFile @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "docker compose command failed for project '$projectName'."
        }
    }
    finally {
        foreach ($key in $EnvironmentVariables.Keys) {
            [Environment]::SetEnvironmentVariable($key, $previousValues[$key], "Process")
        }
    }
}

function Wait-PortReady {
    param(
        [string]$TcpHost,
        [int]$Port,
        [int]$TimeoutSeconds = 120
    )

    Write-Host "Waiting for TCP endpoint ${TcpHost}:${Port}..." -ForegroundColor DarkCyan

    $attempt = 0
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $attempt++
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
            if ($attempt % 5 -eq 0) {
                Write-Host "Still waiting for TCP endpoint ${TcpHost}:${Port}..." -ForegroundColor DarkGray
            }
            Start-Sleep -Seconds 3
        }
    }

    throw "Broker did not become ready on ${TcpHost}:${Port} within $TimeoutSeconds seconds."
}

function Write-BrokerLogs {
    param([string[]]$ContainerNames)

    foreach ($containerName in $ContainerNames) {
        Write-Host ""
        Write-Host "Last broker log lines from $containerName" -ForegroundColor Yellow
        & docker logs --tail 40 $containerName
    }
}

function Invoke-DockerKafkaCommand {
    param(
        [string]$ContainerName,
        [string[]]$Arguments
    )

    function ConvertTo-ProcessArgument {
        param([string]$Value)

        if ($Value -notmatch '[\s"]') {
            return $Value
        }

        return '"' + ($Value -replace '"', '\"') + '"'
    }

    $startInfo = [System.Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = "docker"
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $processArguments = [System.Collections.Generic.List[string]]::new()
    $processArguments.Add((ConvertTo-ProcessArgument "exec"))
    $processArguments.Add((ConvertTo-ProcessArgument $ContainerName))
    foreach ($argument in $Arguments) {
        $processArguments.Add((ConvertTo-ProcessArgument $argument))
    }
    $startInfo.Arguments = $processArguments -join " "

    try {
        $process = [System.Diagnostics.Process]::new()
        $process.StartInfo = $startInfo
        $null = $process.Start()
        $standardOutput = $process.StandardOutput.ReadToEnd()
        $standardError = $process.StandardError.ReadToEnd()
        $process.WaitForExit()

        $script:LASTEXITCODE = $process.ExitCode

        if ($process.ExitCode -eq 0) {
            if ([string]::IsNullOrWhiteSpace($standardOutput)) {
                return @()
            }

            return @($standardOutput -split "`r?`n" | Where-Object { $_ -ne "" })
        }

        return @($standardError -split "`r?`n" | Where-Object { $_ -ne "" })
    }
    finally {
        if ($null -ne $process) {
            $process.Dispose()
        }
    }
}

function Wait-AdminReady {
    param(
        [string]$ContainerName,
        [int]$TimeoutSeconds = 120
    )

    Write-Host "Waiting for Kafka admin path inside $ContainerName..." -ForegroundColor DarkCyan

    $attempt = 0
    $lastError = @()
    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $attempt++
        $result = Invoke-DockerKafkaCommand -ContainerName $ContainerName -Arguments @(
            "/opt/kafka/bin/kafka-topics.sh",
            "--bootstrap-server", "kafka-1:9094",
            "--list"
        )

        if ($LASTEXITCODE -eq 0) {
            return
        }

        $lastError = $result
        if ($attempt % 5 -eq 0) {
            Write-Host "Still waiting for Kafka admin path inside $ContainerName..." -ForegroundColor DarkGray
        }

        Start-Sleep -Seconds 3
    }

    Write-BrokerLogs -ContainerNames $brokerContainerNames

    if ($lastError.Count -gt 0) {
        throw "Kafka admin path did not become ready inside container '$ContainerName' within $TimeoutSeconds seconds. Last admin error: $($lastError -join ' | ')"
    }

    throw "Kafka admin path did not become ready inside container '$ContainerName' within $TimeoutSeconds seconds."
}

function Ensure-TestTopic {
    param(
        [string]$ContainerName,
        [string]$TargetTopicName,
        [int]$MaxAttempts = 20
    )

    Write-Host "Ensuring topic '$TargetTopicName' exists with the expected partition layout..." -ForegroundColor DarkCyan

    for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++) {
        Write-Host "Topic validation attempt $attempt/$MaxAttempts..." -ForegroundColor DarkGray

        $description = Invoke-DockerKafkaCommand -ContainerName $ContainerName -Arguments @(
            "/opt/kafka/bin/kafka-topics.sh",
            "--bootstrap-server", "kafka-1:9094",
            "--describe",
            "--topic", $TargetTopicName
        )

        if ($LASTEXITCODE -ne 0) {
            Write-Host "Topic '$TargetTopicName' does not exist yet. Creating it..." -ForegroundColor DarkGray
            $null = Invoke-DockerKafkaCommand -ContainerName $ContainerName -Arguments @(
                "/opt/kafka/bin/kafka-topics.sh",
                "--bootstrap-server", "kafka-1:9094",
                "--create",
                "--topic", $TargetTopicName,
                "--replica-assignment", $topicReplicaAssignment
            )

            if ($LASTEXITCODE -ne 0) {
                if ($attempt -eq $MaxAttempts) {
                    throw "Failed to create topic '$TargetTopicName'."
                }

                Start-Sleep -Seconds 3
                continue
            }

            $description = Invoke-DockerKafkaCommand -ContainerName $ContainerName -Arguments @(
                "/opt/kafka/bin/kafka-topics.sh",
                "--bootstrap-server", "kafka-1:9094",
                "--describe",
                "--topic", $TargetTopicName
            )
        }

        $partitionLines = @($description | Where-Object { $_ -match "Partition:\\s+\\d+" })
        Write-Host "Observed partition count: $($partitionLines.Count)" -ForegroundColor DarkGray
        if ($partitionLines.Count -ne 9) {
            if ($attempt -eq $MaxAttempts) {
                throw "Topic '$TargetTopicName' does not have 9 partitions."
            }

            Start-Sleep -Seconds 3
            continue
        }

        $isBalanced = $true
        $leaderDistribution = @{}
        foreach ($brokerId in 1..3) {
            $leaderCount = @($partitionLines | Where-Object { $_ -match "Leader:\\s+$brokerId(\\s|$)" }).Count
            $leaderDistribution[$brokerId] = $leaderCount
            if ($leaderCount -ne 3) {
                if ($attempt -eq $MaxAttempts) {
                    throw "Topic '$TargetTopicName' does not have 3 leader partitions on broker '$brokerId'."
                }

                $isBalanced = $false
                break
            }
        }

        Write-Host ("Leader distribution: broker-1={0}, broker-2={1}, broker-3={2}" -f $leaderDistribution[1], $leaderDistribution[2], $leaderDistribution[3]) -ForegroundColor DarkGray

        if (-not $isBalanced) {
            Write-Host "Topic '$TargetTopicName' is not balanced yet. Waiting for leader distribution..." -ForegroundColor DarkGray
            Start-Sleep -Seconds 3
            continue
        }

        return
    }
}

Write-Host ""
Write-Host "Starting KRaft SSL 3-broker environment..." -ForegroundColor Cyan
& $sslScriptPath -ProfileDirectory $profileDirectory -StorePassword $SslStorePassword

$composeEnvironment = @{
    KAFKA_VERSION            = $KafkaVersion
    KAFKA_ADVERTISED_HOST    = $AdvertisedHost
    KAFKA_CLUSTER_ID         = $ClusterId
    SSL_STORE_PASSWORD       = $SslStorePassword
    BROKER_1_EXTERNAL_PORT   = "29091"
    BROKER_2_EXTERNAL_PORT   = "29092"
    BROKER_3_EXTERNAL_PORT   = "29093"
    BROKER_1_CONTAINER_NAME  = "nkafka-e2e-kraft-ssl-3broker-broker-1"
    BROKER_2_CONTAINER_NAME  = "nkafka-e2e-kraft-ssl-3broker-broker-2"
    BROKER_3_CONTAINER_NAME  = "nkafka-e2e-kraft-ssl-3broker-broker-3"
}

Invoke-Compose -Arguments @("up", "-d") -EnvironmentVariables $composeEnvironment

foreach ($port in $brokerPorts) {
    Wait-PortReady -TcpHost $AdvertisedHost -Port $port
}

Wait-AdminReady -ContainerName "nkafka-e2e-kraft-ssl-3broker-broker-1"
Ensure-TestTopic -ContainerName "nkafka-e2e-kraft-ssl-3broker-broker-1" -TargetTopicName $TopicName

Write-Host ""
Write-Host "KRaft SSL 3-broker environment is ready." -ForegroundColor Green
Write-Host "Topic: $TopicName"
Write-Host "Bootstrap servers: ${AdvertisedHost}:29091,${AdvertisedHost}:29092,${AdvertisedHost}:29093"
Write-Host "Client truststore: $truststorePath"
Write-Host "SSL truststore password: $SslStorePassword"
Write-Host "SSL truststore type: PKCS12"
