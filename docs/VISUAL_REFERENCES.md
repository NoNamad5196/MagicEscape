# MagicEscape — 비주얼 방향 · 에셋 · 레퍼런스

> 조사일: 2026-11-10 · 조사 도구: Perplexity MCP(Unity 버전 확인), Exa 웹 검색(에셋 페이지·라이선스 원문 확인). Perplexity deep 검색은 이 날 시간 초과로 실패해 에셋은 웹 검색으로 원문 페이지를 직접 확인했다.
> 원칙: **무료 에셋만** 사용, 유료 구매 없음. 핵심 시스템·퍼즐 로직은 직접 구현하고 에셋은 외형에만 쓴다. 라이선스를 원문에서 확인하지 못한 항목은 "확인 필요"로 표시했다.

## 1. 아트 방향

**키워드**: 아늑함 · 신비로움 · 저녁 무렵의 마법 실습실 · 따뜻한 촛불 vs 차가운 달빛 · 빛나는 문양

- **스타일**: 저폴리 스타일라이즈드(KayKit/Quaternius 계열). 리얼리스틱 PBR보다 에셋 간 톤을 맞추기 쉽고 성능 부담이 적다.
- **명암 구조**: 방 전체는 어둡게(남보라 앰비언트), 퍼즐 오브젝트만 스포트·발광으로 밝혀 시선을 유도한다. "빛나는 것 = 상호작용 대상" 규칙.
- **색의 의미 고정**(퍼즐 피드백에 그대로 사용 — 코드 기본값과 동일)

| 의미 | 색 | Hex | 사용처 |
|---|---|---|---|
| 기본 목재/가구 | 따뜻한 갈색 | `#6B4A2F` / `#3E2A1C` | 바닥·책장·책상 |
| 촛불/램프 | 호박색 | `#FFB45A` | Point Light |
| 달빛/앰비언트 | 차가운 청보라 | `#8FA8FF` / `#2A2540` | Directional, Ambient |
| 진행/정답 입력(물·청) | 시안 | `#4DCCFF` | 문양 점등, 소켓 |
| 오답/봉인 | 빨강 | `#FF2619` / `#FF3030` | 점멸, 봉인 빛 |
| 완료/해제 | 금색 | `#FFCC4D` / `#FFD966` | 퍼즐 해결 |
| 비문/봉인핵 대기 | 연보라 | `#CC99FF` | P3 |
| 물약 단계 | 청 → 주황 → 초록 | `#8C99FF` `#FF7333` `#4DFF80` | 가마솥 |

- **포스트 프로세싱(URP Volume)**: Bloom(Threshold 0.9, Intensity 1.2) — 발광 문양이 번지게, Vignette 0.25 — 시선 집중, ACES Tonemapping, 약한 따뜻한 Color Filter.
- **상태별 시각 규칙**

| 상태 | 표현 |
|---|---|
| Locked | 발광 없음 또는 결계(반투명 청색 구), 차가운 가마솥(거품 없음) |
| Active | 은은한 스포트 조명, 가마솥 거품 파티클 |
| 정답 입력 | 해당 부위 시안 발광 + 맑은 톤 |
| 오답 | 0.6초 빨간 점멸 / 검은 연기 + 둔탁한 소리 |
| Solved | 금색 발광 + 반짝이 파티클 + 출구 봉인 빛 1개 소등 |

## 2. 레퍼런스 게임/작품

