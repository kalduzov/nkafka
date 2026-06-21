#!/usr/bin/env sh

set -eu

PROFILE_DIRECTORY="$1"
STORE_PASSWORD="${2:-changeit}"

CERTIFICATES_DIRECTORY="$PROFILE_DIRECTORY/certs"
KEYSTORE_PATH="$CERTIFICATES_DIRECTORY/kafka.keystore.p12"
CERTIFICATE_PATH="$CERTIFICATES_DIRECTORY/kafka.crt"
TRUSTSTORE_PATH="$CERTIFICATES_DIRECTORY/client.truststore.p12"

if [ -f "$KEYSTORE_PATH" ] && [ -f "$CERTIFICATE_PATH" ] && [ -f "$TRUSTSTORE_PATH" ]; then
  exit 0
fi

mkdir -p "$CERTIFICATES_DIRECTORY"

if [ ! -f "$KEYSTORE_PATH" ]; then
  # The E2E matrix must stay self-contained, so certificate material is generated
  # inside a disposable JDK container instead of depending on host-installed tools.
  docker run --rm \
    -v "$CERTIFICATES_DIRECTORY:/work" \
    eclipse-temurin:17-jdk \
    keytool \
    -genkeypair \
    -alias kafka \
    -keyalg RSA \
    -storetype PKCS12 \
    -keystore /work/kafka.keystore.p12 \
    -storepass "$STORE_PASSWORD" \
    -keypass "$STORE_PASSWORD" \
    -dname "CN=localhost" \
    -ext "SAN=dns:localhost,ip:127.0.0.1" \
    -validity 3650 \
    -noprompt
fi

# Java-based tooling needs a truststore that explicitly trusts the generated
# broker certificate because these profiles rely on self-signed server material.
if [ ! -f "$CERTIFICATE_PATH" ]; then
  docker run --rm \
    -v "$CERTIFICATES_DIRECTORY:/work" \
    eclipse-temurin:17-jdk \
    keytool \
    -exportcert \
    -alias kafka \
    -keystore /work/kafka.keystore.p12 \
    -storetype PKCS12 \
    -storepass "$STORE_PASSWORD" \
    -rfc \
    -file /work/kafka.crt
fi

if [ ! -f "$TRUSTSTORE_PATH" ]; then
  docker run --rm \
    -v "$CERTIFICATES_DIRECTORY:/work" \
    eclipse-temurin:17-jdk \
    keytool \
    -importcert \
    -alias kafka \
    -file /work/kafka.crt \
    -keystore /work/client.truststore.p12 \
    -storetype PKCS12 \
    -storepass "$STORE_PASSWORD" \
    -noprompt
fi
