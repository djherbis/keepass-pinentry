:: Creates the cert
if not exist "%~dp0certificate.pem" openssl req -newkey rsa:4096 -nodes -keyout key.pem -x509 -days 365 -out certificate.pem
:: Save output files & pwd in keepass under TLSKEY
if not exist "%~dp0certificate.p12" openssl pkcs12 -inkey key.pem -in certificate.pem -export -out certificate.p12
