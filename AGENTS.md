# MagicEscape — 에이전트 작업 정책 및 설계 정본

## 1. 프로젝트

- 이름: MagicEscape — 마법학교 3D 1인칭 방탈출 (대학 '멀티미디어와 가상현실' 기말 작품, 제출물 = 게임 + PPT)
- Windows 작업 경로: `C:/Users/nonam/Documents/MagicEscape`; Linux 보조 경로: `/home/box/projects/MagicSchoolEscape` (폴더명 ≠ 저장소명 주의)
- 원격: `https://github.com/NoNamad5196/MagicEscape.git`, 브랜치 `main`
- Notion: 💡 아이디어 일지(`collection://4a1725c4-940c-470a-b663-d0e89ff7ef4f`)의 `MagicEscape — 마법학교 3D 방탈출 게임` 항목이 허브 — https://app.notion.com/p/4147cdef782d81da9895f389197b0300 (하위 문서 5개: 퍼즐 명세 / 시스템·C# / 씬·Unity 가이드 / 에셋·비주얼 / 개발·테스트 기록)

## 2. 환경 제약 (절대)

- Instabox 헤드리스 Linux 서버에서는 Unity Editor/Hub 설치·실행·빌드·Play Mode 금지. Windows 작업 환경에서는 Unity Editor batchmode 씬 생성과 EditMode/PlayMode 검증을 수행한다.
- `.unity`, `.prefab`, `.meta`, `.asset`(ScriptableObject 인스턴스) YAML 직접 작성 금지. `.asmdef`(JSON)만 허용.
- 실제 실행한 검증만 보고한다. Windows Unity 결과는 로그/XML로 남기며, 실제 HMD 미검증과 플레이어 빌드 미실행을 구분한다.
- 순수 C# 로직(`CodeDrafts/Core`)만 서버에서 `dotnet test`로 검증한다.
- 씬 제작·Prefab·Inspector 연결·빌드는 Windows(AyCode + Unity Editor)에서 수행.
- 같은 파일을 서버와 Windows에서 동시에 수정하지 않는다. Windows 작업 시작 전 `git pull`, 끝나면 push.

## 3. Git 정책

- 기능 단위 커밋, 작업 후 push. 강제 push·이력 삭제 금지.
- 비밀/API 키/개인정보, Unity `Library/ Temp/ Logs/ UserSettings/`, dotnet `bin/ obj/` 커밋 금지.
- 다른 프로젝트 디렉터리 수정 금지.

## 4. 기술 기준 (정본)

- Unity **6.3 LTS (6000.3.x)**, 템플릿 **Universal 3D (URP)**. 시작 씬은 **Warehouse.unity (9×7m, 천장 3.06m)**. PC/VR은 **XRI 공용 조작**을 사용한다. Classroom.unity는 역사적 회귀 테스트용 씬이다. 현행 공간·입력 기준은 `docs/WAREHOUSE_XRI.md`.
- 입력: `#if ENABLE_INPUT_SYSTEM`(새 Input System 직접 디바이스 폴링) / `#elif ENABLE_LEGACY_INPUT_MANAGER` 이중 지원. 게임플레이 의도는 `PlayerInputReader`에서 읽고 DesktopXriDriver / VrXriDriver → XRInteractionManager → 공용 Interactable/Socket → 기존 퍼즐 어댑터 → Core로 전달한다. 추적·시뮬레이터·패키지 UI 입력은 패키지 드라이버가 담당한다.
- UI: uGUI + TextMeshPro(uGUI 2.0에 포함). UI Toolkit 미사용.
- C# 9 이하 문법(Unity 호환). `CodeDrafts/Core`는 `UnityEngine` 참조 금지(`noEngineReferences: true` asmdef).
- 퍼즐 규칙은 Core 순수 클래스, MonoBehaviour는 입력·표현·연결만 담당.

## 5. 게임 설계 정본 (문서·코드가 반드시 일치해야 하는 값)

### 진행 구조
- 전체 상태 `GameFlowState`: `Title → Playing ⇄ Paused → Cleared` (Restart = 씬 재로드)
- 퍼즐 상태 `PuzzleState`: `Locked / Active / Solved`
- 시작 시 P1=Active, P2=Locked, P3=Locked. P1 Solved → P2 Active. P1·P2 Solved → P3 Active. P3 Solved → 출구 개방 → ExitTrigger 진입 → Cleared.
- 출구 문에 봉인 빛 3개(Seal1~3). 각 퍼즐 Solved 시 해당 빛이 꺼짐.