| 작품 | URL | 참고할 요소 | MagicEscape 적용 |
|---|---|---|---|
| The Room 시리즈 (Fireproof Games) | https://www.fireproofgames.com/games/the-room | 장치 하나에 여러 단계가 숨어 있는 구조, 조작 시 기계적 피드백 | 봉인 장치(P3)를 비문→소켓→봉인핵 3단계로 구성 |
| Escape Simulator (Pine Studio) | https://store.steampowered.com/app/1435790/Escape_Simulator/ | 1인칭 방탈출의 줍기·조사·조합 UX, 짧은 플레이 단위 | E 상호작용 + 핫바 선택 사용 방식 |
| Hogwarts Legacy (Avalanche) | https://www.hogwartslegacy.com/ | 마법학교 교실의 따뜻한 조명, 책장·촛대·마법 도구 밀도 | 촛불 앰비언트 + 소품 배치 밀도 기준 |
| Potion Craft: Alchemist Simulator | https://store.steampowered.com/app/1210320/Potion_Craft_Alchemist_Simulator/ | 재료 투입 시 물약 색 변화로 진행 피드백 | 가마솥 단계별 색 변화 |
| Myst / riven 계열 관찰형 퍼즐 | https://cyan.com/games/myst/ | 환경 단서(그림·문자)를 읽어 순서 추론 | 액자 4점 + 로마숫자로 문양 순서 추론(P1) |
| KayKit 'Dungeon Pack' 샘플 씬 | https://kaylousberg.itch.io/kaykit-dungeon-pack | 저폴리 판타지 실내 톤·조명 예시 | 전체 아트 톤 기준 |

(레퍼런스는 분위기·UX 참고용이며 어떤 이미지·에셋도 복제하지 않는다.)

## 3. 무료 에셋 후보

