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

---

# 재개 시도 — 2026-11-12 (KST)

## 결과

**S1 프로젝트 생성에서 라이선스 오류로 중단. S1~S9 미완료.**
설치는 완료됐지만 Unity Editor의 유효한 라이선스가 확인되지 않았다. 지시대로 다른 버전·설계 변경·라이선스 우회 없이 멈췄다.

- 시작 `git pull`: Already up to date. HEAD `184edaa`, main, 시작 작업 트리 깨끗함.
- 작성자 로컬 설정 확인: NoNamad5196 / 203311115+NoNamad5196@users.noreply.github.com. 변경하지 않음.
- Git Bash `date`: Thu Nov 12 00:12:01 2026. 처음 일반 `bash -c date`는 WSL에 bash가 없어 실패해 Git Bash 절대 경로로 재실행했다.
- AGENTS.md → UNITY_HANDOFF.md → SCENE_SETUP.md → PUZZLE_SPEC.md → TEST_PLAN.md 및 이전 보고서/TASKS를 읽었다.
- 지정 Editor 파일 및 `Editor/Data/PlaybackEngines/windowsstandalonesupport` 존재 확인.
- 실행 로그 버전: `6000.3.28f1 (8f082d2fe476)`; 6000.6.6f1 미사용.
- Hub UI 템플릿은 사용하지 않고 허용된 `-createProject` 후 URP 패키지/Asset 설정 대체 경로를 시도했다. 라이선스에서 종료되어 URP 설정에는 도달하지 못했다.
- 생성된 UnityProject에는 Logs/Temp/UserSettings만 있으며 Assets/Packages/ProjectSettings는 없다. 프로젝트 생성 성공 아님.
- `UnityProject/.gitignore` 추가: Library/Temp/Logs/UserSettings/Builds 등 생성물 제외. 생성물 삭제 없이 보존, 커밋 제외.
- CodeDrafts 복사/수정, Unity YAML 수기 작성, XR/외부 에셋 설치 없음.

## 실제 명령 및 판정

저장소 위치에서 실행했다. 프로세스는 Hidden으로 시작했고 오류 판정은 로그를 읽어 수행했다.

```powershell
git pull
git status --short
& 'C:\Program Files\Git\bin\bash.exe' -c date
git config user.name
git config user.email
$editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe'
Test-Path $editor
Get-ChildItem 'C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Data\PlaybackEngines'
$p = Start-Process $editor -ArgumentList '-batchmode -nographics -quit -createProject "C:\Users\nonam\Documents\MagicEscape\UnityProject" -logFile "C:\Users\nonam\Documents\MagicEscape\docs\validation\s1-create.log"' -WindowStyle Hidden -PassThru
Get-Content docs/validation/s1-create.log -Tail 90
Select-String -Path docs/validation/s1-create.log -Pattern 'error CS','Exception','Error'
```

- Unity 로그에 종료 코드 **198** 명시. 프로세스 API에서 ExitCode를 직접 수집한 것은 아님.
- `error CS`: 0줄, `Exception`: 0줄, `Error` 포함: **9줄**(라이선스 환경 오류).
- Unity 씬/스크립트 로드 이전 종료이므로 게임 Console Error 수는 **미측정**, 0 Error 통과로 판정하지 않는다.
- 빌더/PlayMode `-runTests -testPlatform PlayMode`: **미실행, 통과/전체 N/A**(0개 실행). 라이선스 실패 뒤 추가 단계는 시도하지 않았다.
- 로그: [s1-create.log](s1-create.log). 오류/스택은 생략하지 않고 보존했으며, 네트워크 IP·세션/상관관계/기계 식별자만 비식별화했다. Unity는 이 실패에 관리 코드 스택을 출력하지 않았다.

### 오류 전문 (로그의 Error 포함 9줄 및 관련 설명)

