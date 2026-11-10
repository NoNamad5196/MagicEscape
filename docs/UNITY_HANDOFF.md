# MagicEscape — Windows / AyCode / Unity 인수인계

> 이 문서만 읽고 Windows에서 바로 제작을 시작할 수 있도록 작성했다.
> 서버(Instabox)에서 완료된 것: 기획·퍼즐 명세·C# 코드 초안(Core 테스트 34/34 통과, Runtime 스텁 컴파일 통과)·씬 명세.
> **Windows에서 처음 확인할 것**: 실제 Unity 컴파일, Play Mode 동작. 서버는 Unity를 실행하지 않았으므로 아래 모든 Unity 단계는 미검증 상태다.

## 0. 한눈에 보기

| 순서 | 작업 | 산출물 | 예상 시간 |
|---|---|---|---|
| 1 | Unity 6.3 LTS 설치, URP 프로젝트를 저장소 안에 생성 | `UnityProject/` | 30분 |
| 2 | CodeDrafts → Assets/Scripts 복사, 컴파일 오류 0 | 스크립트 | 20분 |
| 3 | 그레이박스 방 + 플레이어 + UI | 이동·UI 동작 | 2시간 |
| 4 | 퍼즐 1 → 2 → 3 순서로 오브젝트 구성·연결 | 전체 클리어 가능 | 3~4시간 |
| 5 | 에셋·조명·사운드·VFX 교체 | 완성 외형 | 3~5시간 |
| 6 | 테스트([TEST_PLAN.md](TEST_PLAN.md)), 빌드, 시연 리허설, PPT 캡처 | `Builds/` exe, 영상·스크린샷 | 2시간 |

## 1. Unity 버전과 패키지

