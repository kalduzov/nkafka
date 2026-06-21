param(
    [string[]]$KafkaVersions = @("3.7.1", "3.8.0", "3.9.1"),
    [string]$TargetFramework = "net9.0",
    [switch]$KeepEnvironment
)

$sharedScript = Join-Path $PSScriptRoot "..\..\scripts\run-api-versions-matrix.ps1"

$parameters = @{
    ProfileDirectory = $PSScriptRoot
    TopologyMode = 'kraft'
    SecurityProfile = 'sasl-oauthbearer'
    KafkaVersions = $KafkaVersions
    TargetFramework = $TargetFramework
    SaslMechanism = 'OAuthBearer'
    KeepEnvironment = $KeepEnvironment
}

& $sharedScript @parameters

