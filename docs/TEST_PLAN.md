# MagicEscape — 테스트 계획 및 결과

상태 표기: ✅ 통과 · ❌ 실패 · ⏳ 미실행 · ❔ 미검증(실행 환경 없음)

## 1. 서버 자동 테스트 (Instabox, dotnet 8 SDK)

| ID | 대상 | 명령 | 결과 (2026-11-10) |
|---|---|---|---|
| TS-01 | Core 규칙 34건 | `~/.dotnet/dotnet test Tests/MagicEscape.Core.Tests` | ✅ `Passed! - Failed: 0, Passed: 34, Skipped: 0, Total: 34` |
| TS-02 | Runtime 스텁 컴파일(새 Input System) | `~/.dotnet/dotnet build Tests/RuntimeCompileCheck -p:InputBackend=New` | ✅ 0 Error, 0 Warning |
| TS-03 | Runtime 스텁 컴파일(구 Input Manager) | `~/.dotnet/dotnet build Tests/RuntimeCompileCheck -p:InputBackend=Legacy` | ✅ 0 Error, 0 Warning |

TS-01이 다루는 항목:
- P1: 비활성 무시, 정답 해결, 오답 리셋·실패 카운트, 오답 후 처음부터, 3회 힌트, 정답 정본 값, 잘못된 생성자 인자
- P2: 잠금 시 NotActive·소모 없음, 정답 해결, 비재료 소모 없음, 순서 오류 시 LastFailedBatch 전체, 독버섯 즉시 오답, 2회 힌트 후 재도전 성공
- P3: Dormant 응답, 비문은 물약만, 해독 전 소켓 불가, 틀린 소켓 거부·소모 없음, 비수정 InvalidItem, 전체 해제, 2회 힌트
- 진행: 초기 상태, 순서 위반 무시, 해금 순서와 이벤트 순서, 중복 해결 무시, 출구 열림+Playing일 때만 탈출, 흐름 전이 가드, Playing 중에만 시간 누적
- 인벤토리: 용량, null/빈 문자열, 선택, 마지막 선택 아이템 제거, 휠 순환, Changed 이벤트
- 전체 탈출 시나리오(퍼즐 3종 + 인벤토리 + 진행)

한계: Unity 물리·렌더링·UI·입력·코루틴 타이밍은 서버에서 검증하지 않았다(❔).

## 2. Windows 수동 테스트 (Unity Play Mode / 빌드)

각 케이스: 전제 → 조작 → 기대 결과. 결과 칸은 Windows 작업자가 채운다.

### 기본 조작·UI
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W01 | Play | 타이틀 패널, 커서 보임, 이동 불가 | 자동: Title/패널 확인; 커서·체감 수동 ⏳ |
| TC-W02 | 시작 클릭 | HUD 표시, 커서 잠김, WASD·마우스 동작, 벽 통과 불가 | 자동: 시작 OnClick·이동/벽 충돌; 실제 클릭·마우스 체감 수동 ⏳ |
| TC-W03 | ESC → ESC | 일시정지(시간 정지, 커서 보임) → 재개 | 자동 ✅ ESC 입력·Pause/Resume·timeScale; 커서 체감 수동 ⏳ |
| TC-W04 | 액자 조준 후 E, 다시 E | 읽기 패널 열림(이동 잠금) → 닫힘 | 자동 ✅ 중앙 Raycast/E 읽기·닫기/조작 잠금; 표시 수동 ⏳ |
| TC-W05 | Tab | 목표 "중앙 제단의 마법 문양을…" 표시/숨김 | 자동 ✅ Tab 토글·목표 문구; 표시 수동 ⏳ |
| TC-W06 | 재료 줍기 후 1~8, 휠 | 핫바 아이콘·선택 테두리·이름 변경 | 자동 ✅ 숫자키/합성 휠·아이콘/선택/이름 모델; 표시 수동 ⏳ |

