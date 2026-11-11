param([string]$Name, [string]$Method, [switch]$Tests)
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe'
$project = (Resolve-Path UnityProject).Path
$log = Join-Path (Get-Location) "docs/validation/$Name.log"
$arguments = "-batchmode -projectPath `"$project`" -logFile `"$log`""
if ($Tests) {
    $result = Join-Path (Get-Location) "docs/validation/$Name.xml"
    $arguments += " -runTests -testPlatform PlayMode -testResults `"$result`""
} else { $arguments += " -quit -executeMethod $Method" }
$p = Start-Process $editor -ArgumentList $arguments -WindowStyle Hidden -PassThru
$p.WaitForExit()
Write-Output "Unity exit=$($p.ExitCode); log=$log"
Select-String -Path $log -Pattern 'error CS','Exception','Error' | ForEach-Object { $_.Line }
if ($Tests -and (Test-Path $result)) {
    [xml]$xml = Get-Content $result
    $xml.'test-run' | Select-Object result,total,passed,failed,skipped
}
exit $p.ExitCode