### 아이템 (`ItemData.itemId`)
| itemId | 이름 | 획득 |
|---|---|---|
| `ing_moonpetal` | 월광초 꽃잎 | 남동쪽 창가 화분 (3.75, .78, -2.94), 시작부터 획득 가능 |
| `ing_salamander_scale` | 불도마뱀 비늘 | 동쪽 보관 선반 유리병 (3.9, 1.03, 1.05) |
| `ing_crystal_dust` | 수정 가루 | 남서쪽 궤짝 내부 (뚜껑을 열어 획득) (-3.25, .36, -2.3) |
| `ing_toadstool` | 독버섯 (함정) | 북서쪽 바닥 구석 (-3.85, .13, 3.08) |
| `crystal_sapphire` | 푸른 수정 (물) | P1 보상 (제단에서 픽업) |
| `crystal_ruby` | 붉은 수정 (불) | P2 보상 (인벤토리에 직접 지급) |
| `potion_revelation` | 계시의 물약 | P2 보상 (인벤토리에 직접 지급) |
- 인벤토리 최대 8칸. 선택 슬롯: 숫자키 1~8 / 마우스 휠. 가마솥·봉인 장치는 **선택된 아이템**을 사용.

### P1 — 마법 문양 해독 (`RunePuzzle`)
- 문양 6개 `RuneId`: `Earth(대지) Water(물) Fire(불) Wind(바람) Light(빛) Shadow(그림자)` — 중앙 제단 원형 배치, 각 문양석 E로 누름.
- 정답: **Earth → Water → Fire → Light** (길이 4).
- 단서: 서쪽 벽 및 상자 뒤 바닥에 기대어 둔 액자 4점 '씨앗의 일생'(흙 속 씨앗/비/햇볕의 열기/빛 속의 꽃), 각 액자 모서리에 해당 문양, 아래 로마숫자 판(I~IV). 배치 순서는 섞임(III, I, IV, II). III·II는 같은 높이의 벽걸이, I·IV는 바닥과 벽에 닿도록 기댄다. 번호판과 그림등은 각 액자에 부착한다. 칠판 문구: "생명은 정해진 순서로 깨어난다. 액자의 숫자를 따르라."
- 기호 뜻은 문양석 6개와 액자 모서리에 고운바탕 명조체 황동 이름(대지·물·불·바람·빛·그림자)으로 바로 표시한다. 같은 기호와 이름을 액자 번호 I→IV 순서로 누른다. 문양석 이름도 기호와 함께 각인 채움·발광한다. 원소표와 확대 화면·낱장은 제거했다.
- I·IV는 손잡이 상자를 밀어야 조사 가능하고, 벽걸이 III(-4.39, 1.65, -1.75)는 덮개 천을 걷어야 조사 가능하다. 책 아래에는 단서가 아닌 제본용 나무판을 둔다. III·II의 벽걸이 높이와 기존 부착 규칙을 유지한다.
- 치우기: PC 상자는 좌클릭을 누른 채 WASD로 밀고 끌며, 놓으면 멈춘다. VR은 잡아 끌거나 손으로 민다. 상자는 서쪽 개별 레일과 바닥 높이에 제한되어 출구·제단·가마솥·소켓을 막지 못한다. 천/뚜껑은 클릭/E 또는 VR 선택으로 접기/경첩 열림. 책은 공용 Grab, 놓으면 옆 상자 위에 내려놓는다. 숨긴 단서/수정 가루는 콜라이더·XRI·직접 어댑터 모두 치우기 전 차단한다.
- 판정: 입력 즉시 비교. 오답 → 진행 0으로 리셋, failCount+1, 빨간 점멸+실패음. 재시도 무제한. failCount ≥ 3 → 힌트 "벽에 걸린 액자의 숫자 순서를 따라가 보자."
- 완료: 문양석 6개 모두 금색 발광 → 제단 뚜껑 열림 → `crystal_sapphire` 픽업 등장, Seal1 소등, 마법서 독서대의 결계 해제(P2 Active).

