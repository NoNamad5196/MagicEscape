# MagicEscape — 마법학교 방탈출

마법 실습 중 실수로 '삼중 봉인'을 발동시킨 견습 마법사가 창고의 단서를 조사하고 퍼즐 3개를 풀어 탈출하는 **3D 1인칭 퍼즐 방탈출** 게임입니다. (대학 '멀티미디어와 가상현실' 기말 작품)

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6.3 LTS · URP · uGUI/TextMeshPro · Input System(구 Input Manager도 지원) |
| 플랫폼 | Windows PC + OpenXR VR, XRI 공용 조작 |
| 플레이 | 10~15분, 9×7m Warehouse 1개(천장 3.06m), 퍼즐 3개 |
| 상태 | Windows Unity 6000.3.28f1 씬 생성·EditMode/PlayMode 검증 완료. 실제 HMD·S11 플레이어 빌드는 미검증 |

## 퍼즐 흐름

1. **마법 문양 해독** — 칠판의 번호별 장면을 액자와 맞추고, 명판의 원소 이름과 같은 제단 문양석을 누른다.
2. **마법약 제조** — 마법서 조합법대로 재료 3종을 가마솥에 넣는다(독버섯 함정).
3. **최종 봉인 해제** — 물약으로 비문을 드러내고 두 수정을 맞는 소켓에 끼운 뒤 봉인핵을 활성화한다.

## 창고와 공용 조작

칠판은 씬과 E 확대 모두 I~IV 아래 흙 둔덕·굵은 비·화로 불꽃·빛줄기와 작은 싹을 표시합니다. 같은 장소와 고정 구도의 연속 장면이며 원소 이름·그림문자는 없습니다. 숫자가 없는 액자 하단 황동 명판(그림문자·큰 고운바탕 이름)에서 원소를 확인합니다. Tab 글 힌트는 실패 기준(P1 3회, P2·P3 2회)을 채운 뒤에만 표시합니다. Classroom은 역사적 회귀 씬입니다.

시작 씬은 `UnityProject/Assets/Scenes/Warehouse.unity`. `Classroom.unity`는 기존 회귀 검증용으로 유지합니다. 현행 기준은 [WAREHOUSE_XRI.md](docs/WAREHOUSE_XRI.md), 실제 실행 결과는 [검증 기록](docs/validation/warehouse/RESULTS.md)을 참고하세요.

- PC: WASD/마우스, 좌클릭 잡기·누르기·투입(E도 동일 XRI 경로), 우클릭 놓기, 1~8/휠 슬롯, Tab 힌트, ESC 일시정지. 상호작용 거리 2.5m.
- VR: 양손 grip/trigger, 왼손 primary 버튼 해제 텔레포트, 오른손 스틱 45° 스냅 회전. 활성 HMD에 따라 자동 리그 선택; Inspector RigMode / 개발용 **F9**로 전환합니다. HMD 없이 강제 VR이면 공식 XR Device Simulator를 사용합니다.
- XRI 3.3.2 / OpenXR 1.16.1. PC·VR은 같은 Interactable/Socket과 기존 퍼즐 규칙을 사용합니다. 정답·아이템 ID는 바뀌지 않습니다.
- 치우기: 상자는 좌클릭을 누른 채 이동해서 I·IV 앞에서 밀고 끌며, III의 덮개 천/수정 가루 궤짝은 클릭/E로 열고, 책은 잡아 옮깁니다. VR도 같은 XRI 오브젝트를 씁니다. 문양석 6개와 액자에는 기호와 같은 황동 톤의 고운바탕 한글 이름이 새겨져 있습니다. 칠판 장면 I→IV에 대응하는 액자의 이름·그림문자와 같은 돌을 누릅니다. 원소표는 없고 책 아래는 제본용 나무판 소품입니다. Stage 4: four progressively darker lighting states with local puzzle lights and 1.6s transitions; S11 player build remains out of scope.

