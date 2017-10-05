# "build.ps1","build.cake"|%{Invoke-RestMethod -Uri "https://raw.githubusercontent.com/cake-build/bootstrapper/master/res/scripts/$($_)" -OutFile $_}
#"build.ps1"|%{Invoke-RestMethod -Uri "https://raw.githubusercontent.com/cake-build/bootstrapper/master/res/scripts/$($_)" -OutFile $_}
Invoke-WebRequest http://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1