```console
[Licensing::Client] Error: HandshakeResponse reported an error:
    ResponseCode: 505
    ResponseStatus: Unsupported protocol version '1.19.0'.
[Licensing::Module] Error: Failed to handshake to channel: "LicenseClient-nonam"
[Licensing::Module] Error: Access token is unavailable; failed to update
[Licensing::Client] Error: Code 404 while processing request (status: Found 0 entitlement groups and 0 free entitlements matching requested entitlement ids)
[Licensing::Client] Error: Code 404 while processing request (status: Found 0 entitlement groups and 0 free entitlements matching requested entitlement ids)
[Licensing::Module] Error: 'com.unity.editor.headless' was not found.
[Licensing::Client] Error: Code 404 while processing request (status: Found 0 entitlement groups and 0 free entitlements matching requested entitlement ids)
[Licensing::Module] Error: 'com.unity.editor.headless' was not found.
[Licensing::Client] Error: Code 404 while processing request (status: Found 0 entitlement groups and 0 free entitlements matching requested entitlement ids)
No valid Unity Editor license found. Please activate your license.
Exiting without the bug reporter. Application will terminate with return code 198
```

초기 Hub Licensing Client handshake 실패 뒤 Editor에 포함된 1.19.0 클라이언트 연결은 성공했다. 최종 실패는 유효한 Editor 라이선스 없음이다. 특정 구독 종류나 계정 상태는 추정하지 않는다.

## 단계별 인계

아래 모든 단계의 기능 커밋 없음. 이번 커밋은 환경 증거/문서/.gitignore만 포함한다.

| 단계 | 한 일 | 실제 검증 / 통과·전체 / Console | 수동 확인 및 남은 문제 |
|---|---|---|---|
| S1 | 설치·Support 폴더 확인, 생성 시도, .gitignore 추가 | createProject 로그 exit 198; PlayMode N/A; 환경 Error 9줄, 게임 Console 미측정 | 라이선스 활성화 후 프로젝트 생성·URP·빌더·테스트부터 재개 |
| S2 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | 스크립트 복사/플레이어. 마우스 감도·조작감 수동 확인 필요 |
| S3 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | UI/한글 폰트. UI 클릭·한글 렌더링 수동 확인 필요 |
| S4 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | ItemData 7종 |
| S5 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | 단서·읽기 패널 |
| S6 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | P1 정답/오답/보상/봉인 |
| S7 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | P2 잠금/재생성/보상 |
| S8 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | P3/출구/실제 Trigger/Cleared |
| S9 | 미착수 | 명령 미실행; PlayMode N/A; Console 미측정 | 전체 클리어/재시작/Console 0 미검증 |

- TEST_PLAN의 게임 TC는 모두 ⏳ 유지. 환경 결함 ENV-W02를 §3에 기록했다. S10/S11은 범위 밖이며 미실행.
- **CodeDrafts와 달라진 스크립트: 없음**(Assets/Scripts 미생성).
- **큰 설계 판단: 없음.** Runtime asmdef 필요성 판단 이전. 정답/itemId/상태/문구 정본은 불변.
- 재개 조건: 사용자가 Unity Hub에서 해당 계정의 유효한 라이선스를 활성화해 6000.3.28f1 Editor가 실행되는지 확인한 뒤, S1 프로젝트 생성부터 재시도. 비밀번호·토큰을 채팅/문서에 남기지 않는다.

---

# 세 번째 시도 — 2026-11-12 (KST)

## 결과

**createProject exit 0. S1 빌더 실행 전 Input System 패키지 컴파일 오류(exit 1)로 중단. S1~S9 미완료.**
요청의 중단 규칙을 적용해 패키지 소스 수정/버전 변경 재시도나 이후 단계 실행은 하지 않았다. 라이선스 실패가 아니므로 라이선스 재시도 조건은 해당하지 않는다.