### 퍼즐 1
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W10 | 대지 → 물 → 바람 | 2개 청색 후 전체 빨간 점멸·소등, 실패 토스트 | 자동 ✅ 오답 진행 리셋; 색/효과음 체감 수동 ⏳ |
| TC-W11 | 오답 3회 | 토스트·Tab에 "벽에 걸린 액자의 숫자 순서를…" | 자동 ✅ 3회 실패·정본 힌트; Tab 표시 수동 ⏳ |
| TC-W12 | 대지→물→불→빛 | 금색, 뚜껑 열림, 푸른 수정 등장, 봉인 빛 1 소등 | 자동 ✅ 정답/보상 등장/해금/Seal1; 발광·뚜껑 연출 수동 ⏳ |
| TC-W13 | 해결 후 문양석 조준 | 프롬프트 없음 | 자동 ✅ 해결 후 GetPrompt=null; 실제 조준 표시 수동 ⏳ |
| TC-W14 | 푸른 수정 줍기 | 핫바에 푸른 수정 | 자동 ✅ Raycast/E 획득·인벤토리; 핫바 표시 수동 ⏳ |

### 퍼즐 2
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W20 | P1 전 마법서 E | 결계 메시지, 결계 보임 | 자동 ✅ 마법서 잠금/결계; 표시/문구 시각 수동 ⏳ |
| TC-W21 | P1 전 가마솥에 재료 선택 E | "가마솥이 차갑다…", 재료 유지 | 자동 ✅ 차갑다는 문구/소모 없음/실패 미증가; 수동 ⏳ |
| TC-W22 | P1 후 마법서 | 결계 사라짐, 조합법 패널 | 자동 ✅ 결계 해제·조합법 읽기; 수동 ⏳ |
| TC-W23 | 푸른 수정 선택 후 가마솥 | "이건 넣을 수 없다.", 수정 유지 | 자동 ✅ 비재료 유지/실패 미증가; 수동 문구 표시 ⏳ |
| TC-W24 | 꽃잎 → 가루 | 검은 연기, 꽃잎·가루가 원위치에 재등장 | 자동 ✅ 꽃잎→가루 오답/원래 위치 재생성; 연기 체감 수동 ⏳ |
| TC-W25 | 독버섯 | 오답, 독버섯 원위치, 2회째부터 힌트 | 자동 ✅ 독버섯 재생성·2회 힌트; 표시 수동 ⏳ |
| TC-W26 | 꽃잎→비늘→가루 | 색 변화 3단계, 금색, 물약·붉은 수정 획득, 봉인 빛 2 소등 | 자동 ✅ E 정답/보상2종/Seal2/P3 해금; 색·연출 수동 ⏳ |

### 퍼즐 3
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W30 | P2 전 봉인 장치 E | "아직 봉인이 두 개 남아 있다." | 자동 ✅ Dormant/고정 문구; 표시 수동 ⏳ |
| TC-W31 | P2 후 수정으로 비문 | "비문이 흐릿해 읽을 수 없다." | 자동 ✅ 잘못된 아이템/흐릿한 비문 문구; 수동 ⏳ |
| TC-W32 | 해독 전 소켓 | "소켓이 무엇을 원하는지…" | 자동 ✅ 해독 전 소켓 진행 안 됨; 문구 표시 수동 ⏳ |
| TC-W33 | 물약으로 비문 | 비문 등장, 읽기 패널, 물약 사라짐 | 자동 ✅ 물약 소모/해독 상태/읽기; 문양 렌더링 수동 ⏳ |
| TC-W34 | 왼쪽에 붉은 수정 | 거부, 빨간 점멸, 수정 유지 | 자동 ✅ 거부2회/수정 유지/힌트; 발광 수동 ⏳ |
| TC-W35 | 봉인핵(수정 전) | "봉인핵이 아직 잠잠하다…" | 자동 ✅ 준비 전 봉인핵 문구; 표시 수동 ⏳ |
| TC-W36 | 왼쪽 푸른·오른쪽 붉은 → 봉인핵 | 봉인 빛 3 소등, 약 1초 후 문 2초간 열림 | 자동 ✅ Released/Seal3/문 코루틴/Trigger 지연 활성; 외형 수동 ⏳ |
| TC-W37 | 문 밖 이동 | 클리어 화면, 탈출 시간 mm:ss | 자동 ✅ W 이동/실제 Trigger/Cleared/패널·시간 형식; 수동 ⏳ |