| P2 재료 ID | 창고 위치 (m) |
|---|---|
| `ing_moonpetal` | 남동쪽 창가 화분 (3.75, .78, -2.94) |
| `ing_salamander_scale` | 동쪽 작업대 책 더미 뒤 유리병 (3.65, 1.30, .05) |
| `ing_crystal_dust` | 남서쪽 궤짝 내부 (뚜껑을 열어 획득) (-3.25, .96, -2.3) |
| `ing_toadstool` | 동쪽 나무통 뚜껑 아래 (3.50, 1.10, -1.25) |

## 저장소 구조

```text
AGENTS.md            작업 정책 + 설계 정본(정답·아이템 ID·상태)
CodeDrafts/Core      엔진 독립 규칙(순수 C#) — 퍼즐 3종, 진행, 인벤토리, 문구
CodeDrafts/Runtime   Unity MonoBehaviour — 플레이어, 상호작용, 퍼즐 연출, UI, 오디오
Tests/               dotnet xUnit(Core 44건), Unity EditMode/PlayMode 테스트
docs/                기획서·퍼즐 명세·기술 설계·씬 구성·인수인계·테스트·에셋
```

## 문서

- [게임 기획서](docs/GAME_DESIGN.md) · [퍼즐 명세](docs/PUZZLE_SPEC.md) · [기술 설계](docs/TECH_DESIGN.md)
- [씬·Inspector 구성](docs/SCENE_SETUP.md) · [**Windows 인수인계**](docs/UNITY_HANDOFF.md) · [테스트 계획](docs/TEST_PLAN.md)
- [비주얼·에셋](docs/VISUAL_REFERENCES.md) · [작업 현황](docs/TASKS.md) · [아이디어 일지](docs/IDEA_LOG.md)
- [에셋·사운드 크레딧](docs/CREDITS.md)

## 서버에서 검증하기

```bash
dotnet test Tests/MagicEscape.Core.Tests                           # Core 규칙 44건
dotnet build Tests/RuntimeCompileCheck -p:InputBackend=New         # Runtime 스텁 컴파일
dotnet build Tests/RuntimeCompileCheck -p:InputBackend=Legacy
```

XRI는 기존 RuntimeCompileCheck 스텁이 모델링하지 않으므로 실제 Unity 테스트가 필요합니다. Windows batchmode 재현 명령은 [commands.txt](docs/validation/warehouse/commands.txt)에 있습니다.

Unity 적용 방법은 [UNITY_HANDOFF.md](docs/UNITY_HANDOFF.md)를 따릅니다. `Tests/RuntimeCompileCheck/UnityStubs.cs`는 Unity 프로젝트에 복사하지 마세요.

Warehouse Stage 7: 칠판의 번호별 장면을 액자와 맞추고, 숫자 없는 명판의 직관적 황동 그림문자·큰 고운바탕 이름으로 문양석을 선택한다. 연금술 기호는 Classroom 회귀 씬에만 남는다. 물건 새 위치·조명·예외(월광초 .78m 유지)는 `docs/WAREHOUSE_XRI.md`를 따른다.

Stage 7 실제 검증·캡처: [STAGE7_RESULTS.md](docs/validation/warehouse/STAGE7_RESULTS.md).

## Stage 10 — 원소 장면과 치우기

칠판 4장과 유화 4장을 이미지 생성으로 교체했습니다. 같은 흙 둔덕과 오른쪽 화로가 이어지며 흙 → 비 → 큰 불꽃 → 빛줄기가 각각 주인공입니다. 식물은 조연이고 그림 안 태양·꽃·달·소용돌이·글자는 없습니다. 명판·문양석·정답은 유지합니다.

기존 상자 2개·천·궤짝·제본 책에 작업대 책 더미, 나무통 뚜껑, 궤짝 속 포장 천 묶음을 더해 치울 물건은 총 8개입니다. 조준/근접 발광, 좌클릭 조작 안내, 마찰음·착지음·먼지, 틈새 빛과 해제 시 국소 조명·반짝임·픽업음을 제공합니다. 수정 가루는 궤짝을 열고 천 묶음까지 들어내야 집을 수 있습니다. 달꽃 위치는 그대로입니다. [Stage 10 검증](docs/validation/warehouse/STAGE10_RESULTS.md).
