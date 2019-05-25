REM Build the file if it doesn't already exist.
if not exist "%~dp0KeepassPinentry.dll" call build.cmd

REM Backup the current plugin, and copy over the new one.
call backup.cmd
set plugins=C:\Program Files (x86)\KeePass Password Safe 2\Plugins
mv KeepassPinentry.dll "%plugins%"

REM Create cert files & password, and setup stdproxy env vars.
call createcert.cmd
setx STDPROXY_PORT "500"
set STDPROXY_PORT=500
setx STDPROXY_CERT %~dp0certificate.pem
set STDPROXY_CERT=%~dp0certificate.pem

REM Tell gpg-agent to use stdproxy as the pinentry program.
echo pinentry-program %~dp0stdproxy.exe > %USERPROFILE%/.gnupg/gpg-agent.conf
gpgconf --kill gpg-agent