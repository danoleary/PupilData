
.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

@echo "restoring paket dependencies"
.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)

REM packages\FAKE\tools\FAKE.exe deploy.fsx

@echo "Copying files to web root"
xcopy /s /y . d:\home\site\wwwroot\