| # | 이름 | 출처 URL | 종류 | 라이선스 | Unity 6 / URP | 난이도 | 비고 |
|---|---|---|---|---|---|---|---|
| 1 | KayKit Dungeon Pack (Free) | https://kaylousberg.itch.io/kaykit-dungeon-pack | 벽·바닥·문·책장·테이블·촛대 200+ | CC0 1.0 (원문 확인) | FBX(Unity용 FBX 포함), 단일 아틀라스 → URP/Lit로 머티리얼 재지정 | 하 | 방 구조·문의 1순위. EXTRA/SOURCE는 유료 → 사용 안 함 |
| 2 | KayKit Furniture Bits (Free) | https://kaylousberg.itch.io/furniture-bits | 가구 50+ | CC0 1.0 (원문 확인) | 동일 | 하 | 책상·의자·선반 |
| 3 | Quaternius Fantasy Props MegaKit (Standard 무료판) | https://quaternius.itch.io/fantasy-props-megakit | 가마솥·물약병·책·촛대·두루마리·랜턴 | CC0 1.0 (원문 확인) | FBX/glTF. 무료판은 URP 프로젝트 미포함 → 머티리얼 수동 지정 | 하 | 가마솥(Cauldron), Potion 1~4, BookStand(마법서 독서대) |
| 4 | Quaternius Cauldron (Poly Pizza) | https://poly.pizza/m/QaWJOPa6Gt | 가마솥 단일 모델 | CC0 (Public Domain 표기) | FBX/glTF | 하 | 3번 대체 |
| 5 | KayKit Halloween Bits | https://kaylousberg.itch.io/ (카탈로그) | 버섯·병·호박 등 | CC0 표기(개별 페이지 확인 필요) | FBX | 하 | 독버섯 모델 후보 — 확인 필요 |
| 6 | Poly Haven 텍스처/HDRI | https://polyhaven.com/ | 나무·돌 PBR 텍스처 | CC0 | URP Lit에 바로 사용 | 중 | 바닥·벽에 사실적 질감이 필요할 때만 |
| 7 | ambientCG | https://ambientcg.com/ | PBR 텍스처 | CC0 | URP Lit | 중 | 6번 대체 |
| 8 | Cartoon FX Remaster Free (Jean Moreno) | https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565 | 파티클(반짝임·연기·마법) | Standard Unity Asset Store EULA, 무료 | Built-in + URP 지원, Unity 6 호환 명시. URP에서 안 보이면 Depth Texture ON | 하 | 성공 반짝이, 검은 연기, 봉인 해제 |
| 9 | Unity Starter Assets – FirstPerson | https://assetstore.unity.com/packages/essentials/starter-assets-firstperson-updates-in-new-charactercontroller-pa-196525 | 1인칭 컨트롤러 샘플 | Unity Asset Store EULA, 무료 | 원래 Built-in, URP 업그레이드 가능 | 중 | **우리는 자체 PlayerController 사용**. 비교·참고용만 |
| 10 | Kenney RPG Audio | https://kenney.nl/assets/rpg-audio | 책 넘김·문 열림/닫힘·발소리·금속 딸깍 51종 | CC0 | OGG → Unity 임포트 | 하 | DoorOpen, 마법서, SocketPlace(metalClick) |
| 11 | Kenney Impact Sounds | https://kenney.nl/assets/impact-sounds | 충돌·두드림 | CC0 | OGG | 하 | RunePress(돌), RuneWrong |
| 12 | Kenney Interface Sounds | https://opengameart.org/content/interface-sounds | UI 클릭·확인 100종 | CC0 (원문 확인) | OGG | 하 | UiClick, Pickup, Interact |
| 13 | Kenney Music Jingles | https://kenney.nl/assets/music-jingles | 짧은 성공 징글 85종 | CC0 (OpenGameArt 'All CC0 – Kenney' 목록) | OGG | 하 | RuneSolved, PotionSolved, Clear |
| 14 | freesound.org (CC0 필터) | https://freesound.org/search/?f=license:%22Creative+Commons+0%22 | 거품·마법 웅- 소리 | 개별 CC0 확인 필요 | WAV/OGG | 중 | CauldronAccept(bubbling), SealReveal |
| 15 | OpenGameArt 판타지 BGM | https://opengameart.org/art-search-advanced?field_art_type_tid%5B%5D=12 | 루프 BGM | 곡별 상이(CC0/CC-BY) — 확인 필요 | OGG | 중 | CC-BY면 크레딧 필수 |
| 16 | Pretendard | https://github.com/orioncactus/pretendard | 한글 UI 폰트 | SIL OFL 1.1 (원문 확인) | TMP Font Asset 생성 | 하 | 본문·UI 1순위 |
| 17 | Noto Sans KR | https://fonts.google.com/noto/specimen/Noto+Sans+KR | 한글 폰트 | SIL OFL 1.1 | TMP Font Asset | 하 | 16번 대체 |
| 18 | game-icons.net | https://game-icons.net/ | 아이템·문양 아이콘(SVG/PNG) | CC BY 3.0 (작가별 크레딧 필수) | PNG → Sprite | 하 | 핫바 아이콘, 문양 기호(대지·물·불·바람·빛·그림자) |
| 19 | Kenney UI Pack (RPG Expansion) | https://kenney.nl/assets/ui-pack-rpg-expansion | 패널·버튼 스프라이트 | CC0 | Sprite(9-slice) | 하 | 일시정지·클리어 패널 |

**1차 추천 조합(전부 무료)**: 1·2(방/가구) + 3(가마솥·물약·책) + 8(VFX) + 10·11·12·13(사운드) + 16(폰트) + 18(아이콘).

## 4. 에셋 없이 만드는 대체안 (그레이박스 = 최종 백업)

외부 에셋을 하나도 못 써도 게임이 완성되도록 한다.

