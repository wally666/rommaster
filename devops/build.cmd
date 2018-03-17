:start
if not exist tools\Cake (
	powershell -f bootstrapper.ps1 -Verbose
)

powershell -f ./build.ps1 -Target BuildAll -Verbosity=Diagnostic --settings_skipverification=true
if errorlevel 1 (
echo error 1
pause
goto start
)

:end
pause