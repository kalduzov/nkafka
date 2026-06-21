param(
    [Parameter(Mandatory = $true)]
    [string]$ProfileDirectory,
    [string]$StorePassword = "changeit"
)

$ErrorActionPreference = "Stop"

$certificatesDirectory = Join-Path $ProfileDirectory "certs"
$keystorePath = Join-Path $certificatesDirectory "kafka.keystore.p12"
$certificatePath = Join-Path $certificatesDirectory "kafka.crt"
$truststorePath = Join-Path $certificatesDirectory "client.truststore.p12"

if ((Test-Path $keystorePath) -and (Test-Path $certificatePath) -and (Test-Path $truststorePath)) {
    return
}

New-Item -ItemType Directory -Path $certificatesDirectory -Force | Out-Null

$mountPath = (Resolve-Path $certificatesDirectory).Path

# The E2E matrix must stay self-contained, so certificate material is generated
# inside a disposable JDK container instead of depending on host-installed tools.
if (-not (Test-Path $keystorePath)) {
    & docker run --rm `
        -v "${mountPath}:/work" `
        eclipse-temurin:17-jdk `
        keytool `
        -genkeypair `
        -alias kafka `
        -keyalg RSA `
        -storetype PKCS12 `
        -keystore /work/kafka.keystore.p12 `
        -storepass $StorePassword `
        -keypass $StorePassword `
        -dname "CN=localhost" `
        -ext "SAN=dns:localhost,ip:127.0.0.1" `
        -validity 3650 `
        -noprompt

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to generate SSL keystore artifacts for profile '$ProfileDirectory'."
    }
}

# Java-based tooling needs a truststore that explicitly trusts the generated
# broker certificate because these profiles rely on self-signed server material.
if (-not (Test-Path $certificatePath)) {
    & docker run --rm `
        -v "${mountPath}:/work" `
        eclipse-temurin:17-jdk `
        keytool `
        -exportcert `
        -alias kafka `
        -keystore /work/kafka.keystore.p12 `
        -storetype PKCS12 `
        -storepass $StorePassword `
        -rfc `
        -file /work/kafka.crt

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to export the broker certificate for profile '$ProfileDirectory'."
    }
}

if (-not (Test-Path $truststorePath)) {
    & docker run --rm `
        -v "${mountPath}:/work" `
        eclipse-temurin:17-jdk `
        keytool `
        -importcert `
        -alias kafka `
        -file /work/kafka.crt `
        -keystore /work/client.truststore.p12 `
        -storetype PKCS12 `
        -storepass $StorePassword `
        -noprompt

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to generate the client truststore for profile '$ProfileDirectory'."
    }
}