### 재시작·빌드·안정성
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W40 | 클리어에서 다시 시작 | 타이틀, 모든 퍼즐·인벤토리·봉인 빛 초기화 | 자동 ✅ 클리어 버튼 OnClick→재로드/Title/인벤토리/P1/P3 초기화; 전체 시각 수동 ⏳ |
| TC-W41 | 일시정지에서 다시 시작 | 동일, timeScale 정상 | 자동 ✅ 일시정지 버튼 OnClick→재로드/timeScale=1/Title; 실제 클릭 수동 ⏳ |
| TC-W42 | Windows 빌드 exe 전체 클리어 + 종료 | 정상 동작, 종료 시 앱 닫힘 | 범위 밖(S11), ⏳ 미실행 |
| TC-W43 | 전체 플레이 동안 Console | Error 0건 | ✅ 자동 PlayMode Error/Exception/Assert 0; 사람이 전체 플레이한 결과 아님 |
| TC-W44 | 일반 플레이어(처음 보는 사람) 1명 | 10~15분 내 클리어, 막힌 지점 기록 | ⏳ 일반 플레이어 수동 평가 미실행 |

## 3. 결함 기록

| 날짜 | ID | 현상 | 원인 | 조치 | 커밋 |
|---|---|---|---|---|---|
| 2026-11-11 | ENV-W01 | Unity 6000.3.28f1 설치 실패(exit 1) | Windows 승격 취소 또는 시간 초과(1223 / ELEVATION_CANCELLED) | S1 이전 중단. 관리자 승인 후 설치 재개 필요. [오류 전문](validation/windows-unity-report.md) | 본 기록 커밋 |
| 2026-11-12 | ENV-W02 | S1 createProject 중 Editor 종료(로그 return code 198) | 유효한 Unity Editor 라이선스 없음; Error 9줄(환경) | S1 중단. Hub 라이선스 활성화 후 재개. PlayMode 미실행, 게임 Console 미측정. [오류 전문](validation/windows-unity-report.md), [로그](validation/s1-create.log) | 본 기록 커밋 |
| 2026-11-12 | ENV-W03 | 세 번째 시도: 프로젝트 생성 exit 0, S1 빌더 이전 컴파일 exit 1 | Input System 1.12.0 InputSystemPluginControl.cs(47,25) CS0117: BuildTarget.ReservedCFE 없음 | S1 중단, 호환 패키지 확인 필요. PlayMode 0개 실행/게임 Console 미측정. [오류 전문](validation/windows-unity-report.md), [로그](validation/s1-builder.log) | 본 증거 커밋 |
| 2026-11-12 | ENV-W03 해결 | 위와 동일 | manifest.json에 6000.0 시절 버전(URP 17.0.1, Input System 1.12.0, Test Framework 1.4.2)이 적혀 있었음. 6000.3.28f1 에디터 manifest의 minimumVersion은 URP 17.3.0, Input System 1.20.1, Test Framework 1.6.0 | 세 패키지를 에디터 최소 버전으로 올리고 packages-lock 재생성. batchmode 프로젝트 열기 exit 0, `error CS` 0건. URP 전역 설정 자동 생성 Warning 1건(오류 아님). S1 빌더·PlayMode는 아직 미실행 | 본 수정 커밋 |


### 2026-11-11 Windows 실행 범위

