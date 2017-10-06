@echo off
:start
powershell -f azure\provision.ps1 -Environment dev -Verbose
if errorlevel 1 (
echo error 1
pause
goto start
)

:end
pause