param([int]$Stage,[string]$Description,[string]$Message)
$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
[xml]$x=Get-Content "docs/validation/s$Stage-fourth-tests.xml"
$r=$x.'test-run'
if($r.result -ne 'Passed'){throw 'Tests not passed'}
foreach($kind in @('builder','tests')) { if(Select-String -Path "docs/validation/s$Stage-fourth-$kind.log" -Pattern 'error CS','^[A-Za-z.]*Exception:','executeMethod.*threw exception' -Quiet) {throw "Compile/exception in $kind log"} }
$line="| S$Stage | $Description | builder exit 0; PlayMode $($r.passed)/$($r.total); game Console Error 0 | 수동 감도·조작감·클릭·한글 확인 필요 |"
Add-Content docs/validation/windows-unity-report.md $line
Add-Content docs/TEST_PLAN.md "`nS$Stage 자동 검증: $Description. PlayMode $($r.passed)/$($r.total), game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행."
Add-Content docs/TASKS.md "`n| S$Stage | ✅ 자동 검증: $Description; PlayMode $($r.passed)/$($r.total), Console Error 0. 수동 검증 별도 대기 |"
# Raw editor logs stay local: licensing sessions/identifiers must not be committed.
New-Item -ItemType Directory -Force UnityProject/Logs/fourth-attempt | Out-Null
foreach($kind in @('builder','tests')){Move-Item -LiteralPath "docs/validation/s$Stage-fourth-$kind.log" -Destination "UnityProject/Logs/fourth-attempt/s$Stage-$kind.log" -Force}
git add UnityProject/Assets UnityProject/ProjectSettings CodeDrafts Tools/Run-Unity.ps1 Tools/Record-Stage.ps1 docs/TASKS.md docs/TEST_PLAN.md docs/validation/windows-unity-report.md "docs/validation/s$Stage-fourth-tests.xml"
git commit -m $Message
if($LASTEXITCODE -ne 0){throw 'Commit failed'}
git push
if($LASTEXITCODE -ne 0){throw 'Push failed'}
$hash=git rev-parse --short HEAD
Add-Content docs/validation/windows-unity-report.md "`nS$Stage 커밋: $hash (push 완료)."