| 오브젝트 | Unity 기본 도형 구성 | 머티리얼 |
|---|---|---|
| 방 | Cube 벽·바닥·천장 | URP/Lit 나무색, Smoothness 0.2 |
| 제단 | Cylinder(1.6,0.5,1.6) + 얇은 Cylinder 뚜껑 | 회색 돌 |
| 문양석 | 얇은 Cylinder + 위에 Quad(문양 텍스처) | Quad: URP/Lit, Base 흰색, **Emission ON**, 문양 텍스처는 TMP로 글자(土·水·火·風·光·影) 또는 game-icons PNG |
| 가마솥 | Sphere(0.9) 아래 절반 + Cylinder 테두리, Liquid = 얇은 Cylinder | 검정 금속 / Liquid Emission ON |
| 재료 | 꽃잎=납작 Capsule(은색), 비늘=Quad(빨강), 가루=작은 병(Capsule), 독버섯=Cylinder+반구(보라) | 색으로 구분 |
| 수정 | 회전한 Cube 2개 겹침 또는 Capsule 늘림 | 파랑/빨강 Emission 약하게 |
| 봉인 장치 | Cube 받침대 + Quad 비문판 + 컵 모양 Cylinder 소켓 2 + Sphere 봉인핵 | 돌 + 연보라 Emission |
| 문 | Cube 문짝 + DoorHinge 빈 오브젝트 | 짙은 나무 |
| 봉인 빛 | Sphere(0.15) + Point Light | 빨강 Emission |
| 결계 | Sphere(0.7) | URP/Unlit Transparent 청색 알파 0.25 |
| 파티클 | Unity 기본 Particle System(Shape Cone, 크기 0.05, 금색/검은색) | Default-Particle |

## 5. 라이선스·크레딧 규칙

- CC0: 표기 의무 없음. 그래도 PPT 마지막 장과 빌드 크레딧(README)에 출처를 적는다.
- Unity Asset Store EULA(무료 에셋): 게임 빌드에 포함 가능, **에셋 원본 재배포 금지** → GitHub 공개 저장소에 Asset Store 원본 파일을 커밋하지 않는다. (`UnityProject/Assets/JMO Assets/` 등은 `.gitignore`에 추가하고 Windows 로컬에서만 임포트하는 방식을 권장. 저장소가 비공개면 팀 결정)
- CC BY 3.0(game-icons.net, 일부 BGM): 작가명·출처·라이선스를 크레딧에 반드시 표기.
- OFL 폰트: 폰트 파일을 단독 판매하지 않으면 자유. 빌드 포함 가능, 라이선스 파일 동봉.
- 크레딧 표 템플릿(README·PPT 공용):

| 분류 | 에셋 | 작가 | 라이선스 | URL |
|---|---|---|---|---|
| 3D | KayKit Dungeon Pack | Kay Lousberg | CC0 | … |
| VFX | Cartoon FX Remaster Free | Jean Moreno | Unity Asset Store EULA | … |
| 사운드 | RPG Audio | Kenney | CC0 | … |
| 폰트 | Pretendard | Kil Hyung-jin | SIL OFL 1.1 | … |
| 코드·퍼즐 설계 | MagicEscape 시스템 전체 | 본인(+AI 보조) | — | 저장소 |

- **명시 문구(PPT)**: "모델·텍스처·효과음·폰트는 위 무료 에셋을 사용했으며, 상호작용·인벤토리·퍼즐 3종 상태 머신·진행 관리·UI 로직은 직접 설계·구현했다."

## 6. 출처 (조사일 2026-11-10)

- KayKit Dungeon Pack: https://kaylousberg.itch.io/kaykit-dungeon-pack (CC0, 2026-07-16 Dungeon Remastered → Dungeon Pack 개명)
- KayKit Furniture Bits: https://kaylousberg.itch.io/furniture-bits (CC0)
- Quaternius Fantasy Props MegaKit: https://quaternius.itch.io/fantasy-props-megakit (CC0)
- Cartoon FX Remaster Free: https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565 (FREE, Standard EULA, URP·Unity 6 호환 명시, R 1.5.1 / 2025-12-18)
- Cartoon FX URP Depth Texture 안내: Readme Cartoon FX Remaster FREE (패키지 동봉 문서)
- Starter Assets 소개: https://unity.com/blog/games/say-hello-to-the-new-starter-asset-packages
- Kenney Interface Sounds(CC0): https://opengameart.org/content/interface-sounds
- Kenney RPG Audio(CC0): https://kenney.nl/assets/rpg-audio
- Kenney Impact Sounds: https://kenney.nl/assets/impact-sounds
- Pretendard(OFL 1.1): https://github.com/orioncactus/pretendard/blob/main/LICENSE
- Unity 6.3 LTS: https://discussions.unity.com/t/unity-6-3-lts-is-now-available/1697328

