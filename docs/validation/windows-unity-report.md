# Windows Unity 작업 보고 — 2026-11-11

## 결과

S1 이전 환경 준비에서 중단. Unity 6.3 LTS 설치 실패 시 멈추라는 요청을 준수했다.
S1~S9는 완료하지 않았으며, Unity 컴파일/PlayMode 테스트/Console Error 0을 검증하지 않았다.

- 작업 위치: C:\Users\nonam\Documents\MagicEscape, main
- 시작 시 git pull: Already up to date. 시작 HEAD: 12f4d67.
- AGENTS.md → UNITY_HANDOFF.md → SCENE_SETUP.md → PUZZLE_SPEC.md → TEST_PLAN.md 순서로 확인. Runtime/Core 초안 및 TECH_DESIGN도 참조.
- 시간: Git Bash date로 2026-11-11, PowerShell로 KST 확인.
- Hub releases 명령이 제공한 최신 6000.3.x: **6000.3.28f1**. 설치 완료 버전은 없음.
- Hub 지정 설치 루트: D:\Unity\Editors. 대상 Editor/Unity.exe는 실패 후 존재하지 않음.
- Windows Build Support 기본 포함 여부: 설치되지 않아 미확인.
- URP 템플릿/대체 방식: 프로젝트 생성에 도달하지 않아 미선택.
- UnityProject/ 미생성. .unity/.prefab/.asset/.meta 수기 작성 없음. 외부 에셋/XR 설치 없음.

## 실제 명령

첫 Hub 직접 실행은 상속된 ELECTRON_RUN_AS_NODE=1 때문에 MODULE_NOT_FOUND로 실패했다.
해당 PowerShell 프로세스에서만 환경 변수를 제거하고 Start-Process로 stdout/stderr를 수집하니 releases 조회가 정상 완료됐다.

```powershell
git pull
git status --short
& 'C:\Program Files\Git\bin\bash.exe' -c date
Remove-Item Env:ELECTRON_RUN_AS_NODE -ErrorAction SilentlyContinue
$p = Start-Process 'C:\Program Files\Unity Hub\Unity Hub.exe' -ArgumentList '--','--headless','editors','--releases' -WindowStyle Hidden -RedirectStandardOutput docs/validation/hub-releases.stdout.log -RedirectStandardError docs/validation/hub-releases.stderr.log -PassThru
$p.WaitForExit() # ExitCode=0
$p = Start-Process 'C:\Program Files\Unity Hub\Unity Hub.exe' -ArgumentList '--','--headless','install','--version','6000.3.28f1' -WindowStyle Hidden -RedirectStandardOutput docs/validation/hub-install.stdout.log -RedirectStandardError docs/validation/hub-install.stderr.log -PassThru
$p.WaitForExit() # ExitCode=1
```

Hub가 다운로드 4,208,792,576 bytes 완료 후 설치 승격 단계에서 실패했다.
consent.exe 프로세스를 관측했지만 CUA 앱/창 목록에는 접근 가능한 승인 창이 없었다.
승인을 수행하거나 UAC를 우회하지 않았다. 취소인지 시간 초과인지는 오류만으로 확정할 수 없다.

### 오류 전문

```console
[Unity (6000.3.28f1)] failed to install. Error given: The Windows elevation prompt was cancelled or timed out.
Failed to install.
```

Hub 내부 오류/스택 원문:

```console
Installer child process failed: [Error: 1223 - The user cancelled the operation] { code: 'GenericFailure' } undefined
Error while preparing install service: [Error: 1223 - The user cancelled the operation] { code: 'GenericFailure' }
Installation cancelled by the user during UAC prompt.
Validation FAILED.
Validation failed. Reason: The Windows elevation prompt was cancelled or timed out.
Validation failed. Stack: ELEVATION_CANCELLED: The Windows elevation prompt was cancelled or timed out.
    at InstallerItem.onValidation (file:///C:/Program%20Files/Unity%20Hub/resources/app.asar/.vite/build/InstallationManager-DC41JW8_.js:574:28)
```

전체 설치 stdout/stderr는 이 폴더의 hub-install.*.log, 내부 설치 관련 JSON 로그는 hub-install-errors.jsonl에 보존했다.

## 단계별 상태

모든 단계의 기능 커밋은 없음. 문서/증거만 별도 커밋한다.

| 단계 | 한 일 / 결과 | 실제 검증 | 미검증·남은 문제 |
|---|---|---|---|
| S1 | 선행 설치 시도 후 중단, 씬/프로젝트 미생성 | Hub releases exit 0, install exit 1 | 6.3 설치 및 Build Support 확인 후 재개 |
| S2 | 미착수 | PlayMode 0건 실행, Console 오류 수 미측정 | 스크립트 복사·플레이어·Unity 컴파일 |
| S3 | 미착수 | 미실행 | UI·한글 렌더링·입력 감각 수동 확인 필요 |
| S4 | 미착수 | 미실행 | ItemData 7종 |
| S5 | 미착수 | 미실행 | 단서·읽기 패널 |
| S6 | 미착수 | 미실행 | P1 전체 |
| S7 | 미착수 | 미실행 | P2 전체 |
| S8 | 미착수 | 미실행 | P3·문·ExitTrigger |
| S9 | 미착수 | 미실행, 통과 수 0(테스트 미실행), Console Error 0 주장 안 함 | 전체 탈출 자동화 및 수동 확인 |

TC-W01~W44 기존 결과는 모두 미실행으로 유지했다. 설치 실패는 게임 TC 실패와 구별하여 TEST_PLAN §3에 환경 결함으로 기록했다.
S10/S11 범위 밖 작업은 하지 않았다.

## 변경 및 재개 조건

- 변경: docs/TASKS.md, docs/TEST_PLAN.md, docs/validation/의 보고서 및 원문 로그.
- CodeDrafts와 달라진 스크립트: **없음**. 스크립트 복사/수정 없음.
- 큰 설계 판단: **없음**. 설계 정본 불변. Runtime asmdef 판단 전 단계에서 중단.
- 재개 조건: 사람이 Windows 관리자 승인 가능한 상태에서 Unity Hub로 6000.3.28f1(재개 시 최신 6000.3.x 재조회)을 설치 완료한다. 설치·Build Support 확인 뒤 S1부터 시작한다.
- 기존 6000.6이나 디스크에 발견된 이전 6000.3.26f1로 대체하지 않았다.
