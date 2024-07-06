$ErrorActionPreference = 'Stop'

$toolsDir = $(Split-Path -parent $MyInvocation.MyCommand.Definition)

$filePath = Join-Path $toolsDir "ConfuserEx-CLI.zip"

$packageArgs = @{
  packageName = 'confuserex.commandline'
  destination = "$toolsDir"
  file        = $filePath
}
Get-ChocolateyUnzip @packageArgs
Remove-Item $filePath
