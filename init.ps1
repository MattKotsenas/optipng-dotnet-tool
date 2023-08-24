Set-StrictMode -Version latest
$ErrorActionPrefernce = "Stop"

Write-Progress -Activity "Setting up environment variables"

# From https://github.com/microsoft/vswhere/wiki/Start-Developer-Command-Prompt
$installationPath = & "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property installationPath
if ($installationPath -and (Test-Path "$installationPath\Common7\Tools\vsdevcmd.bat")) {
  & "${env:COMSPEC}" /s /c "`"$installationPath\Common7\Tools\vsdevcmd.bat`" -no_logo && set" | Foreach-Object {
    $name, $value = $_ -split '=', 2
    Set-Content env:\"$name" $value
  }
}
