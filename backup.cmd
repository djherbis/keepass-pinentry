set plugins=C:\Program Files (x86)\KeePass Password Safe 2\Plugins
if exist "%plugins%\KeepassPinentry.dll" cp "%plugins%\KeepassPinentry.dll" .\backup.KeepassPinentry.dll
if exist "%USERPROFILE%/.gnupg/gpg-agent.conf" cp "%USERPROFILE%/.gnupg/gpg-agent.conf" .\backup.gpg-agent.conf