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
| TC-W01 | Play | 타이틀 패널, 커서 보임, 이동 불가 | ⏳ |
| TC-W02 | 시작 클릭 | HUD 표시, 커서 잠김, WASD·마우스 동작, 벽 통과 불가 | ⏳ |
| TC-W03 | ESC → ESC | 일시정지(시간 정지, 커서 보임) → 재개 | ⏳ |
| TC-W04 | 액자 조준 후 E, 다시 E | 읽기 패널 열림(이동 잠금) → 닫힘 | ⏳ |
| TC-W05 | Tab | 목표 "중앙 제단의 마법 문양을…" 표시/숨김 | ⏳ |
| TC-W06 | 재료 줍기 후 1~8, 휠 | 핫바 아이콘·선택 테두리·이름 변경 | ⏳ |

### 퍼즐 1
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W10 | 대지 → 물 → 바람 | 2개 청색 후 전체 빨간 점멸·소등, 실패 토스트 | ⏳ |
| TC-W11 | 오답 3회 | 토스트·Tab에 "벽에 걸린 액자의 숫자 순서를…" | ⏳ |
| TC-W12 | 대지→물→불→빛 | 금색, 뚜껑 열림, 푸른 수정 등장, 봉인 빛 1 소등 | ⏳ |
| TC-W13 | 해결 후 문양석 조준 | 프롬프트 없음 | ⏳ |
| TC-W14 | 푸른 수정 줍기 | 핫바에 푸른 수정 | ⏳ |

### 퍼즐 2
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W20 | P1 전 마법서 E | 결계 메시지, 결계 보임 | ⏳ |
| TC-W21 | P1 전 가마솥에 재료 선택 E | "가마솥이 차갑다…", 재료 유지 | ⏳ |
| TC-W22 | P1 후 마법서 | 결계 사라짐, 조합법 패널 | ⏳ |
| TC-W23 | 푸른 수정 선택 후 가마솥 | "이건 넣을 수 없다.", 수정 유지 | ⏳ |
| TC-W24 | 꽃잎 → 가루 | 검은 연기, 꽃잎·가루가 원위치에 재등장 | ⏳ |
| TC-W25 | 독버섯 | 오답, 독버섯 원위치, 2회째부터 힌트 | ⏳ |
| TC-W26 | 꽃잎→비늘→가루 | 색 변화 3단계, 금색, 물약·붉은 수정 획득, 봉인 빛 2 소등 | ⏳ |

### 퍼즐 3
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W30 | P2 전 봉인 장치 E | "아직 봉인이 두 개 남아 있다." | ⏳ |
| TC-W31 | P2 후 수정으로 비문 | "비문이 흐릿해 읽을 수 없다." | ⏳ |
| TC-W32 | 해독 전 소켓 | "소켓이 무엇을 원하는지…" | ⏳ |
| TC-W33 | 물약으로 비문 | 비문 등장, 읽기 패널, 물약 사라짐 | ⏳ |
| TC-W34 | 왼쪽에 붉은 수정 | 거부, 빨간 점멸, 수정 유지 | ⏳ |
| TC-W35 | 봉인핵(수정 전) | "봉인핵이 아직 잠잠하다…" | ⏳ |
| TC-W36 | 왼쪽 푸른·오른쪽 붉은 → 봉인핵 | 봉인 빛 3 소등, 약 1초 후 문 2초간 열림 | ⏳ |
| TC-W37 | 문 밖 이동 | 클리어 화면, 탈출 시간 mm:ss | ⏳ |

### 재시작·빌드·안정성
| ID | 조작 | 기대 결과 | 결과 |
|---|---|---|---|
| TC-W40 | 클리어에서 다시 시작 | 타이틀, 모든 퍼즐·인벤토리·봉인 빛 초기화 | ⏳ |
| TC-W41 | 일시정지에서 다시 시작 | 동일, timeScale 정상 | ⏳ |
| TC-W42 | Windows 빌드 exe 전체 클리어 + 종료 | 정상 동작, 종료 시 앱 닫힘 | ⏳ |
| TC-W43 | 전체 플레이 동안 Console | Error 0건 | ⏳ |
| TC-W44 | 일반 플레이어(처음 보는 사람) 1명 | 10~15분 내 클리어, 막힌 지점 기록 | ⏳ |

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
`n| 2026-11-12 | TEST-W04 | S2 1/2: NoUnexpectedReceived가 정상 Flow 로그까지 실패 처리 | 테스트 API를 모든 로그 무발생 검증으로 사용한 오류 | 해당 호출 제거, Test Framework 기본 unexpected Error/Exception 실패 감시는 유지; 재실행 | S2 수정 커밋 |

S2 자동 검증: Core/Runtime 복사·Runtime asmdef·시스템·Player·이동 벽 충돌/일시정지. PlayMode 2/2, game Console Error 0. 명령/범위는 windows-unity-report의 네 번째 시도 참조. 수동 TC는 별도 미실행.