**권장: Unity 6.3 LTS (6000.3.x 최신 패치)**
- 근거: 2025-12-04 출시된 현행 LTS, 2027년 12월까지 지원(조사일 2026-11-10 기준 6.7 LTS는 아직 미출시). 학기 중 안정성이 가장 중요하므로 LTS를 선택.
- 다른 버전을 쓸 경우: Unity 6.0 LTS 이상이면 코드 수정 없이 동작하도록 작성했다(uGUI 2.0 + TMP, Input System 1.x, C# 9).

| 패키지 | 필요 | 비고 |
|---|---|---|
| Universal RP | 필수 | Universal 3D 템플릿에 포함 |
| uGUI (`com.unity.ugui` 2.x) | 필수 | TextMeshPro 포함(별도 TMP 패키지 불필요). 처음 TMP 사용 시 "Import TMP Essentials" 실행 |
| Input System | 권장 | 템플릿에 포함될 수 있음. 없으면 Package Manager에서 설치 |
| XR 관련 | 설치하지 않음 | VR 필수 여부 확정 전 |

Player Settings › Other Settings › **Active Input Handling**: `Input System Package (New)` 또는 `Both`. 코드(`PlayerInputReader`)는 어느 쪽이든 동작한다.

## 2. 프로젝트 생성과 저장소 통합

저장소 구조(통합 후 목표):
```text
MagicEscape/            ← git 저장소 루트 (서버 로컬 폴더명은 MagicSchoolEscape)
├── AGENTS.md, README.md
├── CodeDrafts/         ← 서버 작성 원본(정본). Unity가 읽지 않음
├── Tests/              ← dotnet 테스트(서버용)
├── docs/
└── UnityProject/       ← Windows에서 새로 생성
    ├── Assets/  Packages/  ProjectSettings/   ← 커밋
    └── Library/ Temp/ Logs/ UserSettings/     ← 커밋 금지
```

절차:
1. `git clone https://github.com/NoNamad5196/MagicEscape.git` (이미 있으면 `git pull`).
2. Unity Hub › New project › **Universal 3D** 템플릿 › Project name `UnityProject`, Location = 저장소 루트.
3. 저장소 루트의 `.gitignore`는 루트 기준(`/Library/` 등)이므로, **`UnityProject/.gitignore`를 추가**한다. 내용은 루트 `.gitignore`의 Unity 블록을 그대로 복사(경로가 UnityProject 기준이 되도록).
4. Edit › Project Settings › Editor: Version Control Mode = Visible Meta Files, Asset Serialization = Force Text (기본값 확인).
5. 첫 커밋: `git add UnityProject/Assets UnityProject/Packages UnityProject/ProjectSettings UnityProject/.gitignore` → `chore(unity): Unity 6.3 URP 프로젝트 생성`.
6. 대용량 바이너리(에셋 FBX/텍스처/오디오)가 100MB를 넘을 경우에만 Git LFS 검토. 무료 저폴리 팩은 대개 불필요.

**동시 수정 규칙**: `CodeDrafts/`와 `docs/`는 서버(OpenCode)도 수정할 수 있다. Windows에서 스크립트를 고치면 **`UnityProject/Assets/Scripts`를 정본으로 삼고**, 같은 커밋에서 `CodeDrafts`도 똑같이 갱신하거나 TASKS.md에 "Scripts가 정본" 이라고 명시한다. 작업 시작 전 `git pull`, 끝나면 push.

## 3. 스크립트 적용 순서

`CodeDrafts/` → `UnityProject/Assets/Scripts/`로 폴더째 복사:
```text
Assets/Scripts/Core/      ← CodeDrafts/Core/*.cs + MagicEscape.Core.asmdef
Assets/Scripts/Runtime/   ← CodeDrafts/Runtime/** (하위 폴더 유지)
```
- `Tests/`는 복사하지 않는다(특히 `Tests/RuntimeCompileCheck/UnityStubs.cs`는 **절대 복사 금지** — UnityEngine 타입과 충돌).
- Runtime 폴더에는 asmdef를 만들지 않는다(Assembly-CSharp가 Core asmdef, TMP, Input System을 자동 참조).

복사 후 확인:
1. Console에 컴파일 오류 0. (경고 CS0649가 보이면 무시 가능하나 Unity 6은 [SerializeField]에 대해 표시하지 않는 것이 정상)
2. Add Component 검색창에 `RunePuzzle`, `GameManager`가 보이는지.
3. Project 창 Create 메뉴에 `MagicEscape › Item Data`가 있는지.

컴파일 오류가 나면 → 10절 오류 체크리스트.

## 4. 씬 구성 순서 (AyCode 작업 단위)

상세 값은 [SCENE_SETUP.md](SCENE_SETUP.md). 각 단계 끝에 Play로 확인하고 커밋한다.

| 단계 | 작업 | 확인 | 커밋 메시지 예 |
|---|---|---|---|
| S1 | `Classroom` 씬, 레이어(Player, Interactable), 방 그레이박스(Cube), 조명 기본 | 씬 저장 | `feat(scene): 교실 그레이박스` |
| S2 | Systems(GameManager, InventoryManager, AudioManager, EventSystem), Player | skipTitle=true로 이동·시점 | `feat(player): 1인칭 이동·시점` |
| S3 | UI_Canvas 전 패널, 한글 TMP 폰트, 버튼 연결, Hotbar | 타이틀→시작→ESC→재시작 | `feat(ui): 패널·HUD·핫바` |
| S4 | ItemData 7개, InventoryManager 카탈로그 | Create 메뉴 동작 | `feat(items): ItemData 7종` |
| S5 | 단서 오브젝트(액자 4, 칠판) | 읽기 패널 | `feat(clues): 액자·칠판` |
| S6 | 퍼즐 1 Altar 전체 | 정답/오답/보상/봉인 빛 1 | `feat(p1): 문양 제단` |
| S7 | 퍼즐 2 Cauldron, RecipeBook, 재료 4 | 잠금/정답/오답 재생성/보상 | `feat(p2): 마법약 제조` |
| S8 | 퍼즐 3 SealDevice, ExitDoor, ExitTrigger | 비문/소켓/봉인핵/문/클리어 | `feat(p3): 최종 봉인·출구` |
| S9 | 전체 플레이 1회 + Console 0 Error | TEST_PLAN 핵심 시나리오 | `test: 그레이박스 전체 클리어 확인` |
| S10 | 에셋 교체·조명·Volume·VFX·사운드 | 외형 완성 | `art: …` |
| S11 | 빌드 | exe 실행 | `build: Windows 빌드 설정` |

## 5. Prefab 제작 순서

1. `PF_RuneStone` (문양석 1개 완성 → Prefab → 5개 더 배치, rune/텍스처만 변경)
2. `PF_ItemPickup` (재료 1개 완성 → Prefab → 나머지 재료·보상 수정)
3. `PF_Painting` (액자 1개 → 4개)
4. `PF_CrystalSocket`, `PF_SealLight`
5. `PF_Player` (마지막에 Prefab화 — 씬 참조가 끊기지 않도록 씬 인스턴스에서 GameManager.player 재확인)

## 6. 퍼즐 구현 순서와 최소 연결 체크

### P1 (S6)
- [ ] Altar에 RunePuzzle, runeStones 6개 연결, 각 RuneStone.rune 지정(Light/Shadow/Earth/Wind/Water/Fire)
- [ ] 문양 머티리얼 Emission ON, GlowFeedback.targetRenderer 지정
- [ ] Reward_Sapphire: ItemPickup(item=Item_CrystalSapphire, hiddenAtStart=true)
- [ ] GameManager.runePuzzle = Altar
- 테스트: 대지→물→불→빛 / 중간 오답

### P2 (S7)
- [ ] Cauldron에 PotionPuzzle, ingredientPickups 4개, Liquid GlowFeedback
- [ ] RecipeBook: InspectableObject(requirePuzzleSolved, Rune, barrierVisual=Barrier, Barrier 콜라이더 없음)
- [ ] GameManager.potionPuzzle = Cauldron
- 테스트: P1 전 가마솥 / 꽃잎→가루(오답) 재생성 / 정답 보상

### P3 (S8)
- [ ] SealDevice에 SealPuzzle, 소켓·비문·봉인핵 참조, RevealedText 비활성
- [ ] SealInscription.puzzle, SealCore.puzzle = SealDevice
- [ ] ExitDoor: DoorController(hinge, sealLights 3, exitTrigger), ExitTrigger 비활성 BoxCollider isTrigger
- [ ] GameManager.sealPuzzle, exitDoor
- 테스트: 비문/소켓 반대/정상/봉인핵/문/탈출

## 7. UI·사운드 적용

- **한글 폰트**: Noto Sans KR 또는 Pretendard(TTF/OTF) 임포트 → Font Asset Creator(Atlas 4096×4096, Render Mode SDFAA, Character Set: Characters from File = `CodeDrafts/Core/GameText.cs` + UI 문자열 텍스트 파일, 또는 Unicode Range `AC00-D7A3,3131-318E,0020-007E`) → 모든 TMP에 적용. 혹은 Dynamic 폰트 에셋으로 생성해 런타임 글리프 추가.
- **사운드**: [VISUAL_REFERENCES.md](VISUAL_REFERENCES.md)의 CC0 사운드를 `Assets/Audio/`에 넣고 AudioManager.clips에 `SfxId`별로 연결. 클립이 없어도 게임은 동작.
- **VFX**: Cartoon FX Remaster Free 프리팹을 각 VFX_* 자리에 넣고 Loop/Play On Awake 끄기. URP에서 안 보이면 URP Asset의 Depth Texture ON.

## 8. Play Mode 테스트

[TEST_PLAN.md](TEST_PLAN.md)의 "Windows 수동 테스트"를 순서대로. 최소 통과 기준(시연 가능 판정):
- TC-W01~W06(기본 조작·UI), TC-W10~W14(P1), TC-W20~W26(P2), TC-W30~W37(P3), TC-W40~W43(재시작·빌드·Console) 전부 통과
- Console Error 0건

결과는 `docs/TASKS.md`의 Windows 표와 TEST_PLAN 결과 칸에 기록하고 커밋 → 서버 측에서 Notion에 반영.

## 9. 빌드

1. File › Build Profiles(Unity 6) › Windows 프로필 선택 › Scene List에 `Classroom`만 체크(index 0).
2. Player Settings: Company/Product Name `MagicEscape`, Default Screen Mode `Fullscreen Window`, Resolution 1920×1080, Resizable Window ON.
3. Build → 저장소 밖 또는 `UnityProject/Builds/`(gitignore 처리됨)에 출력.
4. exe 실행 → 타이틀 → 전체 클리어 → 종료 버튼으로 앱이 닫히는지 확인.
5. 제출용으로 Build 폴더 전체를 zip.

## 10. 오류 해결 체크리스트

| 증상 | 원인 | 해결 |
|---|---|---|
| `The type or namespace name 'TMPro' could not be found` | uGUI 2.0 미설치/TMP 미임포트 | Package Manager에서 uGUI 확인, Window › TextMeshPro › Import TMP Essential Resources |
| `The type or namespace name 'InputSystem' …` | Active Input Handling이 New/Both인데 패키지 없음 | Input System 패키지 설치, 또는 Active Input Handling을 Old로 |
| `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class…` | EventSystem이 StandaloneInputModule | EventSystem에서 InputSystemUIInputModule로 교체 |
| 클래스가 Add Component에 없음 | 파일명 ≠ 클래스명 / 컴파일 오류 | Console 오류부터 해결, 파일명 확인 |
| `UnityEngine.Object` 관련 중복 정의 오류 | `UnityStubs.cs`를 복사함 | 해당 파일 삭제 |
| 한글이 □로 보임 | TMP 기본 폰트 | 한글 SDF 폰트 에셋 적용 |
| 마우스가 안 움직임/너무 빠름 | 입력 백엔드별 delta 단위 | PlayerController.mouseSensitivity, PlayerInputReader.newInputMouseScale 조정 |
| 프롬프트가 안 뜸 | Collider 없음 / interactMask / 거리 | 대상에 Collider, Raycast 거리 2.5m 이내, interactMask에 해당 레이어 포함 |
| 자기 자신이 조준됨 | interactMask에 Player 포함 | Player 레이어 제외 |
| 발광이 안 보임 | 머티리얼 Emission OFF / Bloom 없음 | URP Lit Emission 체크, Global Volume Bloom |
| 문양이 눌러도 반응 없음 | RunePuzzle.runeStones 누락 / P1 상태 | Console `[RunePuzzle]` 로그 확인 |
| 가마솥에 넣어도 "이건 넣을 수 없다" | ItemData.itemId 오타 | ItemIds.cs 값과 비교 |
| 오답 후 재료가 안 돌아옴 | ingredientPickups 누락 | 4개 연결, `재생성할 ItemPickup을 찾지 못함` 로그 확인 |
| 문이 안 열림 | exitDoor/hinge 누락, P3 미해결 | `[GameManager] Seal → Solved` 로그 확인 |
| 문 밖으로 나가도 클리어 안 됨 | ExitTrigger isTrigger OFF / 플레이어에 PlayerController 없음 / 트리거 이벤트 미발생 | BoxCollider isTrigger, 위치 확인. 그래도 안 되면 ExitTrigger에 Rigidbody(Is Kinematic ON, Use Gravity OFF) 추가 |
| 일시정지 후 버튼 클릭 불가 | EventSystem 없음 | 씬에 EventSystem 1개 |
| VFX가 Game 뷰에서 안 보임 | URP Depth Texture OFF | URP Asset › Depth Texture ON |
| 재시작 후 커서/시간 이상 | timeScale | GameManager.Restart가 1로 복구 — 다른 스크립트가 timeScale을 바꾸는지 확인 |

## 11. 최종 시연 절차 (발표 당일)

1. 빌드 exe 실행(전체 화면). 볼륨 확인.
2. 타이틀에서 콘셉트 한 줄 설명 → 시작.
3. 액자·칠판 조사로 단서 시스템 시연 → 제단 오답 1회(피드백 시연) → 정답.
4. 마법서 열람 → 재료 수집 → 가마솥 오답 1회(재료 재생성 시연) → 정답.
5. 비문 해독 → 수정 반대로 1회(거부 시연) → 정답 → 봉인핵 → 문 개방 연출.
6. 탈출 → 클리어 화면 시간.
7. (선택) Tab 힌트 패널, ESC 일시정지 시연.
8. 백업: 시연 영상(mp4)을 미리 녹화해 두고 PPT에 삽입.
- 정답 치트시트: [PUZZLE_SPEC.md §5](PUZZLE_SPEC.md#5-정답-치트시트-시연용)

## 12. PPT용 캡처 목록 (Windows에서 확보)

| 캡처 | 용도 | 상태 |
|---|---|---|
| 타이틀 화면 | 표지/콘셉트 | 미확보 |
| 교실 전경(탑뷰 Scene 뷰 + 게임 뷰) | 레벨 구성 | 미확보 |
| 액자 단서 읽기 패널 | 퍼즐 1 | 미확보 |
| 문양 오답 빨간 점멸 / 정답 금색 | 피드백 | 미확보 |
| 가마솥 색 변화, 검은 연기 | 퍼즐 2 | 미확보 |
| 비문 해독, 소켓 거부, 문 개방 | 퍼즐 3 | 미확보 |
| 클리어 화면(시간) | 결과 | 미확보 |
| Inspector(GameManager, RunePuzzle) | 구현 근거 | 미확보 |
| 전체 플레이 영상 3~5분 | 시연 백업 | 미확보 |

서버에서 만든 다이어그램(클래스 구조·상태도)은 [TECH_DESIGN.md](TECH_DESIGN.md), [PUZZLE_SPEC.md](PUZZLE_SPEC.md)의 Mermaid를 렌더링해 사용한다. **실제 게임 화면을 가상으로 만들어 쓰지 않는다.**
