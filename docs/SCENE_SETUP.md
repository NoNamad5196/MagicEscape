# MagicEscape — 씬·프리팹·Inspector 구성 명세

> 대상: Windows Unity 6.3 LTS(URP) 작업자(AyCode). 이 서버에서는 씬/프리팹 파일을 만들지 않았다. 아래 순서대로 Editor에서 직접 만든다.
> 좌표계: 바닥 중앙 (0,0,0), **+Z = 북쪽(출구)**, +X = 동쪽, 단위 m. 레이아웃 근거는 [GAME_DESIGN.md §8](GAME_DESIGN.md#8-레벨-및-공간-구성).
> 표기: `이름 [컴포넌트, …]` · Inspector 값은 `필드 = 값`.

## 0. 사전 설정

| 항목 | 값 |
|---|---|
| 씬 | `Assets/Scenes/Classroom.unity` 1개 (Build Settings index 0) |
| 레이어 | `Player`(8), `Interactable`(9) 추가 |
| 태그 | 기본 `Player` 사용 |
| Player Settings › Active Input Handling | `Input System Package (New)` 또는 `Both` (코드는 둘 다 지원) |
| Color Space | Linear (기본) |
| URP Asset | Depth Texture ON (Cartoon FX Soft Particle용), HDR ON, Post Processing ON(Renderer) |
| Physics | 기본 |

## 1. 전체 계층 구조

```text
Classroom (Scene)
├── --- Systems ---
│   ├── GameManager            [GameManager]
│   ├── InventoryManager       [InventoryManager]
│   ├── AudioManager           [AudioManager, AudioSource(BGM), AudioSource(SFX)]
│   ├── EventSystem            [EventSystem, InputSystemUIInputModule]
│   └── GlobalVolume           [Volume (Global)]
├── --- Player ---
│   └── Player (Tag Player, Layer Player)  [CharacterController, PlayerInputReader, PlayerController, PlayerInteraction]
│       └── CameraPivot (0, 1.6, 0)        [Camera(MainCamera), AudioListener, UniversalAdditionalCameraData]
├── --- Environment ---
│   ├── Room
│   │   ├── Floor, Ceiling, Wall_N(문 구멍), Wall_S(창문), Wall_E, Wall_W   [MeshRenderer, BoxCollider]
│   │   ├── Window_SE
│   │   └── Props (책상·의자·책장·촛대·장식, 상호작용 없음)
│   └── Lighting
│       ├── Moonlight (Directional)
│       ├── Lamp_Desk, Lamp_Shelf_1, Lamp_Shelf_2, Fire_Cauldron (Point)
│       └── Spot_Altar, Spot_Seal (Spot)
├── --- Clues ---
│   ├── Painting_III, Painting_I, Painting_IV, Painting_II  [InspectableObject, BoxCollider]
│   └── Chalkboard                                          [InspectableObject, BoxCollider]
├── --- Puzzle1_Rune ---
│   └── Altar [RunePuzzle]
│       ├── AltarBase   [MeshRenderer, MeshCollider/BoxCollider]
│       ├── AltarLid    [MeshRenderer, BoxCollider]
│       ├── RuneStone_Light / _Shadow / _Earth / _Wind / _Water / _Fire  [RuneStone, GlowFeedback, MeshRenderer, BoxCollider, (Light)]
│       ├── Reward_Sapphire  [ItemPickup, MeshRenderer, SphereCollider]  (hiddenAtStart)
│       └── VFX_RuneSolved   [ParticleSystem]
├── --- Puzzle2_Potion ---
│   ├── Cauldron [PotionPuzzle, MeshRenderer, MeshCollider]
│   │   ├── Liquid        [GlowFeedback, MeshRenderer]
│   │   ├── VFX_Bubble, VFX_Smoke, VFX_Success  [ParticleSystem]
│   ├── RecipeBook [InspectableObject, BoxCollider]
│   │   └── Barrier      [MeshRenderer(반투명), 콜라이더 없음]
│   ├── Ingredient_Moonpetal        [ItemPickup, Collider]
│   ├── Ingredient_SalamanderScale  [ItemPickup, Collider]
│   ├── Ingredient_CrystalDust      [ItemPickup, Collider]
│   └── Ingredient_Toadstool        [ItemPickup, Collider]
├── --- Puzzle3_Seal ---
│   ├── SealDevice [SealPuzzle]
│   │   ├── Pedestal       [MeshRenderer, BoxCollider]
│   │   ├── Inscription    [SealInscription, GlowFeedback, MeshRenderer, BoxCollider]
│   │   │   └── RevealedText  [TextMeshPro(3D), 자식 Quad_WaterRune, Quad_FireRune]
│   │   ├── Socket_Left    [CrystalSocket, GlowFeedback, MeshRenderer, BoxCollider]
│   │   │   └── PlacedCrystal (푸른 수정 모델, 비활성)
│   │   ├── Socket_Right   [CrystalSocket, GlowFeedback, MeshRenderer, BoxCollider]
│   │   │   └── PlacedCrystal (붉은 수정 모델, 비활성)
│   │   ├── SealCore       [SealCore, GlowFeedback, MeshRenderer, SphereCollider, Light]
│   │   └── VFX_Reveal, VFX_Release [ParticleSystem]
│   └── ExitDoor [DoorController]
│       ├── DoorFrame      [MeshRenderer, BoxCollider]
│       ├── DoorHinge      (문 왼쪽 가장자리 위치의 빈 오브젝트)
│       │   └── DoorPanel  [MeshRenderer, BoxCollider]
│       ├── SealLight_1, SealLight_2, SealLight_3  [MeshRenderer(빨강 Emission 구), Light(Point 빨강)]
│       ├── VFX_DoorOpen   [ParticleSystem]
│       └── ExitTrigger    [BoxCollider(isTrigger), ExitTrigger]  (비활성 시작)
└── UI_Canvas [Canvas(Screen Space-Overlay), CanvasScaler, GraphicRaycaster, UIManager]
    ├── TitlePanel / HUDPanel / PausePanel / ClearPanel / ReadPanel / ObjectivePanel
    └── (5절 참고)
```

## 2. 시스템 오브젝트

### GameManager
- 빈 GameObject `GameManager` + `GameManager`
- `player` = Player, `runePuzzle` = Altar, `potionPuzzle` = Cauldron, `sealPuzzle` = SealDevice, `exitDoor` = ExitDoor
- `skipTitle` = false (개발 중 빠른 테스트 때만 true)

### InventoryManager
- `capacity` = 8
- `itemCatalog` = ItemData 7개(아래 6절) 모두 드래그

### AudioManager
- AudioSource 2개 추가: 첫째 `bgmSource`(Loop ON, Volume 0.4, Play On Awake OFF, Spatial Blend 0), 둘째 `sfxSource`(Volume 1, Play On Awake OFF, 2D)
- `bgmClip` = 판타지 루프 1곡
- `clips` 배열: `SfxId` 15종마다 한 줄 {id, clip, volume}. 비어 있어도 오류 없음(그레이박스 단계 가능)

### EventSystem
- Canvas 생성 시 자동 생성되는 EventSystem 사용. Active Input Handling이 New이면 `StandaloneInputModule` → **`InputSystemUIInputModule`로 교체**(Inspector의 "Replace with InputSystemUIInputModule" 버튼). 교체하지 않으면 UI 버튼 클릭 시 InvalidOperationException.

### GlobalVolume
- `Volume`(Mode Global) + 새 Profile: Bloom(Threshold 0.9, Intensity 1.2, Scatter 0.6), Vignette(0.25), Tonemapping(ACES), Color Adjustments(Post Exposure 0, Saturation 5, Color Filter 연한 주황)
- Main Camera › Rendering › Post Processing 체크

## 3. 플레이어

| 오브젝트 | 설정 |
|---|---|
| `Player` 위치 | (0, 0, -4), 회전 (0, 0, 0) — 북쪽을 바라봄 |
| Tag / Layer | Player / Player |
| CharacterController | Center (0, 0.9, 0), Radius 0.3, Height 1.8, Skin Width 0.05, Step Offset 0.3, Slope Limit 45 |
| PlayerInputReader | newInputMouseScale = 0.1 |
| PlayerController | cameraPivot = CameraPivot, walkSpeed 3.2, gravity -15, mouseSensitivity 2, minPitch -80, maxPitch 80, invertY false |
| PlayerInteraction | viewCamera = CameraPivot의 Camera, interactDistance 2.5, interactMask = Everything에서 **Player 제외** |
| `CameraPivot` | 로컬 (0, 1.6, 0), Camera: FOV 70, Near 0.05, Far 50, Tag MainCamera, Post Processing ON |

- 씬의 기본 Main Camera는 삭제한다(카메라 2개 금지).
- Player 레이어를 interactMask에서 빼지 않으면 Raycast가 자기 CharacterController에 맞을 수 있다.

## 4. 환경·조명

### 4.1 방
| 오브젝트 | Transform (위치 / 스케일) | 비고 |
|---|---|---|
| Floor | (0,0,0) / Plane (1.2,1,1) 또는 Cube (12,0.1,10) y=-0.05 | 나무 바닥 |
| Ceiling | (0,4.5,0) / Cube (12,0.1,10) | 어두운 목재 |
| Wall_W | (-6,2.25,0) / (0.2,4.5,10) | 액자·칠판 부착 |
| Wall_E | (6,2.25,0) / (0.2,4.5,10) | 책장 |
| Wall_S | (0,2.25,-5) / (12,4.5,0.2) | 창문(남동) |
| Wall_N | 문 구멍(폭 1.6, 높이 2.6)을 두고 좌·우·상 Cube 3개로 구성 | 중앙 x=0 |
- 모든 벽·바닥에 BoxCollider. 소품은 Static 체크(라이트 베이크 시).

### 4.2 조명
| 이름 | 종류 | 위치 | 설정 |
|---|---|---|---|
| Moonlight | Directional | 회전 (35, 150, 0) | 색 #8FA8FF, Intensity 0.3, Soft Shadows |
| Lamp_Desk | Point | (-3.5, 1.4, -3.0) | #FFB45A, Intensity 2, Range 4 |
| Lamp_Shelf_1/2 | Point | (5.4, 2.6, 0) / (5.4, 2.6, 2.5) | #FFB45A, 1.5, Range 4 |
| Fire_Cauldron | Point | (3.0, 0.3, 2.5) | #FF7A2A, 1.5, Range 3 |
| Spot_Altar | Spot | (0, 4.2, 0.5) 아래 향함 | 흰색, 3, Angle 45 |
| Spot_Seal | Spot | (0, 4.2, 3.6) 아래 향함 | #C8B4FF, 2, Angle 50 |
- Environment Lighting: Source Color, Ambient #2A2540 정도.
- 실시간 조명만으로 시작하고, 시간이 있으면 Mixed + Bake.

## 5. UI Canvas

`UI_Canvas` [Canvas Screen Space-Overlay, CanvasScaler(Scale With Screen Size, 1920×1080, Match 0.5), GraphicRaycaster, **UIManager**]

| 패널 | 자식 구성 | 기본 활성 |
|---|---|---|
| TitlePanel | 배경 Image(반투명 남색), Title TMP "MagicEscape", Subtitle TMP "마법학교 방탈출", Button_Start("시작"), Button_Quit("종료") | ON |
| HUDPanel | Crosshair(Image 8×8 흰 점, 중앙), PromptText(TMP, 중앙 아래 y=-60, 28pt), ToastRoot(Image 배경, 상단 중앙) › ToastText(TMP 30pt), Hotbar(아래 5.1) | OFF |
| ReadPanel | 반투명 배경, ReadTitle(TMP 40pt), ReadBody(TMP 30pt, 줄바꿈), 안내 TMP "[E] / [ESC] 닫기" | OFF |
| ObjectivePanel | 좌상단, ObjectiveText(TMP 28pt), HintText(TMP 24pt, 노랑) | OFF |
| PausePanel | 어두운 배경, "일시정지", Button_Resume("계속하기"), Button_Restart("다시 시작"), Button_QuitPause("종료") | OFF |
| ClearPanel | "탈출 성공!", ClearTime TMP, ClearMessage TMP, Button_Restart2, Button_Quit2 | OFF |

**UIManager 연결**
- titlePanel, hudPanel, pausePanel, clearPanel, readPanel, objectivePanel = 위 패널
- promptText = PromptText, toastRoot = ToastRoot, toastText = ToastText, toastSeconds = 3
- readTitleText, readBodyText, objectiveText, hintText, clearTimeText, clearMessageText = 각 TMP

**버튼 OnClick (UI_Canvas의 UIManager 지정)**
| 버튼 | 함수 |
|---|---|
| Button_Start | `UIManager.OnClickStart` |
| Button_Resume | `UIManager.OnClickResume` |
| Button_Restart, Button_Restart2 | `UIManager.OnClickRestart` |
| Button_Quit, Button_QuitPause, Button_Quit2 | `UIManager.OnClickQuit` |

**한글 폰트**: Window › TextMeshPro › Font Asset Creator로 Noto Sans KR(또는 Pretendard) SDF 생성(Atlas 4096, Character Set: Custom Range 또는 `GameText.cs` 문자열을 Characters from File로). 모든 TMP의 Font Asset을 이 에셋으로. 기본 LiberationSans는 한글이 □로 나온다.

### 5.1 Hotbar
- `HUDPanel/Hotbar` [Horizontal Layout Group, spacing 8, **HotbarUI**] 하단 중앙
- 자식 Slot_1~Slot_8: 각 Image(배경 64×64) › Icon(Image), Frame(Image 금색 테두리, 비활성)
- HotbarUI: slotIcons = Icon 8개(순서대로), selectionFrames = Frame 8개, selectedNameText = Hotbar 위 TMP

## 6. ItemData 에셋 (ScriptableObject)

`Assets/Data/Items/`에서 Create › MagicEscape › Item Data로 7개 생성.

| 파일 | itemId (정확히) | displayName | 아이콘 |
|---|---|---|---|
| Item_Moonpetal | `ing_moonpetal` | 월광초 꽃잎 | 은빛 꽃잎 |
| Item_SalamanderScale | `ing_salamander_scale` | 불도마뱀 비늘 | 붉은 비늘 |
| Item_CrystalDust | `ing_crystal_dust` | 수정 가루 | 반짝이 병 |
| Item_Toadstool | `ing_toadstool` | 독버섯 | 보라 버섯 |
| Item_CrystalSapphire | `crystal_sapphire` | 푸른 수정 | 파란 결정 |
| Item_CrystalRuby | `crystal_ruby` | 붉은 수정 | 붉은 결정 |
| Item_PotionRevelation | `potion_revelation` | 계시의 물약 | 금빛 물약병 |

> itemId 오타는 퍼즐을 영구히 막는다. `CodeDrafts/Core/ItemIds.cs`에서 복사해 붙여 넣는다.

## 7. 단서 오브젝트

공통: Layer Interactable, BoxCollider(액자 크기), `InspectableObject`.

| 오브젝트 | 위치 (x, y, z) | promptVerb | title | body |
|---|---|---|---|---|
| Painting_III | (-5.88, 1.7, -2.4) 회전 (0,90,0) | 액자 살펴보기 | 씨앗의 일생 III | 뜨거운 햇볕이 새싹을 깨운다. |
| Painting_I | (-5.88, 1.7, -0.8) | 액자 살펴보기 | 씨앗의 일생 I | 씨앗이 흙 속에서 잠든다. |
| Painting_IV | (-5.88, 1.7, 0.8) | 액자 살펴보기 | 씨앗의 일생 IV | 빛 속에서 마침내 꽃이 핀다. |
| Painting_II | (-5.88, 1.7, 2.4) | 액자 살펴보기 | 씨앗의 일생 II | 비가 내려 씨앗을 적신다. |
| Chalkboard | (-5.88, 1.5, -4.0) | 칠판 읽기 | 칠판 | 생명은 정해진 순서로 깨어난다. 액자의 숫자를 따르라. |

- 각 액자 그림 아래에 로마숫자 명판(TMP 3D 또는 텍스처), 그림 모서리에 해당 문양(대지/물/불/빛) 아이콘을 실제로 보이게 배치한다(조사하지 않아도 보이는 단서).
- 잠금 옵션은 모두 OFF.

## 8. 퍼즐 1 — Altar

| 오브젝트 | 설정 |
|---|---|
| Altar | 위치 (0, 0, 0.5). `RunePuzzle`: runeStones = 6개(순서 무관), altarLid = AltarLid, rewardCrystal = Reward_Sapphire, solvedVfx = VFX_RuneSolved, lidOpenOffset (0,0,0.6), lidOpenSeconds 1.5, hintThreshold 3, litColor #4DCCFF, wrongColor #FF2619, solvedColor #FFCC4D |
| AltarBase | Cylinder 스케일 (1.6, 0.5, 1.6) → 높이 1.0, BoxCollider |
| AltarLid | 제단 윗면 중앙 원판(지름 0.5) y=1.0, 아래 빈 공간에 수정이 숨어 있는 형태 |
| RuneStone_X | 제단 윗면 y=1.02, 반지름 0.6 원 위. 각도(북=0°, 시계방향): Light 0°, Shadow 60°, Earth 120°, Wind 180°, Water 240°, Fire 300°. Cylinder 스케일 (0.22, 0.03, 0.22) + 위에 문양 Quad(Emission 텍스처). `RuneStone.rune` 각자 지정 |
| GlowFeedback(각 문양석) | targetRenderer = 문양 Quad의 MeshRenderer, glowLight = (선택) 자식 Point Light(Range 0.6, 비활성), intensity 3 |
| Reward_Sapphire | 제단 중앙 y=1.05, `ItemPickup`: item = Item_CrystalSapphire, promptVerb "줍기", **hiddenAtStart = true** |
| VFX_RuneSolved | 제단 위, 금색 반짝이 1회(Loop OFF, Play On Awake OFF) |

- 문양석의 Collider는 문양석 루트에 둔다(Raycast → `GetComponentInParent<IInteractable>()`).
- 문양 머티리얼: URP/Lit, Emission 체크, Emission Color 검정(코드가 MaterialPropertyBlock으로 제어).

## 9. 퍼즐 2 — Cauldron

| 오브젝트 | 설정 |
|---|---|
| Cauldron | 위치 (3.0, 0, 2.5). `PotionPuzzle`: ingredientPickups = 재료 4개, liquidGlow = Liquid의 GlowFeedback, bubbleVfx/smokeVfx/successVfx 지정, hintThreshold 2, stepColors [#8C99FF, #FF7333, #4DFF80], solvedColor #FFCC4D |
| Liquid | 가마솥 입구 원판, URP/Lit Emission ON. GlowFeedback(targetRenderer = 자신) |
| VFX_Bubble | Loop ON, Play On Awake OFF(코드가 P2 Active 시 재생) |
| VFX_Smoke | 검은 연기 1회, Loop OFF |
| VFX_Success | 금색 반짝이 1회 |
| RecipeBook | 위치 (4.2, 1.1, 3.6), `InspectableObject`: promptVerb "마법서 읽기", title "계시의 물약", body = `GameText.Recipe` 문구, **requirePuzzleSolved = true**, requiredPuzzle = Rune, lockedMessage = `GameText.RecipeSealed` 문구, barrierVisual = Barrier |
| Barrier | 책을 덮는 구(지름 0.7), 반투명 청색 Emission, **Collider 없음**(Raycast가 책에 닿도록) |

**재료 (모두 `ItemPickup`, Collider 필수, Layer Interactable)**
| 오브젝트 | 위치 | item | 비고 |
|---|---|---|---|
| Ingredient_Moonpetal | (5.0, 1.05, -4.2) 창턱 화분 위 | Item_Moonpetal | 화분 자체는 상호작용 없음 |
| Ingredient_SalamanderScale | (5.6, 1.4, 1.5) 책장 선반 | Item_SalamanderScale | 병에 '불꽃' 라벨 |
| Ingredient_CrystalDust | (-3.5, 0.75, -3.25) 반쯤 열린 교탁 서랍 안 | Item_CrystalDust | 서랍 앞판에 별 장식 |
| Ingredient_Toadstool | (-5.4, 0.05, 4.4) 바닥 | Item_Toadstool | 보라색, 눈에 띄게 |
- 모두 hiddenAtStart = false. 오답 시 코드가 원래 Transform으로 되돌린다.

## 10. 퍼즐 3 — SealDevice / ExitDoor

| 오브젝트 | 설정 |
|---|---|
| SealDevice | 위치 (0, 0, 4.2). `SealPuzzle`: revealedInscription = Inscription/RevealedText, inscriptionGlow = Inscription의 GlowFeedback, leftSocket = Socket_Left, rightSocket = Socket_Right, coreGlow = SealCore의 GlowFeedback, revealVfx, releaseVfx, hintThreshold 2, readyColor #CC99FF, releasedColor #FFD966 |
| Pedestal | Cube (1.8, 1.0, 0.5), y=0.5 |
| Inscription | 받침대 윗면 뒤쪽 비스듬한 석판 (1.2 × 0.5), y≈1.15. `SealInscription.puzzle` = SealDevice. RevealedText(TMP 3D "왼쪽엔 흐르는 것을, 오른쪽엔 타오르는 것을.") + 왼쪽 물 문양 Quad, 오른쪽 불 문양 Quad — **RevealedText는 비활성으로 시작**(코드도 Awake에서 끔) |
| Socket_Left | (-0.6, 1.05, 4.2) 받침대 위 왼쪽 컵 모양. `CrystalSocket`: side = Left, placedCrystalVisual = 자식 PlacedCrystal(푸른), glow = 자신의 GlowFeedback |
| Socket_Right | (0.6, 1.05, 4.2). side = Right, placedCrystalVisual = 자식 PlacedCrystal(붉은) |
| SealCore | (0, 1.15, 4.2) 구(지름 0.25). `SealCore.puzzle` = SealDevice. GlowFeedback(glowLight = 자식 Point Light) |
| ExitDoor | 위치 (0, 0, 5.0). `DoorController`: hinge = DoorHinge, openAngle -100, openSeconds 2, openDelay 1, sealLights = [SealLight_1, SealLight_2, SealLight_3], exitTrigger = ExitTrigger, openVfx = VFX_DoorOpen |
| DoorHinge | 로컬 (-0.8, 0, 0) — 문 왼쪽 경첩 위치. DoorPanel은 그 자식으로 로컬 (0.8, 1.3, 0), 스케일 (1.6, 2.6, 0.1) |
| SealLight_1~3 | 문 위 (-0.5, 3.0, -0.1), (0, 3.0, -0.1), (0.5, 3.0, -0.1). 빨간 Emission 구 + 자식 Point Light(#FF3030, 1, Range 2). **켜진 상태로 시작** |
| ExitTrigger | 로컬 (0, 1.25, 1.0), BoxCollider isTrigger, Size (2, 2.5, 1). `ExitTrigger` 컴포넌트. **비활성으로 시작**(DoorController.Awake도 끔) |

- 문 바깥(z>5)에 빛나는 복도 바닥/평면을 두어 문이 열렸을 때 '밖'이 보이게 한다.
- 열린 문이 Wall_N이나 SealDevice에 겹치지 않는지 openAngle 부호(±100)를 확인한다.

## 11. 프리팹 목록

씬에서 하나 완성 → Project로 드래그해 Prefab화.

| 프리팹 | 루트 컴포넌트 | 비고 |
|---|---|---|
| `PF_Player` | Player 일체 | 카메라 포함 |
| `PF_RuneStone` | RuneStone, GlowFeedback, Collider | 6개 배치 후 rune·문양 텍스처만 변경 |
| `PF_ItemPickup` | ItemPickup, Collider | 재료·수정 공용, item만 교체 |
| `PF_Painting` | InspectableObject, Collider | 4개, title/body만 교체 |
| `PF_CrystalSocket` | CrystalSocket, GlowFeedback | 좌/우 |
| `PF_SealLight` | Emission 구 + Light | 3개 |
| `PF_UI_Canvas` | UIManager | 선택(씬 하나뿐이면 생략 가능) |
- 프리팹 내부 참조(예: RunePuzzle.runeStones)는 씬 인스턴스에서 연결한다.

## 12. 스크립트 참조 관계 체크리스트

| 컴포넌트 | 필수 참조 | 누락 시 증상(Console) |
|---|---|---|
| GameManager | player, runePuzzle, potionPuzzle, sealPuzzle, exitDoor | `[GameManager] … 미지정` Error |
| PlayerController | cameraPivot | Warning 후 자식 카메라 사용 |
| PlayerInteraction | viewCamera | Error, 상호작용 불가 |
| InventoryManager | itemCatalog 7개 | 아이콘 없음 + `카탈로그에 없는 itemId` Warning |
| RunePuzzle | runeStones 6, rewardCrystal | Error |
| PotionPuzzle | ingredientPickups 4 | 오답 시 `재생성할 ItemPickup을 찾지 못함` |
| SealPuzzle | leftSocket, rightSocket | Error |
| SealInscription / SealCore | puzzle | Error(상호작용 시) |
| DoorController | hinge, sealLights 3, exitTrigger | Error / 탈출 불가 |
| UIManager | 패널 6, TMP 9 | 해당 UI만 미표시(토스트는 Console에 `[Toast]`) |
| HotbarUI | slotIcons 8, selectionFrames 8 | 아이콘 미표시 |
| ItemPickup | item | Error |

## 13. Play Mode 검증 절차(구성 단계별)

1. **플레이어만**: skipTitle=true로 Play → WASD 이동, 마우스 시점, 벽 충돌, 커서 잠금 확인.
2. **UI**: skipTitle=false → 타이틀 표시·커서 보임 → 시작 → HUD. ESC로 일시정지/계속.
3. **단서**: 액자 조준 시 `[E] 액자 살펴보기` → 읽기 패널 → E로 닫힘, 열린 동안 이동 불가.
4. **P1**: 오답 1회 → 빨간 점멸·토스트. 정답 → 뚜껑 열림, 수정 등장, 봉인 빛 1 소등, 마법서 결계 사라짐.
5. **P2**: P1 전 가마솥 → 차갑다는 메시지. 재료 수집 → 슬롯 선택 → 정답/오답 경로 확인(오답 시 재료 원위치).
6. **P3**: 물약 → 비문 → 수정 반대로 → 거부 → 정상 → 봉인핵 → 문 개방 → 문 밖 → 클리어 화면 시간 표시.
7. **재시작**: 클리어/일시정지에서 다시 시작 → 모든 상태 초기화 확인.
8. 각 단계마다 Console에 Error 0건.

세부 케이스는 [TEST_PLAN.md](TEST_PLAN.md).
