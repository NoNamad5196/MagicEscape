# MagicEscape — 마법학교 방탈출

마법 실습 중 실수로 '삼중 봉인'을 발동시킨 견습 마법사가 창고의 단서를 조사하고 퍼즐 3개를 풀어 탈출하는 **3D 1인칭 퍼즐 방탈출** 게임입니다. (대학 '멀티미디어와 가상현실' 기말 작품)

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6.3 LTS · URP · uGUI/TextMeshPro · Input System(구 Input Manager도 지원) |
| 플랫폼 | Windows PC + OpenXR VR, XRI 공용 조작 |
| 플레이 | 10~15분, 9×7m Warehouse 1개(천장 3.06m), 퍼즐 3개 |
| 상태 | Windows Unity 6000.3.28f1 씬 생성·EditMode/PlayMode 검증 완료. 실제 HMD·S11 플레이어 빌드는 미검증 |

## 퍼즐 흐름

1. **마법 문양 해독** — 벽의 액자 4점 번호를 읽고 제단 문양을 순서대로 누른다.
2. **마법약 제조** — 마법서 조합법대로 재료 3종을 가마솥에 넣는다(독버섯 함정).
3. **최종 봉인 해제** — 물약으로 비문을 드러내고 두 수정을 맞는 소켓에 끼운 뒤 봉인핵을 활성화한다.

## 창고와 공용 조작

시작 씬은 `UnityProject/Assets/Scenes/Warehouse.unity`. `Classroom.unity`는 기존 회귀 검증용으로 유지합니다. 현행 기준은 [WAREHOUSE_XRI.md](docs/WAREHOUSE_XRI.md), 실제 실행 결과는 [검증 기록](docs/validation/warehouse/RESULTS.md)을 참고하세요.

- PC: WASD/마우스, 좌클릭 잡기·누르기·투입(E도 동일 XRI 경로), 우클릭 놓기, 1~8/휠 슬롯, Tab 힌트, ESC 일시정지. 상호작용 거리 2.5m.
- VR: 양손 grip/trigger, 왼손 primary 버튼 해제 텔레포트, 오른손 스틱 45° 스냅 회전. 활성 HMD에 따라 자동 리그 선택; Inspector RigMode / 개발용 **F9**로 전환합니다. HMD 없이 강제 VR이면 공식 XR Device Simulator를 사용합니다.
- XRI 3.3.2 / OpenXR 1.16.1. PC·VR은 같은 Interactable/Socket과 기존 퍼즐 규칙을 사용합니다. 정답·아이템 ID는 바뀌지 않습니다.
- 치우기/밀기, 새로운 어두워짐 연출, S11 빌드는 아직 포함하지 않습니다.

| P2 재료 ID | 창고 위치 (m) |
|---|---|
| `ing_moonpetal` | 남동쪽 창가 화분 (3.75, .78, -2.94) |
| `ing_salamander_scale` | 동쪽 보관 선반 유리병 (3.9, 1.03, 1.05) |
| `ing_crystal_dust` | 남서쪽 열린 상자 내부 (-3.25, .36, -2.3) |
| `ing_toadstool` | 북서쪽 바닥 (-3.85, .13, 2.7) |

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
