SETLOCAL 
set DOCKER_HOST=
docker build -t keepass-pinentry .
docker create --name kb keepass-pinentry
docker cp kb:C:\app\KeepassPinentry\bin\Debug\KeepassPinentry.dll KeepassPinentry.dll
docker rm -v kb
ENDLOCAL