Unity 설치 실패로 TC-W01~W44 및 Unity PlayMode 테스트는 전부 미실행이다.
기존 ⏳ 결과를 유지하며, 게임 테스트 ❌ 또는 Console Error 0으로 간주하지 않는다.
설치 시도 버전은 6000.3.28f1이며 실제 설치 완료 패치 버전은 없다.
마우스 감도·시점 감각·UI 클릭감·한글 렌더링은 자동 검증 범위 밖으로 수동 확인 필요(현재 미실행).


### 2026-11-12 Windows 재개 실행 범위

6000.3.28f1 설치/Windows Standalone Support 폴더는 확인했으나 S1 프로젝트 생성이 라이선스 오류로 종료됐다. S1~S9 미완료, 빌더/PlayMode 미실행(0개), 게임 Console Error 수 미측정. TC-W01~W44 결과는 ⏳ 유지(환경 실패를 게임 TC 실패로 기록하지 않음). 마우스 감도·조작감·UI 클릭·한글 렌더링은 수동 확인 필요. S10/S11 미실행.


### 2026-11-12 세 번째 시도 결과

S1 createProject exit 0, S1 builder exit 1(Input System 패키지 CS0117 1종). `error CS` 3줄은 동일 오류 중복 출력, `Exception` 1줄은 파일명 ExceptionHelpers.cs, `Error` 9줄은 환경/컴파일 요약 포함. 게임 Console 이벤트 수는 미측정이며 0 Error 불충족. PlayMode 테스트 소스 1건 작성, 실행 0건, 통과/전체 N/A. 컴파일 실패에서 중단해 runTests 명령은 실행하지 않았다. TC-W01~W44는 실제 수동 검증하지 않아 전부 ⏳ 유지. S1~S9 미완료, S10/S11 범위 밖. 마우스 감도·조작감·UI 클릭·한글 렌더링 수동 확인 필요.


