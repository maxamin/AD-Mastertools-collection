$ErrorActionPreference = 'Stop'

$toolsDir = $(Split-Path -parent $MyInvocation.MyCommand.Definition)

$filePath = Join-Path $toolsDir "ConfuserEx.zip"

$packageArgs = @{
  packageName = 'confuserex'
  destination = "$toolsDir"
  file        = "$filePath"
}
Get-ChocolateyUnzip @packageArgs
Remove-Item $filePath

New-Item "$($packageArgs.destination)\ConfuserEx.exe.gui" -Type file -Force | Out-Null