### P2 — 마법약 제조 (`PotionPuzzle`)
- 조합법(마법서): "계시의 물약 — ① 달빛 아래 핀 꽃잎 ② 불꽃 속에서 벗겨낸 비늘 ③ 별처럼 빛나는 가루. 독버섯은 결코 넣지 말 것."
- 정답 순서: **ing_moonpetal → ing_salamander_scale → ing_crystal_dust**. 재료 후보 4종(독버섯 포함).
- P2가 Locked이면 가마솥 상호작용 시 "가마솥이 차갑다. 제단의 봉인이 먼저인 것 같다." (실패 아님)
- 투입: 선택 아이템으로 E. 재료가 아닌 아이템 → "이건 넣을 수 없다." (실패 아님, 소모 없음). 재료면 인벤토리에서 소모 후 즉시 판정.
- 오답(순서 틀림·독버섯) → 검은 연기+실패음, 이번 시도에서 투입된 모든 재료를 **원래 위치에 재생성**, failCount+1. failCount ≥ 2 → 힌트 "마법서의 순서를 다시 읽어 보자. 독버섯은 함정이다."
- 완료: 녹색→금색 발광, `potion_revelation` + `crystal_ruby` 인벤토리 지급, Seal2 소등, P3 Active.

### P3 — 최종 봉인 해제 (`SealPuzzle`)
- 출구 앞 봉인 장치: 비문판(Inscription), 수정 소켓 `Left`/`Right`, 중앙 봉인핵(SealCore).
- 하위 상태 `SealStage`: `Dormant(P3 Locked) → AwaitingReveal → AwaitingCrystals → ReadyToRelease → Released`
- Dormant에서 상호작용 → "아직 봉인이 두 개 남아 있다." 
- AwaitingReveal: 비문판에 `potion_revelation` 선택 후 E → 물약 소모, 비문 출현: "왼쪽엔 흐르는 것을, 오른쪽엔 타오르는 것을." + 왼쪽 물 문양, 오른쪽 불 문양. 다른 아이템 → "비문이 흐릿해 읽을 수 없다."
- AwaitingCrystals: 정답 **Left=crystal_sapphire, Right=crystal_ruby**. 맞으면 소켓 고정(인벤토리에서 소모). 틀리면 거부(소모 안 함), failCount+1. failCount ≥ 2 → 힌트 "물은 흐르고 불은 타오른다. 수정의 색을 보자."
- 두 소켓 완료 → ReadyToRelease, 봉인핵 발광. 봉인핵 E → 최종 검증 → Released: Seal3 소등, 문 개방, ExitTrigger 활성.
- ExitTrigger 진입 → Cleared: 클리어 화면(소요 시간, 다시 시작/종료).

### 조작
- PC: WASD 이동, 마우스 시점, 좌클릭 잡기/누르기/소켓 투입, 우클릭 놓기. Warehouse의 E는 동일 XRI 클릭 경로 별칭. 1~8/휠 슬롯 선택, Tab 목표/힌트, ESC 일시정지. 카메라 XRRayInteractor 거리 2.5m. Classroom만 기존 E 입력 회귀 경로를 유지한다.
- VR: XR Origin Floor, 양손 컨트롤러 grip/trigger 선택, 왼손 primary 버튼 해제 시 텔레포트, 오른손 스틱 45° 스냅 회전.
- OpenXR 초기화 후 활성 HMD면 VR, 없으면 데스크톱. Inspector RigMode 또는 개발용 **F9**로 리그 전환. HMD 없이 강제 VR을 선택하면 공식 XR Device Simulator 샘플 사용.
- XRI 3.3.2 / OpenXR 1.16.1. 인벤토리는 기존 퍼즐 거래 원장, P2 보상은 기존 지급과 물리 픽업을 함께 제공한다. HandlingProfile은 Lift(책·작은 물건), PushPull(손잡이 상자), Fixed(뚜껑·천)로 분류한다. 공용 XRI 치우기는 퍼즐 상태를 바꾸지 않는다. Warehouse progressiveDarkening=true: four lighting stages, 1.6s blends, no interaction lock; tune WarehouseLighting.asset. S11 player build remains out of scope.

## 6. 산출물 위치