## 7. S10 실제 사용 및 수동 다운로드 목록 (2026-11-12 KST)

| 사용 | 로컬 파일 | 출처/라이선스 | Git |
|---|---|---|---|
| 테이블·선반·의자·양초 | Assets/ThirdParty/KayKitDungeon: table_medium, shelf_small, chair, candle_lit, candle_triple FBX + dungeon_texture.png + LICENSE.txt | [KayKit Dungeon](https://kaylousberg.itch.io/kaykit-dungeon-pack), Kay Lousberg, CC0. 기존 인계 파일의 동봉 라이선스는 Dungeon Remastered 1.0; 현 배포 페이지도 CC0임을 재확인. 유료 EXTRA/SOURCE 미사용 | 필요한 파일/Unity meta만 커밋 |
| SfxId 15종 연결 | Assets/ThirdParty/KenneyInterface OGG 12종 + License.txt | [Interface Sounds](https://opengameart.org/content/interface-sounds), Kenney, CC0. 동봉 License.txt 확인. 이번 웹 조회는 접근 제한으로 재검증 불가 | 사용하는 OGG/Unity meta만 커밋 |
| 가마솥 시각 메시·반짝임·검은 연기·봉인 해제 | ClassroomArtBuilder.S10에서 Unity API로 생성/설정 | 프로젝트 자체 제작. 외부 VFX 원본 사용 없음 | 생성 메시/텍스처/머티리얼과 빌더 커밋 |

- **사용자가 받아야 할 목록(선택 사항)**: [Cartoon FX Remaster Free](https://assetstore.unity.com/packages/vfx/particles/cartoon-fx-remaster-free-109565), Jean Moreno, 무료/Standard Unity Asset Store EULA. 로그인/My Assets 흐름이 필요한 후보라 이번에는 건너뜀. 사용자는 Unity 계정으로 추가 후 Package Manager > My Assets > Download/Import. `.gitignore`의 JMO Assets/ThirdParty/CartoonFX 경로에 로컬 보관, 원본 커밋 금지. 없어도 자체 제작 VFX로 동작한다. 현재 페이지에서 FREE/EULA/URP 호환 확인(2026-11-12).
- Quaternius와 다른 §3 후보는 미사용·미다운로드. 유료 구매, XR/VR 패키지 추가, BGM 추가 없음. Sfx는 인터페이스 팩 대체음이므로 문/가마솥 음색·음량은 수동 청취 필요.
- S10은 S9부터 재생성한다. FBX는 MeshFilter/Renderer만 복사하며 원본 스크립트/콜라이더를 가져오지 않는다. 파일이 없으면 기존 그레이박스 메시를 유지하고 누락 오디오를 null로 건너뛴다. 선택 Asset Store 패키지에 대한 코드 의존성 없음.
- URP Depth Texture/HDR, Bloom/ACES/약한 Vignette, 촛불·창빛, 문양·봉인·액체 Emission 유지. 그림자는 끈 상태로 비용을 제한했다.

## 8. S10b second art pass (2026-11-12 KST)

- Poly Haven [Stone Wall 02](https://polyhaven.com/a/stone_wall_02), [Wood Floor](https://polyhaven.com/a/wood_floor): CC0, 1K diffuse JPG only. Source https://polyhaven.com/license; downloaded using public files API. Metric cube UVs use 2m/repeat, including narrow wall sections. Ceiling uses dark timber plus mesh-only beams.
- Downloaded archives stay in ignored Tools/asset-cache; only selected files enter ThirdParty. No Asset Store originals.
- Previous untracked exit-resume crash/report removed: they contained licensing-handshake/session discussion and local crash details, so were not committed. Existing untracked XML reports left untouched.

- [Quaternius Fantasy Props MegaKit Standard](https://quaternius.com/packs/fantasypropsmegakit.html) / [official download](https://quaternius.itch.io/fantasy-props-megakit): CC0, bundled License_Standard.txt. Selected 11 FBX: Bookcase_2, BookGroup_Medium_1, Book_Stack_1, BookStand, Book_5, Cauldron, Potion_1/2, Scroll_1, Shelf_Small_Bottles, Lantern_Wall. Three used base-colour trim atlases (Furniture/Metal/Props) downsampled 2048?1024; unused Cloth atlas removed. No Pro/Source files. MeshRenderer material names select original Furniture/Props/Metal/Cloth atlas.
- KayKit Dungeon Pack 1.1 FREE: pillar_decorated, banner_patternA_blue, table_small (Unity FBX), bundled License_1.1.txt. Existing 1.0 furniture retained. All added models copied as mesh/renderer-only hierarchy, with world-space fitting before parenting. Self-authored cauldron mesh remains the S10 fallback.

- Original project art in Assets/Art/S10b: six rune icons (Earth/Water/Fire/Wind/Light/Shadow), four storybook panels (seed/rain/sun/flower), magic circle and chalkboard grain. Reproducible with `python Tools/Generate-Art.py` (Pillow), 256/512px, no third-party icon or font input. Roman plates I?IV and matching corner rune symbols retain original order III/I/IV/II. Chalkboard text uses GameText.Chalkboard directly.

- New KayKit 1.1 models use `dungeon_texture_1_1.png`; existing S10 1.0 models keep their original atlas. Window frame is original mesh-only timber trim using Poly Haven wood. New bookcase rows contain BookGroup_Medium_1 meshes, not empty shelves.
- No S10b asset requires user login/manual download: both official free itch.io archives were downloadable. Furniture Bits was unnecessary (Dungeon + Quaternius cover the furniture). The previous optional Cartoon FX download remains optional and was not used.
- Exact new payload sizes/file list: [asset inventory](validation/s10b/asset-inventory.json), excludes Unity .meta. All selected CC0 originals, licenses, authored PNGs and Unity-generated meta are committed; full ZIP archives are not.

## 9. S10c 3차 아트 패스 — 2026-11-12

거의 검정인 푸른 ambient, 제한된 따뜻한 빛 웅덩이, 차가운 창빛을 기준으로 조정했다. 실시간 그림자는 플레이어 랜턴과 창문 스포트 두 개만 사용한다. 기존 CC0 가구 모델만 재사용했으며 새 외부 3D/텍스처/아이콘은 없다.

| 신규 사운드 | 제작자 | 원문 / 라이선스 | 프로젝트 파일 / 가공 |
|---|---|---|---|
| wind1 | Luke.RUSTLTD | [OpenGameArt](https://opengameart.org/content/wind1), CC0 1.0 | AtmosphereCC0/wind.wav, 900 Hz 저역 통과, 46초 순환 루프 |
| ambient sound creepy | DaGameKnower | [OpenGameArt](https://opengameart.org/content/ambient-sound-creepy), CC0 1.0 | AtmosphereCC0/drone.wav, 220 Hz 저역 통과로 저음만 사용, 46초 순환 루프 |
| Wood Creak Single V1 | Rudmer_Rotteveel | [Freesound](https://freesound.org/people/Rudmer_Rotteveel/sounds/502505/), CC0 1.0 | 공개 HQ MP3 preview를 creak.wav로 변환, 약 0.987초 |

- 세 원문 페이지의 CC0 표기 및 [CC0 법률 원문](https://creativecommons.org/publicdomain/zero/1.0/legalcode.txt)을 직접 확인했다. 원문 전체는 `Assets/ThirdParty/AtmosphereCC0/CC0-1.0.txt`, 출처·가공 정보는 동 폴더 `CREDITS.md`와 [전체 크레딧](CREDITS.md)에 있다. 타이틀 화면 하단에도 제작자를 표기한다.
- 22050 Hz/16-bit/mono PCM. 드론·바람은 최초 48초의 양끝 2초를 오버랩해 루프 이음매를 완화했다. 원본/HTML은 ignored `Tools/asset-cache/atmosphere`에만 보관한다. 재생 음량은 각각 0.075/0.10, 삐걱임 0.095. 새 오디오의 기본 재생 경로는 RoomAmbience이며 퍼즐 SfxId를 변경하지 않는다.
- `Tools/Generate-Atmosphere-Art.py`: 직접 만든 진홍색 직물+금색 문장·테두리, 별/초승달 밤하늘. 외부 그림·글꼴 입력 없음. `ClassroomThirdArtBuilder.BakeItemIcons`: 같은 아이템 모델·메시를 카메라로 촬영한 투명 256px PNG 7장. 별도 아이콘 라이선스 부담 없음.
- 기존 Quaternius Potion_1/2를 재료 병과 물약에 재사용. 꽃잎/버섯/6면 수정과 작은 수정군은 Editor API로 제작했다. 사파이어의 푸른색, 루비의 붉은색을 유지했다. 소켓·원래 상호작용 콜라이더는 유지한다.
- ACES, Bloom 0.55, Vignette 0.33, Film Grain 0.075, Chromatic Aberration 0.012, 청록 그림자/주황 하이라이트 Split Toning, Saturation -12, 청록 Exponential Fog 0.023. URP HDR/Depth/Soft Shadows 활성화; 추가 그림자 두 개는 Medium/1024 atlas로 제한한다.


## S10d ? ?? ?? ??? / 2026-11-12

### ??? ??? ??
- ?? ???? **??? ??? ???**? ??. ??? ?? ??/??? ??, ??? ?? ?, ?? ??, ??? ??, ?? ???? ??, ???? ??? `Tools/Generate-Art.py`? ?? ??? ???. ????? ?? ??? ?? ??? ??? ????.
- ?? ??? ? ? ?? / ? / ??? ?? / ? ? ??? ??. ?? ??? Earth/Water/Fire/Light? ? ?? **III, I, IV, II**? ?? ?? ????. ??? ?? ?? ?? ??? Art/S10b? ?????.
- UI? ??? ?? ?? ?? ?, ??? ?? ?? ???. ?? ??? ??? ???? 3D ???? ??? ???. ?????? ?? ??? ?. E ?? ?? ??? TMP rich text? ????.
- ?? ?????? ??? ??? ?? UV? ???? ????, ?? ??/??/???? ?? ???. ????? ??? ?? ???? ??. ??? ??? ???? ???? ??? ?? ?? ???? ???? ??(?? ??/?? ???? ??).
- ?? ??/URP ??????? ??. ?? ?? ???, ??? ?? ??, ?? ?? ???? ??. ??? ?? ?? ??? ? ??. ? ??/??? ?? ??.

### ??????? ?? ??
- [Gowun Batang OFL ??](https://raw.githubusercontent.com/google/fonts/main/ofl/gowunbatang/OFL.txt): Copyright 2021 The Gowun Batang Project Authors, SIL OFL 1.1. UI/??/?? ??.
- [Nanum Pen Script OFL ??](https://raw.githubusercontent.com/google/fonts/main/ofl/nanumpenscript/OFL.txt): Copyright 2010 NHN Corporation, SIL OFL 1.1, Nanum ?? Reserved Font Names ??. ?? ??.
- OFL? ?? ?? ?? ??, ???/???? ??, Reserved Font Name ??? ???? ????. TTF ??? ???? ???. ???? ?? ??? ? TTF ? `*-OFL.txt`; ???? ? ???? ????.
- `ClassroomFourthArtBuilder.OldFont`? GameText? public static string ?? ??? ????, ? ??/Runtime?Core ??/ASCII? ?? SDF? ???. ?? Dynamic atlas? TestRunner ???? ??? ? ??? ??? ??? ????. ???? Static atlas + clear-on-build ??? ????, GameText? fallback ?? ??? ????. ??? ??? `validation/s10d/*-glyph-audit.txt`.

### ?? ?? ?? / ?? ?? ???
- Quaternius Standard FREE ZIP `License_Standard.txt`? CC0 1.0 ??. Crate_Wooden, Candle_2, Scroll_2, Book_7? ??? ??. ?? 11? + KayKit ??/?? ???. ????? ?? ThirdParty/Quaternius/License_Standard.txt ??. ?? PRO/SOURCE ?? ?? ??.
- ?? ?? ??? S10c 33? ? S10d 72?(? **2.18?**). ? 39? ?? mesh/renderer? ???? ????/????? ???? ???. ?? ??, ?? ??/??, ??????????? ?? ?? ?? ?? ??.
- ?? ??? ??/?? ??: `validation/s10d/REVIEW.md`.

## S10f — 중세 연금술 사본/목판화 (2026-11-12)
- 시각 기준: 16세기 연금술 목판화의 세피아 잉크, 해칭, 이중 장식 테두리. 역사 원본 이미지를 복제/다운로드하지 않았으며 그림은 `Tools/Generate-Alchemy.py` 자체 제작이다. Splendor Solis / Rosarium philosophorum은 방향 참고 명칭일 뿐 원본 에셋 사용을 주장하지 않는다.
- 기호: Fire U+1F702 / Water U+1F704 / Wind U+1F701 / Earth U+1F703 / Light U+2609 / Shadow U+2644. 모든 문맥에서 동일 7/256 선 굵기, 낡은 황동, 저채도. 실제 `NotoSansSymbols[wght].ttf` cmap 조사에서 U+2609가 없고 나머지 5개는 존재함을 확인. 통일된 선 굵기를 위해 6종 모두 독자 벡터로 작성(Shadow는 토성 십자+낫). 글리프 원본을 래스터화한 것이 아니다.
- Noto 원본/라이선스: https://github.com/google/fonts/tree/main/ofl/notosanssymbols ; https://raw.githubusercontent.com/google/fonts/main/ofl/notosanssymbols/OFL.txt — SIL OFL 1.1 원문 직접 확인/보관. `Tools/FontSource/NotoSansSymbols/{NotoSansSymbols.ttf,OFL.txt,glyph-audit.json}`. 게임에 새 폰트를 임베드하지 않음; 기존 고운바탕 유지.
- 불 플립북: Unity Labs Paris, Thomas Iché, Flame02 / `Flame02_16x4.tga` 64프레임. 공식 다운로드/CC0 고지 직접 확인: https://unity.com/blog/engine-platform/free-vfx-image-sequences-flipbooks ; https://unity3d.com/files/labs/downloads/vfx/assets01/Flame02/Flame02-flipbooks.zip . RGBA PNG 무손실 변환, 2048×1024. 게임 파일 `UnityProject/Assets/Art/S10f/Flame02.png`. Asset Store 사용 없음. 라이선스 원문: https://creativecommons.org/publicdomain/zero/1.0/legalcode .
- 물 노멀: 외부 자산 아님. 사인 합성 높이장의 기울기로 만든 자체 256×256 타일 노멀 (`WaterNormal.png`). ambientCG 후보 다운로드 Water002는 404이므로 사용하지 않음. 물과 가마솥은 같은 스크롤 노멀/프레넬/환경반사/표면 높이 셰이더, 색만 다름. 실제 scene-depth 기반 굴절 및 열 왜곡은 미구현(오버드로 절제).
- 재현: 최초 실행에는 `Tools/asset-cache/s10f/Flame02/Flame02_16x4.tga`, Noto 원본이 필요하며, 캐시는 Git 제외. 커밋된 원본 폰트와 게임 PNG는 보존된다.
