param([string]$Name, [string]$Method, [switch]$Tests, [string]$TestFilter, [ValidateSet("PlayMode","EditMode")][string]$TestPlatform = "PlayMode")
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe'
$freeCommitGB = (Get-CimInstance Win32_OperatingSystem).FreeVirtualMemory / 1MB
Write-Output ('Free commit headroom: {0:N2} GB' -f $freeCommitGB)
if ($freeCommitGB -lt 6) { throw "Unity launch refused: free commit headroom $freeCommitGB GB is below 6 GB." }
$runningEditors = @(Get-CimInstance Win32_Process | Where-Object { $_.ExecutablePath -like '*\Editor\Unity.exe' })
if ($runningEditors.Count -gt 0) { throw ('Unity launch refused: Editor already running (PID ' + ($runningEditors.ProcessId -join ', ') + ').') }
$project = (Resolve-Path UnityProject).Path
$log = Join-Path (Get-Location) "docs/validation/$Name.log"
$arguments = "-batchmode -projectPath `"$project`" -logFile `"$log`""
if ($Tests) {
    $result = Join-Path (Get-Location) "docs/validation/$Name.xml"
    $arguments += " -runTests -testPlatform $TestPlatform -testResults `"$result`""
    if ($TestFilter) { $arguments += " -testFilter `"$TestFilter`"" }
} else { $arguments += " -quit -executeMethod $Method" }
# Snapshot before launching Unity as its initial import may also reserialize shared assets / fonts.
$preserved = @{}
if ($Tests -or $Method -like "*Warehouse") {
    git ls-files docs/validation UnityProject/Assets/Fonts UnityProject/Assets/Settings UnityProject/Assets/Prefabs UnityProject/Assets/Data UnityProject/Assets/Art/S10c/Icons UnityProject/Assets/Scenes/Classroom.unity | ForEach-Object {
        if ($_ -notlike 'docs/validation/warehouse/stage10-*' -and $_ -notlike 'UnityProject/Assets/Settings/*Warehouse*' -and $_ -ne 'docs/validation/warehouse/scene-manifest.txt' -and (Test-Path -LiteralPath $_)) {
            $preserved[$_] = [System.IO.File]::ReadAllBytes((Join-Path (Get-Location) $_))
        }
    }
}
$p = Start-Process $editor -ArgumentList $arguments -WindowStyle Hidden -PassThru
try { $p.WaitForExit() } finally {
    foreach ($file in $preserved.Keys) { [System.IO.File]::WriteAllBytes((Join-Path (Get-Location) $file), $preserved[$file]) }
}
Write-Output "Unity exit=$($p.ExitCode); log=$log"
Select-String -Path $log -Pattern 'error CS','^[A-Za-z.]*Exception:','Error:' | ForEach-Object { $_.Line }
if ($Tests -and (Test-Path $result)) {
    [xml]$xml = Get-Content $result
    $xml.'test-run' | Select-Object result,total,passed,failed,skipped
}
exit $p.ExitCode

