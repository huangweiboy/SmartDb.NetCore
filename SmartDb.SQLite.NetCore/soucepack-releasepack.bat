@echo off
set soucepackpath=F:\work\dotnetcore-work\SmartDb-work\SmartDbNetCoreSouceTest\
set packpath=%~dp0

xcopy "%soucepackpath%SmartDb.SQLite.NetCore\push-package.bat" "%packpath%" /r /c /v /y
call "%packpath%push-package.bat"
del "%packpath%push-package.bat"
pause