- 시작 HEAD `9af200e`, main, 작업 트리 깨끗함. `git pull`: Already up to date.
- 작성자 NoNamad5196 / 저장소 로컬 noreply 설정 유지.
- Git Bash date: Thu Nov 12 00:22:26 2026 (KST). 첫 WSL bash 호출은 /bin/bash가 없어 실패했고 지정 Git Bash로 재확인했다.
- 문서는 AGENTS → UNITY_HANDOFF → SCENE_SETUP → PUZZLE_SPEC → TEST_PLAN 순서로 읽고, 출력 잘림 부분은 개별 재독했다. AGENTS.md/정답/itemId/문구 정본 변경 없음.
- 지정 Editor 6000.3.28f1만 사용. `ProjectSettings/ProjectVersion.txt`에도 동일 버전 기록.
- 모든 Unity 실행에서 **-nographics 미사용**.
- Hub 템플릿 대신 허용된 `-createProject` 경로 사용. 프로젝트 생성은 성공했으나 URP Asset/씬 설정 실행 전 컴파일에서 중단됐다.
- 패키지 버전의 출처: 설치 Editor의 `Data/Resources/PackageManager/ProjectTemplates/com.unity.template.3d-cross-platform-17.0.14.tgz` 안 `package/ProjectData~/Packages/manifest.json`. 요청 버전 URP 17.0.1/Input System 1.12.0/uGUI 2.0.0/Test Framework 1.4.2. 실제 lock: URP **17.3.0** 및 Test Framework **1.6.0**(Editor builtin), Input System **1.12.0**(registry), uGUI 2.0.0.
- 생성 기본 manifest의 multiplayer.center 및 VR/XR 모듈 직접 의존성을 제거했다. 최종 lock에 VR/XR 패키지 없음. 유료 에셋/외부 폰트 미도입.
- `.gitignore`는 기존 내용 유지 및 Library/Logs/Temp/UserSettings 제외 확인.
- Editor API 기반 멱등 S1 빌더 소스, 별도 PlayMode asmdef와 S1 테스트 소스, 재실행용 PowerShell 래퍼를 추가했다. **소스는 미검증이며 빌더 메서드 자체는 실행되지 않았다.** .unity/.prefab/.asset/.meta YAML 수기 작성 없음(.meta/ProjectSettings는 Unity 생성).
- Assets/Scripts 복사, CodeDrafts 수정, Tests/ 복사 없음. UnityStubs.cs 없음. Runtime asmdef 미추가.

## 실제 명령 및 결과

저장소 루트에서 실행:

```powershell
git status --short
git pull
& 'C:\Program Files\Git\bin\bash.exe' -c date
git config user.name
git config user.email
$p = Start-Process 'C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe' -ArgumentList '-batchmode -quit -createProject "C:\Users\nonam\Documents\MagicEscape\UnityProject" -logFile "C:\Users\nonam\Documents\MagicEscape\docs\validation\s1-third-create.log"' -WindowStyle Hidden -PassThru
$p.WaitForExit()
$p.ExitCode # 0
./Tools/Run-Unity.ps1 -Name s1-builder -Method MagicEscape.EditorTools.ClassroomBuilder.S1
# wrapper: -batchmode -projectPath <UnityProject> -logFile <s1-builder.log> -quit -executeMethod MagicEscape.EditorTools.ClassroomBuilder.S1
# exit 1
Select-String -Path docs/validation/s1-builder.log -Pattern 'error CS','Exception','Error'
```

- createProject: exit 0. Handshake/Access token Error가 있어도 entitlement resolve 및 프로젝트 생성 완료. 라이선스 오류로 종료된 이전 시도와 구별한다.
- S1 builder: exit 1. `error CS` **3줄(동일 CS0117 1건 중복 출력)**, `Exception` **1줄(컴파일 입력 목록의 ExceptionHelpers.cs 파일명, 런타임 예외 아님)**, `Error` **9줄**. Error 줄에는 라이선스 초기 handshake/token 3줄 및 LogAssemblyErrors/컴파일 요약이 섞여 있다. 원시 문자열 매치 수를 Console 이벤트 수로 치환하지 않는다.
- **컴파일 오류 1종 존재, Console Error 0 기준 불충족.** 씬 Play 미실행이므로 게임 Console 오류 수 미측정.
- `-runTests -testPlatform PlayMode`: 컴파일 실패에서 중단해 미실행, 통과/전체 **N/A (0개 실행)**. XML 결과 없음. 새 테스트 파일 1건을 작성했지만 테스트 발견/통과 여부 미검증.
- 원문 오류/스택은 아래 및 [s1-builder.log](s1-builder.log). 전체 생성 로그는 [s1-third-create.log](s1-third-create.log). 커밋 로그는 네트워크/세션/기계/라이선스 식별자만 비식별화. 로컬 원본은 gitignore된 UnityProject/Logs/third-attempt에 보존했다. 관리 코드 예외 스택은 출력되지 않았다.

### 차단 오류 전문

