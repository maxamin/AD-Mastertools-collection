[CmdletBinding()]
param (
	[Parameter(Mandatory=$true)]
	[string]
	$PackageVersion
)

if ($PackageVersion.StartsWith('v')) {
	$PackageVersion = $PackageVersion.Substring(1)
}

Push-Location $PSScriptRoot
try {
	Get-ChildItem .\ -Directory | ForEach-Object {
		Get-ChildItem $_.FullName -Filter *.nuspec | Remove-Item -Force -ErrorAction SilentlyContinue

		$packageHash = Get-ChildItem $_.FullName -Filter tools\*.zip |
			Select-Object -First 1 |
			Get-FileHash -Algorithm SHA256

		$replaceDict = @{
			'{{PACKAGEVERSION}}' = $PackageVersion;
			'{{CHECKSUMTYPE}}' = $packageHash.Algorithm;
			'{{CHECKSUM}}' = $packageHash.Hash
		}

		Get-ChildItem $_.FullName -Filter *.template -Recurse |
			ForEach-Object {
				Get-Content -Path $_.FullName |
					ForEach-Object {
						$line = $_
						$replaceDict.GetEnumerator() | ForEach-Object {
							$line = $line -replace $_.Key, $_.Value
						}
						$line
					} |
					Out-File (Join-Path $_.Directory $_.BaseName)
			}
	}

	Get-ChildItem *.nuspec -Recurse -Depth 1 | ForEach-Object { . choco pack $_.FullName }
} finally {
	Pop-Location
}
