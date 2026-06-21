$ErrorActionPreference = "Stop"

$profileDirectory = $PSScriptRoot
$composeFile = Join-Path $profileDirectory "docker-compose.yml"
$projectName = "nkafka-e2e-kraft-ssl-3broker"

& docker compose --project-name $projectName -f $composeFile down -v --remove-orphans

if ($LASTEXITCODE -ne 0) {
    throw "docker compose down failed for project '$projectName'."
}