```console
Library\PackageCache\com.unity.inputsystem@920b46832575\InputSystem\Editor\InputSystemPluginControl.cs(47,25): error CS0117: 'BuildTarget' does not contain a definition for 'ReservedCFE'
## Script Compilation Error for: Csc Library/Bee/artifacts/1900b0aE.dag/Unity.InputSystem.dll (+2 others)
## CmdLine: "C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Data\NetCoreRuntime\dotnet.exe" exec "C:/Program Files/Unity/Hub/Editor/6000.3.28f1/Editor/Data/DotNetSdkRoslyn/csc.dll" /nostdlib /noconfig /shared "@Library/Bee/artifacts/1900b0aE.dag/Unity.InputSystem.rsp" "@Library/Bee/artifacts/1900b0aE.dag/Unity.InputSystem.rsp2"
## Output:
Library\PackageCache\com.unity.inputsystem@920b46832575\InputSystem\Editor\InputSystemPluginControl.cs(47,25): error CS0117: 'BuildTarget' does not contain a definition for 'ReservedCFE'

Library\PackageCache\com.unity.inputsystem@920b46832575\InputSystem\Editor\InputSystemPluginControl.cs(47,25): error CS0117: 'BuildTarget' does not contain a definition for 'ReservedCFE'
AssetDatabase: script compilation time: 25.746714s
Scripts have compiler errors.
Exiting without the bug reporter. Application will terminate with return code 1
```

로컬 패키지 소스 46~48행에서 `#if UNITY_6000_0_OR_NEWER` 아래 `BuildTarget.ReservedCFE`를 참조함을 확인했다. 직접 패키지 캐시/Editor API/설계 정본을 수정하지 않았다. 재개 시에는 6000.3.28f1 호환 Input System 패키지 버전 확인이 먼저 필요하다. 버전 후보나 수정 효과는 아직 검증하지 않았다.

## S1~S9 인계

아래 기능 단계의 성공 커밋은 모두 없음. 이번 커밋은 실패한 환경 준비/빌더 초안 및 증거 보존용이며, 완료 커밋이 아니다.

| 단계 | 한 일 | 실제 검증 / 통과·전체 / Console | 커밋 | 수동 확인 / 남은 문제 |
|---|---|---|---|---|
| S1 | 프로젝트 생성, URP 패키지 추가, 씬 빌더·PlayMode 테스트 초안 | create exit 0; builder exit 1; PlayMode N/A; CS0117 1종, 게임 Console 미측정 | 기능 완료 커밋 없음; 본 증거 커밋 | Input System 호환 오류 해결 후 빌더 실행·씬 저장·테스트 필요 |
| S2 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | Core/Runtime 복사·시스템·플레이어; 마우스 감도/조작감 수동 확인 필요 |
| S3 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | UI·OFL 한글 폰트; UI 클릭/한글 렌더링 수동 확인 필요 |
| S4 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | ItemData 7종 |
| S5 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | 액자/칠판/읽기 패널 |
| S6 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | P1·보상·봉인 |
| S7 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | P2·재료 재생성·보상 |
| S8 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | P3·문·실제 Trigger/Cleared |
| S9 | 미착수 | 미실행; N/A; Console 미측정 | 없음 | 전체 탈출/재시작/Console 0 |

- TC-W01~W44 수동 결과는 ⏳ 유지. S10/S11 미실행.
- **CodeDrafts와 달라진 Runtime/Core 스크립트: 없음**(Assets/Scripts 미생성). 추가 소스는 Assets/Editor/ClassroomBuilder.cs와 Assets/Tests/PlayMode/ClassroomTests.cs뿐이며 CodeDrafts 복제 대상이 아니다.
- **큰 설계 판단: 없음.** 허용된 createProject→URP 경로 선택만 했고 정본 불변. Runtime asmdef 필요성 판단 전 중단.

---
# 네 번째 시도 — 2026-11-12 (KST)

## 최종 결과

**S1~S9 자동 구현/검증 완료. 세 퍼즐을 풀고 실제 물리 ExitTrigger로 Cleared 도달. 최종 PlayMode 9/9, 실패/skip 0, 게임 Console Error/Exception/Assert 0.**
수동 플레이를 수행했다는 뜻은 아니다. 감도·조작감·실제 UI 클릭·한글 렌더링은 수동 확인 필요. S10/S11은 미실행이다.

