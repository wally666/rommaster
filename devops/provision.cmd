:start
powershell -f ./build.ps1 -Target Provision -Verbose --settings_skipverification=true
if errorlevel 1 (
echo error 1
pause
goto start
)

:end
pause