S1 자동 검증: URP Asset/Renderer·레이어·교실/복도/북문 구멍·조명 저장. PlayMode 1/1, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.
`n| 2026-11-12 | TEST-W04 | S2 1/2: NoUnexpectedReceived가 정상 Flow 로그까지 실패 처리 | 테스트 API를 모든 로그 무발생 검증으로 사용한 오류 | 해당 호출 제거, Test Framework 기본 unexpected Error/Exception 실패 감시는 유지; 재실행 | 5f728ee |

S2 자동 검증: Core/Runtime 복사·Runtime asmdef·시스템·Player·이동 벽 충돌/일시정지. PlayMode 2/2, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.


S3 자동 검증: uGUI/TMP·OFL Noto Sans KR·6패널·버튼·8슬롯·읽기/목표/일시정지 재시작. PlayMode 3/3, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.

S4 자동 검증: ItemData 7종·정본 itemId·카탈로그·색상 아이콘·슬롯/휠 모델 반영. PlayMode 4/4, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.

S5 자동 검증: 액자 III/I/IV/II·보이는 문양/장면/숫자·칠판·읽기 패널. PlayMode 5/5, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.

S6 자동 검증: P1 문양석6·발광·뚜껑·숨김 보상·오답3회 힌트·P2 해금·Seal1. PlayMode 6/6, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.

S7 자동 검증: P2 가마솥·마법서 결계·재료4·오답 재생성/힌트·선택 아이템 소모/보상·Seal2. PlayMode 7/7, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.

S8 자동 검증: P3 비문/소켓 거부/소모·봉인핵·문 코루틴·실제 Trigger/Cleared·클리어 재시작. PlayMode 8/8, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.


### 네 번째 시도 — 자동/수동 구분 및 해결 결함

S1~S8 단계별 누적 1/1~8/8 통과. S9는 9/9, 실패/skip 0, 실제 Error/Exception/Assert 이벤트 0.
자동 테스트 결과를 위 결과 칸에 세분화했다. 사람의 마우스 감도·조작감·실제 버튼 클릭·한글 렌더링은 **미실행/수동 확인 필요**다.
S9 입력 시나리오는 테스트용 키보드/마우스로 ESC/Tab/E/숫자키/휠을 보낸다. 상호작용 장소 사이 이동·시점 정렬은 테스트에서 배치하고, 상호작용은 거리 2.5m 중앙 Raycast를 통과한다. 마지막 출구 구간은 실제 W 입력과 CharacterController 충돌/ExitTrigger로 Cleared에 도달했다. 전 구간을 사람이 걸어 플레이한 것으로 쓰지 않는다.
원문 XML 및 반복 빌더 동일성/Console 계측: validation/s9-fourth-tests.xml, s9-idempotency.txt, s9-console-audit.txt.

| 날짜 | ID | 현상 | 원인 | 조치 | 커밋 |
|---|---|---|---|---|---|
| 2026-11-12 | ENV-W05 | S3 TMP_FontAsset 생성 NullReferenceException | TMP_Settings 미설치: ImportPackage 비동기 완료 전에 Font API를 호출함 | importPackageCompleted 후 종료하는 별도 ImportTMP 실행, S3 재실행 | 36bc0ba |
| 2026-11-12 | TEST-W06 | S9 합성 ESC가 Playing을 유지(8/9) | 비활성 batch Game View의 기본 입력 라우팅이 키보드 이벤트를 편집기로 보냄; 패키지 InputSettings/InputManager 소스 확인 | 테스트 동안만 AllDeviceInputAlwaysGoesToGameView/IgnoreFocus 설정 후 복구. 게임 코드/입력 설계 변경 없음. 이후 실제 E로 P1 해결 도달 | f3331b9 |
| 2026-11-12 | GAME-W07 | 실제 Raycast에서 보상 수정을 AltarBase가 가림(8/9) | Unity Cylinder 기본 CapsuleCollider가 xz 최대 스케일에 맞춰 높이를 늘림: 시각 제단 상면 1.0m보다 콜라이더가 높음 | Cylinder 그레이박스 생성 시 명세의 BoxCollider로 교체(로컬 size 1,2,1). 보이는 치수/보상/정답 불변 | f3331b9 |
| 2026-11-12 | GAME-W08 | 전체 E 풀이 후 출구 우회 보행 막힘(8/9) | 받침대/비문/상부 소켓이 북벽과 0.45m 틈을 만들어 지름 0.6m+skin의 CharacterController가 못 지남 | 시각 크기/위치·Player 규격 유지. 받침대 hull 깊이 0.2m, 중심 z를 0.1m 앞쪽으로 inset해 뒤 0.7m 통로 확보. 비문/소켓컵/핵은 Player 접촉만 제외(중앙 Raycast 유지). 보행 waypoint 허용 오차도 0.09→0.035m로 보정; 전체 입력 재검증 | f3331b9 |
| 2026-11-12 | TEST-W09 | S9 글리프 검사 추가 후 CS0128 | 같은 테스트 범위에 out 변수 missing을 두 번 선언 | out discard로 수정, 빌더/전체 테스트 재실행 | f3331b9 |

S9 자동 검증: 합성 키보드/휠·중앙 Raycast E로 세 퍼즐·W 물리 탈출 Cleared·Console 계측0·멱등 재실행. PlayMode 9/9, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.


### 출구 통로 재개 중단 — 2026-11-12 KST
- 기존 출구 확장/봉인 재배치/직진 3경로 테스트 변경 보존. 임시 InitTestScene 정리 완료.
- exit-tests-resumed: Unity TestRunner 시작 OOM 및 native crash, exit -536870911, 신규 XML 없음. 이번 확정 통과 0, 게임 Console Error 미측정. 원인 불명 오류 중단 규칙 적용.
- 1부 미검증·미커밋·미push. S10/S11 미착수. [인계 보고](validation/exit-resume-report.md), [오류 전문](validation/exit-resume-crash.md).

### 출구 통로 재검증 완료 — 2026-11-12 KST
- ENV-W10: 지난 OutOfMemoryException은 코드/Unity 결함이 아니라 Windows 커밋 메모리 부족(페이지 파일 꺼짐, 한도 47.9GB, 직전 여유 3.3GB)으로 확인됨(인계 정보). Comet 종료 후 재실행으로 해결. OS/페이지 파일 설정은 변경하지 않았다.
- 매 실행 전 FreeVirtualMemory/1MB 확인: 빌더 9.38GB, 테스트 9.45GB(6GB 이상). Editor 한 개씩 실행, 종료 후 잔류 Editor 없음 확인.
- `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name exit-build-verified -Method MagicEscape.EditorTools.ClassroomBuilder.S9`: exit 0.
- `powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name exit-tests-verified -Tests`: exit 0, PlayMode **12/12**, failed/skipped 0. s9-console-audit.txt: Error/Exception/Assert **0**. W-only 중심/좌/우 3건 포함. E 시나리오로 비문/소켓 2.5m Raycast 확인.
- 최초 빌더 호출은 잘못된 executeMethod 이름으로 exit 1; 올바른 namespace/메서드로 수정 재호출했다. 게임 코드 결함 아님.
- 로그의 초기 Hub handshake 오류 뒤 Editor client 연결/entitlement 확인, 빌더/테스트 정상 종료. 라이선스/세션 원본 로그는 gitignore로 제외했다.
- 외형, 마우스 체감, 실제 UI 클릭은 수동 확인 필요. 이전 중단 기록은 당시 결과이며 본 재검증으로 갱신됨.

### S10 자동 검증 및 한계 — 2026-11-12 KST
- ClassroomArtBuilder.S10: CC0 시각 모델/아틀라스, 자체 제작 가마솥 메시·소프트 파티클, 조명/Volume, 모든 SfxId 연결. 기존 gameplay 스크립트/콜라이더/레이어/참조를 바꾸지 않음(AudioManager.clips의 명시적 연결 제외). 변경 Editor/테스트 소스는 CodeDrafts에 복사.
- 최초 s10-tests: 11/13. batchmode의 ScreenCapture.CaptureScreenshot이 파일을 쓰지 않아 캡처 테스트 실패. Screen.SetResolution을 제거하고 실제 Camera.main+HUD를 RenderTexture에 1280×720으로 렌더하여 저장하도록 변경. SceneView 캡처나 사람의 PlayMode 시연이 아니다.
- 같은 최초 실행에서 기존 S9 보행이 z=4.32에서 5초 제한 실패 1회. 단독 s10-walk-diagnostic은 1/1, 이후 s10-tests-final·s10-tests-release 전체는 각각 13/13. 게임 콜라이더나 테스트 시간/거리 허용값은 완화하지 않았고 보행 frame/실시간/시뮬레이션 시간/문 각도 계측만 추가했다. 최초 보행 실패의 독립 근본 원인은 확정 못 했으므로 재발 관찰 항목으로 남긴다.
- 캡처 시각 QA로 비문 텍스트 overflow, 가마솥 외형, 사각 파티클을 확인해 자동 크기 맞춤·자체 메시·알파 텍스처로 보정. 텍스트 내용/정답/itemId 불변.
- 수동 확인 필요: 전체 외형·밝기·폰트 크기/가독성·음색/음량·실제 GameView 표시/마우스/UI 클릭/보행 체감. 선택 Asset Store 원본 미설치 상태는 정상 경로이나 CC0 폴더 전체 제거 후 재실행은 별도 미검증. S11 빌드 미실행.

### S10 최종 씬 검증
- `s10-final-build`와 `s10-final-repeat-build`: 모두 exit 0. scene-manifest SHA256 동일, collider 불변 audit 통과(전체 YAML의 바이트 동일성을 주장하지 않음).
- `s10-tests-verified`: 전체 13/13, failed/skipped 0, game Error/Exception/Assert 0. 마지막 캡처 타이밍 변경 후 결과는 validation/s10/report.md 및 results.json에 기록.
- Runtime/Core 코드는 변경하지 않았고 패키지 버전/manifest도 변경하지 않았다. S11은 미실행.
- ART-W11: 캡처 시 문양 발광 누락 발견. URP material importer가 검정 직렬화 EmissionColor의 _EMISSION 키워드를 제거했다. S10 Reactive 머티리얼의 미세 비검정 기본 발광/Emissive GI 플래그로 보정(GlowFeedback Runtime 코드는 유지). 저장 후 재임포트한 모든 GlowFeedback 렌더러에 _EMISSION이 남는지 ArtTests에서 검사한다. 최종 명령/결과는 validation/s10/report.md 및 results.json.
- 최종 `s10-tests-final-accepted`: PlayMode **13/13**, failed/skipped 0, game Error/Exception/Assert **0**. `s10-readable-build`/`s10-readable-repeat-build` 각각 exit 0/구조 동일. Emission 실제 발광 및 비문 clipping 보정 후 재검증 결과. 원본 로그·XML 대신 식별정보를 제외한 validation/s10/results.json과 console-audit.txt를 커밋.


### S10b ?? ?? ? 2026-11-12 KST
- S10b ?? ??: ??/?? ?????, CC0 ??+?/???/?? ??, ?? ????/???/???, ?? ?? ?????/? ?, ????/? ?????? HUD ??. Runtime/Core/??/itemId/GameText ?? ??. CodeDrafts ???.
- ?? `s10b-accepted-build`/`s10b-accepted-repeat` exit 0, ?? SHA256 ??. ????/???/?? ?????? ? ?? ???? ?????? ?? audit ??, 3m ?? ??.
- `s10b-tests-accepted`: PlayMode **13/13**, failed/skipped **0**, exit **0**, ?? Error/Exception/Assert **0**. ?? ?? `s10b-tests`? 13/13. Emission ?????? glyph bounds/?? ? ??/HUD ??????? glyph ?? ??. ??? ??? ?? ??.
- ?? Camera.main+HUD PNG **7?**, 1280?720, `validation/s10b/`. Unity ?? ??? 6GB ??? ? ?? Editor ??? ??.
- **?? ??? ?? ?? ??**: ?? ??????2.5m ???/??????? ?????? ??/??. ?? ??/??/??? ??? ?? ?? ??. S11 ???.
- ????? ?????/????? ??: [S10b ??](validation/s10b/report.md), [?? ??](validation/s10b/results.json).

### S10c 자동 검증 및 직접 시각 점검 — 2026-11-12 KST

- `ClassroomBuilder.S10c`를 두 번 재생성: 두 번 exit 0, scene-manifest hash 동일. S10b 대비 collider serialization/월드 위치·스케일/레이어 및 기존 gameplay MonoBehaviour 설정·참조 불변 검사 통과. 장식에 신규 collider 없음.
- `s10c-tests-release` 및 감사 경로 정리 후 `s10c-tests-final`: 각각 **13/13**, failed/skipped 0, game Error/Exception/Assert **0**. [기계 판독 결과](validation/s10c/results.json). 기존 13개 테스트의 범위를 유지하면서 S10c 미술 검사를 확장했다.
- 추가 검사: near-black ambient/Exponential Fog, shadowed light 2개 제한, 새 shader 지원 여부, CC0 ambience clip/loop 실제 재생 상태, 봉인 고리 회전, 256px 아이콘 연결, 칠판 원문·NoWrap·bounds. 기존 비문 glyph/아이콘 겹침/Emission 보존 검사 유지.
- 실제 MainCamera+HUD PNG 14장: S10b와 동일 01~07, 랜턴만 켠 복도 08, 달빛 창문 09, 액자/칠판/공개 비문 10~12, 준비된 수정 색 13, 전 아이콘 14. 각 이미지를 직접 열어 확인했다.

| 시각 확인 | 결과 / 근거 |
|---|---|
| 액자 로마숫자 | III/I/IV/II 구분 가능(02), 근접에서 선명(10) |
| 칠판 | 원문 전체 읽힘, ‘순서로’·‘액자의’ 내부 끊김 없음(11) |
| 공개 비문 | 두 줄 문장 전체 읽힘, 물/불 문양과 겹침 없음(05/12) |
| 수정 | 왼쪽 파랑/오른쪽 빨강 명확(13), 핫바 색/형태 구분(14) |
| 봉인 받침대 | 우하단 핫바가 중앙 받침대를 가리지 않음(05/13) |

- 캡처 격리: 08은 다른 Light/ambient만 일시적으로 끄고 복구한다. ExitTrigger에 의해 클리어 UI가 덮이지 않도록 그 캡처 동안만 trigger를 비활성화했다가 원위치에서 복구한다. 전체 입력/출구 3경로 실제 W 테스트는 그대로다.
- 중간 CS0200(읽기 전용 URP 그림자 해상도 프로퍼티)은 SerializedObject API로 수정했고, 그림자 atlas 자동 축소 경고는 Medium tier로 해결했다. 잘린 수정 아이콘/회색 병/촛불 겹침/뒤집힌 복도 유리는 시각 점검 후 수정·재촬영했다. 최종 컴파일/실행에 오류 없음.
- 실행마다 6GB 메모리 및 Editor 단일 실행 게이트 통과. 원본 로그/XML의 환경·라이선스 식별정보는 커밋하지 않고 sanitized results/console/builder/lighting/idempotency audit만 보관한다.
- **미검증 유지:** 실제 프로젝터, 일반 노트북 60fps, 실제 음향 청취·마우스 조작감·10~15분 사용자 플레이, S11 실행 파일 빌드. 자동 isPlaying 검사는 사람이 소리를 들은 검증이 아니다. [전체 보고/명령](validation/s10c/report.md).


## S10d ? ?? ?? ??? 4? ?? (2026-11-12)

- [x] ???? ?? UI/?? + ????? ? ??, OFL 1.1 ??/TTF ??????. Static TMP atlas: ???? 518? / ?? 23?, ?? 0. ??? GameText ? ??? ?? ??.
- [x] ??? 4?/?? ??/? UI/?? ??/?? ???, ?? 2? ??. III/I/IV/II? Core/GameText ?? ??.
- [x] ?? 33?72?, ??? ??/?????, ?????????????????. ?/?? ???Fresnel ????? ???, ??/?? ???.
- [x] ?? ?? exit 0; ?? ????/?? ??. PlayMode **13/13**, Console Error/Exception/Assert 0, W-only ?? ??/?/? ?? ??. ?? ??/??? ??? ??? 18.23/17.64GB, Editor ?? ??.
- [x] s10c? ?? ??? 01~14, ??? ??? 1~5 ??? ?? ??. ??: [results](validation/s10d/results.json), [?? ?????](validation/s10d/REVIEW.md), [????](validation/s10d/).
- [x] Runtime/UI/UIManager.cs? ? ??? ???? CodeDrafts ???. FourthArtBuilder/ArtTests/FullInputTests/UITests? ?? ???.
- [ ] ?? ???? ?????? / ???? ?? ?? / ?? ??? ?? ??. ??? ?????? ?? ??? ??? ??? ?? ???? ?? ??? ???? ??.
- ?? 12/13 ? ?(? ?? ???, Dynamic atlas ???)? TMP ??? API ??? ??? ????. ?? ??? ??? ??? results/REVIEW? ??. Editor ???? token ???? ?? Console ??? ????.