- 저장소: C:/Users/nonam/Documents/MagicEscape, main. 시작 git pull은 Already up to date, HEAD be76629. 시작 트리 깨끗함. Git Bash date: Thu Nov 12 00:32:07 2026. 일반 bash는 WSL /bin/bash 미설치로 실패해 지정 Git Bash로 재확인.
- AGENTS → UNITY_HANDOFF → SCENE_SETUP → PUZZLE_SPEC → TEST_PLAN 순서로 읽고 기존 보고/TASKS/코드를 확인. AGENTS.md 및 정답/itemId/상태/문구 변경 없음.
- Editor: C:/Program Files/Unity/Hub/Editor/6000.3.28f1/Editor/Unity.exe만 사용. -nographics 없음. URP 17.3.0/Input System 1.20.1/Test Framework 1.6.0 유지. XR/VR·유료 에셋 없음.
- URP Renderer/ClassroomURP를 Editor API로 생성하고 Graphics 기본 및 현재 Quality에 지정(나머지 Quality는 Graphics 기본 사용). Depth Texture/HDR ON, 카메라 Post Processing ON. S9에 명세의 기본 Volume/조명만 추가; 외형 에셋 교체/빌드는 하지 않음.
- 모든 씬/Prefab/ItemData/Material/Font/Volume/설정은 Unity API/공식 패키지 import로 생성했다. .unity/.prefab/.asset/.meta YAML 수기 작성 없음. Tests 폴더와 UnityStubs.cs 복사 없음.
- 로컬 Git 작성자 NoNamad5196 / 203311115+NoNamad5196@users.noreply.github.com 유지. 각 단계 builder → PlayMode → 오류 검사 → 기능 커밋 → push 완료. 강제 push 없음.

## 단계별 결과

아래 통과 수는 단계 당시의 **누적 테스트 수**다(최종 고유 테스트 9개). 모든 행의 builder/테스트 프로세스 exit=0, error CS=0, 예상하지 않은 게임 Error=0. 남은 자동 차단 결함은 없다. 각 행의 수동 항목은 아직 미실행이다.

| 단계 | 한 일 | 실제 검증 | 커밋(push 완료) | 수동 확인 / 남은 문제 |
|---|---|---|---|---|
| S1 | URP Asset/Renderer·Graphics/Quality 지정, 레이어·교실/복도/북문 구멍·기본 조명 | S1 빌더 + PlayMode 1/1; Console Error 0 | b40bbde | 그레이박스 외형 수동 확인 필요; 자동 차단 없음 |
| S2 | Core/Runtime 복사(Tests 제외), Runtime asmdef·시스템·EventSystem·Player/카메라 | S2 빌더 + PlayMode 2/2; Console Error 0 | 5f728ee | 마우스 감도·조작감 수동 확인 필요; 자동 차단 없음 |
| S3 | OFL Noto Sans KR 동적 TMP 4096, UI 6패널·버튼·핫바 8칸 | S3 빌더 + PlayMode 3/3; Console Error 0 | 36bc0ba | 실제 UI 클릭·한글 렌더링 수동 확인 필요; 자동 차단 없음 |
| S4 | 정본 ItemData 7종·카탈로그·색상 임시 아이콘 | S4 빌더 + PlayMode 4/4; Console Error 0 | 4ffaa50 | 아이콘 식별·이름 가독성 수동 확인 필요; 자동 차단 없음 |
| S5 | 액자 III/I/IV/II와 보이는 숫자/문양/장면·칠판·읽기 | S5 빌더 + PlayMode 5/5; Console Error 0 | bda1d18 | 액자/문양/칠판 가독성 수동 확인 필요; 자동 차단 없음 |
| S6 | P1 문양석6·발광·뚜껑·오답3회/힌트·푸른 수정·Seal1/P2 해금 | S6 빌더 + PlayMode 6/6; Console Error 0 | 9f2c71c | 발광/뚜껑/보상 연출 체감 수동 확인 필요; 자동 차단 없음 |
| S7 | P2 가마솥/마법서 결계/재료4·오답 원위치 재생성·보상2종·Seal2 | S7 빌더 + PlayMode 7/7; Console Error 0 | ea3ca54 | 색/연기/보상 피드백 체감 수동 확인 필요; 자동 차단 없음 |
| S8 | P3 비문/소켓 거부·소모/핵·문 코루틴·실제 Trigger/Cleared·재시작 | S8 빌더 + PlayMode 8/8; Console Error 0 | 0b471db | 문 회전/받침대 뒤 보행·비문 가독성 수동 확인 필요; 자동 차단 없음 |
| S9 | 실제 입력/Raycast로 퍼즐3 해결, W/물리로 탈출; 6개 Prefab·기본 Volume/조명·멱등 확인 | S9 빌더 + PlayMode 9/9; Console Error 0 | f3331b9 | 사람의 전 구간 플레이·UI/감도/한글·10~15분 평가 수동 확인 필요; 자동 차단 없음 |

