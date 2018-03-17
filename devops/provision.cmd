:start
powershell -File ../../../devops/Azure/provision.ps1 -Verbose -ErrorAction Stop -Environment dev
if errorlevel 1 (
echo error 1
pause
goto start
)

:end
pause