- `CodeDrafts/Core/` 순수 C# 규칙, `CodeDrafts/Runtime/` MonoBehaviour, `Tests/` dotnet 테스트
- Windows에서 생성할 Unity 프로젝트 위치: 저장소 루트의 `UnityProject/` (UNITY_HANDOFF.md §2). 생성 후에는 `UnityProject/Assets/Scripts`가 스크립트 정본이 될 수 있으므로, 서버에서 코드를 고치기 전 `git pull`로 확인한다.
- 서버 검증 명령: `~/.dotnet/dotnet test Tests/MagicEscape.Core.Tests`, `~/.dotnet/dotnet build Tests/RuntimeCompileCheck -p:InputBackend=New|Legacy`. `Tests/RuntimeCompileCheck/UnityStubs.cs`는 Unity로 복사 금지.
- `docs/` GAME_DESIGN, PUZZLE_SPEC, TECH_DESIGN, SCENE_SETUP, VISUAL_REFERENCES, UNITY_HANDOFF, TEST_PLAN, IDEA_LOG, TASKS
- 위 정본 값을 바꿀 때는 이 파일 → 코드 → 문서 → Notion 순으로 함께 갱신한다.

## 7. 비용·모델

- 유료 API·유료 에셋 구매 금지. 무료/CC0 에셋만 후보로 기록하고 라이선스를 명시.
- 외부 조사는 Perplexity MCP, 출처 URL과 조사일 기록.

> 아래 S10e/S10f는 Classroom 제작 이력이다. Warehouse의 공간·P2 위치·공용 XRI 입력·progressiveDarkening=true는 위 현행 기준과 `docs/WAREHOUSE_XRI.md`가 우선한다. 표시 규칙과 퍼즐 정답/ID는 유지한다.

## S10e — 시각 우선 단서와 연출 (2026-11-12)
- 사용자 요청으로 단서 표현을 시각 우선으로 변경. 정답/ID/Core/거리/공간은 불변.
- P1: 칠판 I→IV 그림 + 원소 모서리 문양, 정답 입력마다 대응 액자 점등/오답 전체 소등.
- P2: 번호 그림 레시피(달꽃→불비늘→별가루, 독버섯 X) + 청백→주황→금 액체.
- P3: 물약 사용 후 왼쪽 물/오른쪽 불 각인·효과 + 수정 자체 원소 표현.
- Chalkboard/Recipe/InscriptionRevealed 상수 보존. 단서 문장은 실패 기준(P1 3/P2 2/P3 2) 또는 Tab에서만. E는 그림 확대.
- Earth 갈색/녹색 균열, Water 파랑/물결·방울, Fire 주황/불씨, Wind 회녹/소용돌이·잎, Light 금/광륜, Shadow 보라검정/연기를 공통 사용.
- 시작 뒤 12초 무문장 오프닝, 아무 키 건너뛰기. 봉인 해제마다 밝기/앰비언스/다음 위치 신호. 출구 통과 시 백색 페이드 후 기존 Ending/시간/버튼.
- 연출은 표현 계층에서 입력 잠금. 테스트용 SkipPresentation() 명시 경로. Unity 하나, 시작 전 가상메모리 여유 6GB 필수.
- 재료 위치에 관한 이 역사적 S10e 항목은 현행 Warehouse 기준(위 아이템 표 및 `docs/WAREHOUSE_XRI.md`)으로 대체한다. Classroom 회귀 씬의 창가/책장/서랍/북서 구석 배치는 유지한다. 창가 월광초에 국소 달빛 추가. 새 외부 에셋 없이 자체 제작 그림/마스크 사용.

## S10f — 연금술 표시 규칙 (S10e 표시 규칙보다 우선)
- RuneId/정답/ID/판정/2.5m 거리 불변. 공간은 현행 Warehouse 기준이 우선한다. Fire=위 삼각형, Water=아래 삼각형, Wind=가로선 위 삼각형, Earth=가로선 아래 삼각형, Light=원 안 점, Shadow=토성(납). 색이 아닌 형태로 구분.
- 모든 기호는 동일 황동 각인, 같은 선 굵기/도안. 6개 석판 형태 통일, 누르면 각인 채움 뒤 짧은 효과. 물·불만 청·적 물질색 허용, 나머지는 저채도 황동.
- 봉인 해제마다 봉인등 소등+앰비언트/촛불 감소. 실내 단서 조명은 보존. 문 뒤는 차가운 야간 복도, 탈출은 검정 페이드, 엔딩은 검정 바탕 작은 금박 패널+고운바탕.
- 새 표시 코드 변경 시 CodeDrafts 미러 동기화. S10f 빌더는 기존 씬 위 `S10fApply`, 전체 재현은 `S10f`.