## 실행 명령 및 증거

저장소 루트에서 N=1~9 각각 실제 실행했다. S9는 수정 후 여러 차례 재실행했고, 최종 동일 소스/구성으로 빌더를 연속 두 번 실행한 뒤 PlayMode를 다시 통과했다.

```powershell
git pull
& 'C:\Program Files\Git\bin\bash.exe' -c date
./Tools/Run-Unity.ps1 -Name sN-fourth-builder -Method MagicEscape.EditorTools.ClassroomBuilder.SN
./Tools/Run-Unity.ps1 -Name sN-fourth-tests -Tests
./Tools/Record-Stage.ps1 -Stage N -Description '<해당 단계 작업>' -Message '<기능 커밋>'
```

래퍼가 실행한 핵심 Unity 인자:

```console
-batchmode -projectPath <repo>/UnityProject -quit -executeMethod MagicEscape.EditorTools.ClassroomBuilder.SN -logFile <repo>/docs/validation/sN-fourth-builder.log
-batchmode -projectPath <repo>/UnityProject -runTests -testPlatform PlayMode -testResults <repo>/docs/validation/sN-fourth-tests.xml -logFile <repo>/docs/validation/sN-fourth-tests.log
```

- [S1 XML](s1-fourth-tests.xml) ~ [S9 XML](s9-fourth-tests.xml): 각 단계 누적 1/1~9/9, skip 0.
- [Console 계측](s9-console-audit.txt): Application.logMessageReceived에서 Error/Exception/Assert 이벤트 **0**. 예상 오류를 허용하는 LogAssert.Expect 사용 없음. NUnit 기본 unexpected Error 감시도 유지.
- [원시 문자열 검사 요약](fourth-attempt-log-scan.json): 성공한 각 builder/test의 error CS, 관리 예외 헤더, Debug.LogError/LogException 0. 단순 Exception/ Error 부분 문자열 수는 별도 기록했다.
- 로그 시작의 Licensing handshake/token Error 3줄은 이후 정상 라이선스 연결/exit 0과 구분했다. LogAssemblyErrors, ExecuteEnumerableAndRecordExceptions 같은 이름도 단순 문자열 검색에 걸리므로 게임 Console 이벤트 수로 치환하지 않았다.
- 원시 로그/실패 중간 XML은 UnityProject/Logs/fourth-attempt에 로컬 보존(식별자 포함 가능, gitignore). 성공 XML/계측/스캔 요약만 커밋했다.
- [멱등 검사](s9-idempotency.txt): 최종 S9 두 번 재구성 후 171개 오브젝트의 이름/계층·활성·위치·스케일·컴포넌트 타입 manifest SHA256 동일. [manifest](s9-scene-manifest.txt). Unity YAML 파일 ID/바이트 동일성을 의미하지 않는다. 에셋은 경로로 재사용한다.
- CodeDrafts/Core·Runtime의 모든 .cs/.asmdef를 Assets/Scripts의 상대 대응 파일과 SHA256 비교: 차이 0. UnityStubs.cs 없음 확인. 패키지 버전/AGENTS 불변 확인.

### TMP 사전 import 명령

ImportPackage가 완료되기 전에 Font API를 호출한 ENV-W05를 수정했다. quit 없이 importPackageCompleted 콜백이 exit 0으로 종료하게 실행한 뒤 S3를 재실행했다.

```console
Unity.exe -batchmode -projectPath <repo>/UnityProject -executeMethod MagicEscape.EditorTools.ClassroomBuilder.ImportTMP -logFile <repo>/UnityProject/Logs/fourth-attempt/import-tmp.log
```

