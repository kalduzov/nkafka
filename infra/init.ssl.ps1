
#install openssl
choco install openssl

$curDir = Get-Location

try
{
    $certDir = "$env:APPDATA\ksecrets"
    md $certDir -Force

    $tld = "local"
    $password ="kafka"
    
    keytool -genkeypair -alias "broker" -dname "CN=broker.$tld, OU=CIA, O=REA, L=Melbourne, S=VIC, C=AU" -keystore "kafka.broker.keystore.jks" -keyalg RSA -storepass @password -keypass $password;
}
catch
{
}
finally
{
    cd $curDir
}

