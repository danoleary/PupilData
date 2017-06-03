
.paket\paket.bootstrapper.exe
if errorlevel 1 (
  exit /b %errorlevel%
)

@echo "restoring paket dependencies"
.paket\paket.exe restore
if errorlevel 1 (
  exit /b %errorlevel%
)


packages\FAKE\tools\FAKE.exe %* --fsiargs deploy.fsx
