:start
powershell -f ./build.ps1 -Target Default -Verbose
if errorlevel 1 (
echo error 1
pause
goto start
)

:end
pause