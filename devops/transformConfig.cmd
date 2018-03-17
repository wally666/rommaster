:start
if not exist tools\Cake (
	powershell -f bootstrapper.ps1 -Verbose
)

powershell -File build.ps1 -Target TransformConfig -Verbosity=Diagnostic --settings_skipverification=true -environment="dev" --buildNumber="1"
if errorlevel 1 (
echo error 1
pause
goto start
)

:end
pause