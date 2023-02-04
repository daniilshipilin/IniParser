@echo off

set "cert=%CodesignCertPath%"
set "timestamp=http://timestamp.digicert.com"
set "src=.\bin\Release\netstandard2.1"

del /S /Q "%src%" >nul 2>&1

dotnet build -c Release

for /r %%i in (%src%\*.dll) do (
    signtool.exe sign /fd sha256 /a /f "%cert%" "%%i"
    signtool.exe timestamp /tr "%timestamp%" /td sha256 "%%i"
)

for /r %%i in (%src%\*.exe) do (
    signtool.exe sign /fd sha256 /a /f "%cert%" "%%i"
    signtool.exe timestamp /tr "%timestamp%" /td sha256 "%%i"
)

dotnet pack -c Release --no-build

for /r %%i in (".\bin\Release\*.nupkg") do (
    nuget.exe sign %%i -CertificatePath "%cert%" -Timestamper "%timestamp%"
)

pause