폰트는 Google Fonts Noto Sans KR 원본 TTF/OFL 1.1을 Assets/Fonts에 보존했다. 출처/라이선스는 Assets/Fonts/README.md와 OFL.txt. authored TMP 전부 해당 폰트 적용, 글리프 커버리지 자동 검사 통과(실제 시각 렌더링은 미확인).

## 검증 범위와 해결 사항

- S1~S8은 씬을 로드한 뒤 실제 Runtime 컴포넌트/인벤토리/코루틴/물리 Trigger/UI OnClick을 검사했다. S9는 테스트 장치를 통해 ESC/Tab/E/숫자키/마우스 휠을 전달하고 중앙 Raycast 거리 2.5m를 검증했다. 퍼즐/아이템 상호작용은 실제 PlayerInteraction 경로를 탄다.
- 상호작용 장소 간 이동 및 조준 정렬은 테스트에서 결정론적으로 배치했다. **전 구간을 WASD로 돌아다닌 인간 플레이가 아니다.** 마지막 출구 우회 구간은 W 입력 → CharacterController 충돌 → 실제 Trigger → Cleared로 검증했다. S8/S3에서 클리어/일시정지 버튼 재시작과 초기화도 검증했다.
- 정상 Flow 로그까지 실패시킨 테스트 API 사용(TEST-W04), TMP 비동기 import(ENV-W05), batch Game View 입력 라우팅(TEST-W06), 중복 out 변수(TEST-W09)는 원인 확인 후 수정했다.
- 실제 Raycast가 찾은 GAME-W07: Cylinder 기본 CapsuleCollider가 시각 제단보다 높아 보상을 가렸다. 명세대로 BoxCollider(로컬 1,2,1)로 생성해 수정했다.
- 실제 보행이 찾은 GAME-W08: 받침대/비문 주변과 북벽 틈이 좁았다. **시각 크기·위치와 Player 규격은 유지**하고 받침대 충돌 hull 깊이만 0.2m/중심을 0.1m 앞쪽으로 inset했다(후면 통로 0.7m). 비문판/소켓컵/핵은 Player 접촉만 제외하고 Raycast는 유지했다. 테스트 waypoint 오차도 조정한 뒤 최종 9/9를 연속 두 번 통과했다. 자세한 결함/실패 수치는 TEST_PLAN §3에 기록했다.
- 빌드/실제 종료 버튼·일반 플레이어 평가·S10 외형 완성은 하지 않았다. 무음/기본 도형/단순 파티클의 그레이박스이며, VFX/빛/조작 체감 및 받침대 뒤쪽 보행은 수동 확인 필요.

## 파일·설계 인계

- 주요 생성물: Assets/Scenes/Classroom.unity, Assets/Settings, Assets/Data/Items(7종), Assets/Fonts, Assets/Prefabs(6종), Assets/Scripts/Core·Runtime, Assets/Editor/Classroom*Builder.cs, Assets/Tests/PlayMode.
- 실행 도구: Tools/Run-Unity.ps1(TestFilter 옵션 추가), Tools/Record-Stage.ps1. 현재 전체 테스트는 최종 S9 씬을 전제로 한다. 이전 단계만 다시 구성하면 해당 단계 테스트를 -TestFilter로 선택한다.
- 문서: TEST_PLAN 결과 칸/§3 결함, TASKS Windows 표, 이 보고서 갱신.
- **CodeDrafts와 달라진 .cs/.asmdef 목록: 없음.** Runtime asmdef 신설은 양쪽에 같은 내용으로 반영했다. Editor 빌더/Unity PlayMode 테스트는 CodeDrafts 복제 대상이 아니다.
- **큰 설계 판단: 없음.** 별도 PlayMode asmdef가 Assembly-CSharp를 참조할 수 없어 Runtime asmdef만 추가(Core/TMP/uGUI/Input System 참조). 정본/게임 설계 불변. 위 충돌 hull 조정은 실제 보행으로 확인한 그레이박스 구현 보정이다.
- 남은 항목: 마우스 감도·조작감·실제 UI 클릭·한글 렌더링·전체 인간 플레이. S10/S11은 별도 작업.
