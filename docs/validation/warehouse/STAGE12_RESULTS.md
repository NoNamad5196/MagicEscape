# Warehouse Stage 12 — 시각 품질 재검수

## 범위와 보존 계약

A와 B1~B8을 Warehouse 전용 빌더/에셋에서 수정했다. 정답 Earth → Water → Fire → Light, RuneId 순서, 아이템 ID, 월광초 `(3.75,.78,-2.94)`, 나머지 재료 홈, 치우기 게이트, XRI 공용 입력, 기존 콜라이더와 3/2/2 실패 힌트 기준은 유지한다. `.unity`/`.asset`/`.mat`/`.meta`는 Unity가 생성·저장했으며 YAML을 수작업 편집하지 않았다.

## 변경

| 요청 | 반영 |
|---|---|
| A / 카드 efc8585a | 네 장 모두 창가 작업대·평평한 토분·기름등의 동일 구도로 교체. I 흙/씨앗, II 물주기, III 밤의 수직 심지 불꽃, IV 창의 사선 햇빛/작은 싹. 액자는 세피아 잉크와 저채도 수채, 칠판은 성긴 분필 외곽선. 원소/글자/숫자를 그림 안에 생성하지 않았다. |
| B1 | 6개 문양석은 얼룩 있는 회녹색 석재·깎인 모서리·광물 상감. 그림문자와 이름의 빈 픽셀을 잘라 검은 아이콘 사각형을 제거. `_Fill`/발광 피드백은 그대로 연결. 같은 처리를 액자 명판·P3·문 위 봉인등에도 적용. |
| B2 | 초승달 표시 이름은 달. `RuneId.Shadow` 및 enum/authoring key는 그대로. Warehouse 전용 이름 마스크로 역사적 Classroom 그림은 바꾸지 않음. |
| B3 | P1/P2/P3 성공 설명 토스트와 P2 정상 재료 투입의 진행 나레이션 제거. 소리·재료색·물리 보상·봉인등·진행 이벤트 유지. 실패 피드백/힌트 유지. 호환용 GameText 상수도 빈 문자열로 정리했고 성공 경로에서 호출하지 않는다. |
| B4 | WaterNormal 직조 노이즈 대신 어두운 청록 액체, 두 개의 긴 저주파 파동과 약한 둥근 수면 반사. 기존 GlowFeedback과 해결 후 WarehouseDarkening 감쇠를 유지. |
| B5 | 창 방향 차가운 보조 스폿과 통로의 간접 반사광을 backgroundLights에 등록해 4단계와 함께 감쇠. 촛불은 변경하지 않음. 기존 작업대 도기는 낮은 채도 청록으로. 그림의 조도는 실제 조명에서 받되 색온도만 중립화하여 휴대 랜턴 때문에 모든 안료가 주황이 되지 않게 함. |
| B6 | GetPrompt가 Core의 IsIngredient로 판정. 수정 2종/물약에는 넣기 표시가 없고, 독버섯을 포함한 후보 재료 4종은 동일하게 넣기 표시. 불가능한 물건의 직접 호출도 소모/실패 없이 거부됨을 테스트. |
| B7 | P2 레시피와 P3 미해독/해독 후를 실제 게임 카메라로 새 촬영. 레시피를 수채 사본 그림으로 교체하고, 비늘이 잎 같던 첫 시안을 다시 수정. 버섯 제외 도해를 별도 하단 행으로 분리. P3의 물방울/불꽃은 검은 판 없이 직접 각인. WarehouseClearingBuilder의 triangle/triangles는 천 메시의 인덱스 배열이며 화면의 연금술 기호가 아니다. |
| B8 | 제단 층과 봉인 장치 받침의 모서리·재질을 다듬고 궤짝 지지대에 목재 걸레받이/철 띠를 추가. 기존 통의 속 빈 판재 구조, 책 표지·등·종이, 상자 나뭇결·띠는 보존. 전면 교체/동선 변경 없음. |

### 다시 보고 고친 것

- 첫 투명 분필 출력은 흰 면적이 넓어 폐기. 검정 위 성긴 외곽선으로 다시 생성한 뒤 실제 축소 캡처에서 선이 너무 가늘어, 네 장 모두 선을 굵게 재편집했다. 최종 월드 셰이더는 검정 바탕을 버려 칠판 면을 그대로 보인다.
- 첫 석재 메시의 옆면 winding이 안쪽으로 향한 문제를 캡처에서 발견해 수정. 모든 정점 법선의 바깥 방향을 EditMode 회귀 테스트로 추가했다. 옆면 UV를 별도 투영해 세로 늘어짐도 줄였다.
- 첫 레시피의 절차적 도형은 여전히 아이콘 느낌이라 최종안에서 버렸다. 내장 이미지 생성으로 세밀한 사본 그림을 만들고, 잎처럼 보이는 비늘만 한 번 더 편집했다. 번호는 코드로 별도 합성한다.
- 따뜻한 랜턴이 저채도 그림도 다시 주황으로 만들었다. 전용 WarehouseIllustration 셰이더는 광량은 유지하고 안료의 색온도 오염만 줄인다. 그림 자체 고정 발광을 추가하지 않았다.
- 차가운 창 스폿만으로는 다광원 Forward 렌더링에서 변화가 약했다. 동선 위의 낮은 간접 반사광을 보강하고, 실제 조명 on/off 같은 시점 캡처를 남겼다.
- 첫 수면은 너무 평평해 보였다. 고주파 노이즈를 되살리지 않고 약한 저속 원형 파문을 추가했다.
- 칠판 촬영에서 이동 충돌로 시선이 튄 캡처는 폐기하고, 촬영 중 컨트롤러만 잠시 정지해 동일 목표를 다시 촬영했다. 게임 컨트롤 동작 자체는 변경하지 않았다.

## 실제 검증

| 최종 검증 | 결과 | 증거 |
|---|---|---|
| Warehouse batch 생성 | Exit 0 | 로컬 stage12-build-lighting.log |
| 전체 EditMode | 18 통과 / 실패 0 / 건너뜀 0 | stage12-editmode.xml |
| 전체 PlayMode — Classroom·공용 XRI·시뮬레이션 VR 포함 | 43 통과 / 실패 0 / 건너뜀 0 | stage12-playmode.xml |
| 최종 집중 시각/전 진행 검증 | 1 통과 / 실패 0 | stage12-capture-final.xml; 이후 최종 전체 43개에서도 재통과 |
| 수정 전 캡처 | 1 통과 / 실패 0 | stage12-before.xml |
| 순수 C# Core | 44 통과 / 실패 0 / 건너뜀 0 | stage12-core.trx |
| 소스 공백 검사·Core/Runtime 미러 | 통과 / 5개 byte-identical | git diff --check, 파일 비교 |
| 시각 검수 | 전후 및 추가 캡처 33장 | 이 문서 이미지·아래 해시 |

최종 전체 테스트는 마지막 칠판 합성 배경 수정까지 포함한다. 성공 GameText 세 상수는 빈 문자열이고 호출 경로도 제거했다. 실패/잘못된 물건 피드백만 남는다. 전체 PlayMode는 실제 실패 후 재료 원위치 복구, 치우기/접근 차단/리셋, 어두워짐, 소프트웨어 VR 회귀를 포함한다.

실행 중 메모리 부족 크래시는 없었다. 6GB 가드를 유지했으며 Unity는 직렬 실행했다. 로그의 라이선스 토큰 갱신 경고 뒤 entitlement가 해결되어 테스트가 정상 종료했다. OpenXR의 no-HMD 초기화 메시지는 실제 헤드셋 미연결에 따른 것이며 HMD 검증을 뜻하지 않는다. 셰이더/스크립트 컴파일 오류는 없었다.

구현 커밋: `4e8ad68`(상호작용·토스트), `9aa5954`(그림·재질·조명·테스트·정본). 검증 증거는 별도 커밋으로 남긴다.

주요 파일: `Assets/Editor/WarehouseStage12Builder.cs`, `Assets/Shaders/Warehouse{Chalk,Illustration,Incision,Brew}.shader`, `Assets/Art/Warehouse/`, `Assets/Scripts/Runtime/Puzzles/`, `Assets/Scripts/Core/GameText.cs`와 CodeDrafts 미러, `Assets/Tests/{EditMode,PlayMode}/WarehouseStage12*`, `Tools/Generate-Warehouse-Art.py`, `Tools/Run-Unity.ps1`, `AGENTS.md`, `docs/WAREHOUSE_XRI.md`.

```powershell
git pull --ff-only
python Tools/Generate-Warehouse-Art.py
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage12-build-final -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage12-editmode -Tests -TestPlatform EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage12-playmode -Tests -TestPlatform PlayMode
dotnet test Tests/MagicEscape.Core.Tests --logger 'trx;LogFileName=stage12-core.trx' --results-directory docs/validation/warehouse
```

수정 전 증거는 `MAGICESCAPE_STAGE12_BEFORE=1`로 WarehouseStage12VisualTests만 기존 씬에서 실행했다. 최종 캡처는 같은 테스트의 after 경로이며 실제 게임 카메라/HUD를 1280×720 RenderTexture로 렌더링한 것이다. SceneView·합성 목업이 아니다. 바닥 액자는 정상 서 있는 눈높이에서 아래를 본다. 칠판 클로즈업은 FOV 38이며 월드 크기를 키운 것이 아니다.

## 전후 스크린샷

아래 before는 수정 전 실제 씬, after는 최종 씬이다. 전체 원본은 같은 폴더의 stage12-*.png에 있다.

| 대상 | 수정 전 | 수정 후 |
|---|---|---|
| P1 칠판 | ![칠판 전](stage12-before-chalk.png) | ![칠판 후](stage12-after-chalk.png) |
| 칠판 확대 | ![확대 전](stage12-before-chalk-inspect.png) | ![확대 후](stage12-after-chalk-inspect.png) |
| 액자 I / 대지 | ![I 전](stage12-before-frame-I.png) | ![I 후](stage12-after-frame-I.png) |
| 액자 II / 물 | ![II 전](stage12-before-frame-II.png) | ![II 후](stage12-after-frame-II.png) |
| 액자 III / 불 | ![III 전](stage12-before-frame-III.png) | ![III 후](stage12-after-frame-III.png) |
| 액자 IV / 빛 | ![IV 전](stage12-before-frame-IV.png) | ![IV 후](stage12-after-frame-IV.png) |
| 제단 / 6문양석 | ![제단 전](stage12-before-altar.png) | ![제단 후](stage12-after-altar.png) |
| 가마솥 / 잘못된 수정 선택 | ![가마솥 전](stage12-before-cauldron.png) | ![가마솥 후](stage12-after-cauldron.png) |
| 완성 후 가마솥 | ![완성 전](stage12-before-cauldron-solved.png) | ![완성 후](stage12-after-cauldron-solved.png) |
| P2 레시피 확대 | ![레시피 전](stage12-before-recipe.png) | ![레시피 후](stage12-after-recipe.png) |
| P3 해독 전 | ![P3 전](stage12-before-seal-hidden.png) | ![P3 후](stage12-after-seal-hidden.png) |
| P3 해독 후 / 받침 | ![각인 전](stage12-before-seal-revealed.png) | ![각인 후](stage12-after-seal-revealed.png) |
| 시작 방 전경 | ![방 전](stage12-before-room.png) | ![방 후](stage12-after-room.png) |
| 마지막 어두워짐 | ![어둠 전](stage12-before-room-darkest.png) | ![어둠 후](stage12-after-room-darkest.png) |

추가 소품·색 대비 증거:

- ![창가의 차가운 영역](stage12-after-window.png)
- ![작업대 맥락](stage12-after-workbench.png)
- ![궤짝 지지대](stage12-after-chest-support.png)
- ![달빛 보조광을 끈 비교](stage12-after-room-moon-off.png)

수면 시간차 추가 증거: ![수면 움직임](stage12-after-cauldron-motion.png)

## 네 가지 기준 자기 검수

| 그림/소품 | 장면 개연성 | AI 슬롭/도식성 | 색 단조로움 | 퍼즐 명확성·아쉬움 |
|---|---|---|---|---|
| 칠판 4컷 | 한 작업대·같은 토분의 파종/물주기/등불/아침이라 이전 흙더미+화로보다 자연스럽다. | 흰 면·빽빽한 목판 질감 없이 건조한 분필 외곽선. 창/토분 반복은 의도적 설명 그림이다. | 분필은 무채색, 실제 칠판색이 그대로 보인다. | 굵은 선과 번호 유지. I의 작은 씨앗은 거리상 보조 정보이며 주된 형태는 흙 든 토분. 작은 III 불꽃/IV 광선은 E 확대와 액자 연결로 재확인 가능. |
| 액자 I | 파종용 토분과 씨앗, 작업대의 등잔/책/말린 약초가 창고 작업실 맥락에 맞는다. | 정중앙 대칭·구멍 둔덕·광택 유화 제거. | 푸른 창과 회녹 그림자/흙색, 중립 안료 조명. | 평평한 흙과 씨앗이 대지. 씨앗 크기는 설명을 위해 다소 과장되어 있다. |
| 액자 II | 동일 토분에 물을 주는 손/주전자. | 세밀한 잉크 선과 얇은 담채, 비대칭. | 청회색 물줄기와 창, 흙은 조연. | 물줄기/젖은 흙이 원소를 직접 가리킨다. 물줄기가 작은 월드 화면에서 가늘어지므로 확대 검수도 함께 했다. |
| 액자 III | 실내 파종대의 작은 등불로 야간 보온한다. | 옆 폭발 대신 심지 위로 선 좁은 불꽃. | 불 주변만 호박색이고 창/벽은 냉회색. | 불이 III임이 분명하다. 실제 원예 난방 효율보다 사본의 상징적 장면에 가깝다는 한계는 있다. |
| 액자 IV | 같은 창으로 아침 빛이 들고 작은 싹이 튼다. | 태양 아이콘/대칭 광륜/유화 광택 없음. | 청회 창, 녹색 싹, 옅은 종이빛 광선. | 빛줄기가 주역이고 식물은 작다. 햇빛의 부피감은 명확성을 위해 현실보다 도해적으로 강조했다. |
| 6문양석·명판 | 손으로 누르는 석재 판, 광물 상감/깎인 모서리. | 검은 버튼 사각형과 금색 UI 선을 없앴다. | 회녹 석재와 옅은 중성 각인, 정답별 장난감 색구분 없음. | 6개의 기존 그림문자와 큰 바탕 이름 유지. 달 표시와 Shadow ID를 분리. 표면 요철은 저비용 재질이며 실제로 깊게 조각된 지오메트리까지는 아니다. |
| 가마솥 | 철 솥 안의 어두운 액체/기포가 제조 작업과 맞는다. | 직조형 노이즈를 제거. | 청록 수면과 따뜻한 불빛이 대비. | 정상 단계색·보상 소리는 유지. 원형 파문은 물리 유체가 아니라 약한 셰이더 연출이며, 정지화면 한 장만으로 움직임을 입증할 수 없어 시간차 캡처도 남긴다. |
| P2 레시피 | 약초/비늘/광물병의 작업용 사본. | 첫 아이콘형 시안과 잎 같은 비늘을 재수정. | 회청 유리·회녹 잎·소량 갈색만 사용. | I→II→III 화살표와 분리된 금지 버섯. 비늘이 조개껍질처럼 읽힐 여지는 남으나 불꽃·세계의 비늘 병·그림 순서가 중복 단서를 이룬다. |
| P3 봉인판/받침 | 문 옆 석제 장치, 물/불 소켓과 대응하는 직접 각인. | 검은 붙임판 제거; 천 메시의 triangles는 화면 기호가 아니다. | 냉색 물/따뜻한 불의 대비와 기존 국소광 유지. | 읽기 전에는 답 각인이 없고 물약 후에만 나타난다. 중앙 부유핵/회전 장식은 의도적으로 마법적이며 사실적 고증을 목표로 하지 않는다. |
| 궤짝 받침·상자 | 기존 지지대에 걸레받이와 철 띠, 쌓인 상자 접지/지지 유지. | 날것의 박스 느낌을 줄였으나 모든 상자를 고밀도 모델로 교체하진 않았다. | 갈색 목재에 회청 철 띠, 더 주황 재질을 늘리지 않음. | 치우기 콜라이더/레일/덮개와 숨김 조건은 동일. 좁은 각도에선 기존 나뭇결 반복이 보인다. |
| 도기·통·책 | 작업대의 고정 약재 도기, 속 빈 나무통, 제본 책 맥락 유지. | 통/책의 기존 판재·띠·표지 세부를 보존. | 도기는 낮은 채도 청록 포인트. | 도기는 비상호작용 배경이며 정답색으로 사용하지 않는다. 작은 병목/잔은 여전히 저폴리 실루엣이다. |
| 방 조명 | 창에서 비스듬히 드는 보조 달빛과 기존 촛불. | 화면 전체를 균일한 청색 필터로 바꾸지 않는다. | 따뜻한 섬과 차가운 보조광, 색 있는 그림을 분리. | 풀수록 어둠이 유지되고 국소광은 보존. 따뜻한 창고라는 정체성은 여전히 강하며 주황을 완전히 제거하는 목표는 아니다. |

## 이미지 생성 방식·재현

칠판 확대도 네 개의 서로 다른 검정 사각형이 보이지 않도록 합성 여백을 원본과 같은 검정으로 통일했다. 월드에서는 여전히 검정을 잘라 실제 칠판 면을 보인다.

내장 image_gen만 사용했고 외부 유료 API/CLI를 호출하지 않았다. Story_1 생성 후 같은 원본을 참조해 물주기/밤 등불/창빛으로 편집. 공통 프롬프트는 “16th-century apothecary manuscript, fine imperfect sepia ink with sparse muted watercolor, asymmetric fixed window potting bench, flat soil, slate-blue/sage/earth pigments, no text/numerals/glyphs, no oil painting/clipart/cavities”. Chalk는 같은 장면의 검정 위 성긴 흰 분필 외곽선으로 만들고 네 장 모두 선 굵기를 재편집했다. 최초 투명 배경 시안은 채택하지 않았다.

RecipeStudy는 같은 세피아 사본 화풍으로 왼쪽 달꽃, 가운데 불꽃 아래 비늘, 오른쪽 수정가루 병, 별도 하단 X 버섯을 요청했다. 후속 편집은 “change only central leaf-like scales to rounded shed reptile keratin plates, preserve everything else”. 최종 파일은 `Assets/Art/Warehouse/Textures/Story_1~4.png`, `Chalk_1~4.png`, `RecipeStudy.png`에 복사했다. `Tools/Generate-Warehouse-Art.py`는 원본 그림을 재생성하지 않으며 칠판 합성·레시피 번호·기존 그림문자·Warehouse 이름 마스크만 재현한다.

## 남은 한계

- 실제 HMD 착용 시 손 도달/시야, 실제 스피커의 주관적 음량, 저사양 PC 프레임률, 독립 플레이어 빌드는 미검증. 기존 소프트웨어 테스트/시뮬레이터 통과를 하드웨어 합격으로 표현하지 않는다.
- 이미지 생성물의 사본 화풍은 역사적 실물 복제나 고증 보증이 아니다. 위 표의 씨앗 과장·비늘 해석 가능성·저폴리 세부는 솔직한 잔여 한계다. 사용자 최종 미적 승인은 별도다.
- Unity는 한 번에 하나만 실행하고 여유 커밋 메모리 6GB 가드를 유지했다. 이번 실행에는 메모리 부족 크래시가 없었다.
- 기존 미추적 XML/TRX와 기존 추적 검증 이미지, Classroom 씬/공유 폰트·프리팹·에셋은 보존한다. 이번 증거만 명시적으로 스테이징한다.

## 최종 에셋·캡처 SHA-256

| 파일 | SHA-256 |
|---|---|
| `Chalk_1.png` | `f91f8f1d870641128860d1a06d283cd2847fb6ae7630c361c19b3c0f8096e53b` |
| `Chalk_2.png` | `b0c4b45e4e6d04101b991341abd4351f76cfd690d1a2e669b5c23b7ebfb36789` |
| `Chalk_3.png` | `7de6dc421109c24cedc2b8d573823215b5c4b9caec49f1e41a17d5b787b0cb75` |
| `Chalk_4.png` | `6e3364b74a413efdcdd90bba4f8bcd99890a151405704166e84175b5c49dc1b4` |
| `RecipeStudy.png` | `73b046eee12f79c9ff1aa2f3e27dd295ca03f8bcbcc3c992f7dd73f82213371c` |
| `Story_1.png` | `1eabd69ef66d684d1e9de60a1abe1ab9f2d32e7be83ed1e4afc1830fed1aaa09` |
| `Story_2.png` | `b5dbe55ad036285318e0e69f04fb20981ccbc0400dbb84b15d17f8814edc00e9` |
| `Story_3.png` | `07ff13229aae600fd8868741a1f72c8dcabdeaf0972a680353289703a506ee2e` |
| `Story_4.png` | `d50f5e799baf3741161712887395dc03bc9b589f8d2e12636499d36424d17dfb` |
| `ChalkScenes.png` | `685387d8997f950289f0d53e14b37e1652565e25199a09707cdff6178fe2875d` |
| `PictureRecipe.png` | `f93343f5387017a3f446d29076507bbf4128a86cb8396a773bb3976c629a36df` |
| `stage12-after-altar.png` | `1fb203fe122e4da0fecf24ce698bddf396b5687ec927572a7f3ae7ad4e660ca8` |
| `stage12-after-cauldron-motion.png` | `1922f8ff073ab92963de0857cf38162c8639ae2b63f40c7d2bbc639369b4addc` |
| `stage12-after-cauldron-solved.png` | `f6bf2116ace50035ca291aa8f552c95a18434163b7d47de3fdd05d9658ee65f4` |
| `stage12-after-cauldron.png` | `dfc8945483f124c982bdc88395827e469c005235bd6fbef2fed5ce202fab6242` |
| `stage12-after-chalk-inspect.png` | `4d31f78e9b8c5acc0ca259c0ebf0f014617a93abc9faffa5dfe33104cc7617f9` |
| `stage12-after-chalk.png` | `e6b84b5e220973dd9a89c5bc0c891525dd6efb54f32728db5635424be23170f7` |
| `stage12-after-chest-support.png` | `efe37dda48ed37521dc944286bdb38f5dc90bf8a035be772aae7be36f98384b8` |
| `stage12-after-frame-I.png` | `4782dd27a93074e05b76160d044b4c542bae7c1197fea572caca71053c51ef8d` |
| `stage12-after-frame-II.png` | `fabc8d8fb975a355ef86a12f44391cdcbb0fb44d0f73c3c087797a5f9043e72d` |
| `stage12-after-frame-III.png` | `ab3fce8d5c36860e574417de8a5a6e24b7296bf2842295d6b24dbb30baa57fa2` |
| `stage12-after-frame-IV.png` | `668266a3706e0e8bc2e57b737f4430233a27cb4c4af56476420e3584bf41c09c` |
| `stage12-after-recipe.png` | `3c5ae72afca2b57f00d8bac921d97fa05d6b07aa29d52f7074384aea848a2b4f` |
| `stage12-after-room-darkest.png` | `4fc224aa539de4ca8d0f748a8745f5567ea057d6a365aa788374f84a7be6cf94` |
| `stage12-after-room-moon-off.png` | `89a944d118f4ac7b360c66c73ecc07f47f7233386be2be3ae09e4e129fdef421` |
| `stage12-after-room.png` | `9314ee18796b47adc54c02e4f55c04e3f5347b29cac98c3f784171d310dd0283` |
| `stage12-after-seal-hidden.png` | `71e8615cea1717aa090f868314a1e4465d96bdcf6482e2f568e76cfc6d1339ea` |
| `stage12-after-seal-revealed.png` | `3928813be1de5cb6f159ab15a97767a9630df1405648e21148a8bb11f743b647` |
| `stage12-after-window.png` | `c24dbca074945b8e15296d5f02541b6726c2f140b663eac03e2c77c95971ee19` |
| `stage12-after-workbench.png` | `1679be86fd4eaa7db35abc0c37b12979ab95f7995e10ab3e9d273fc30cbc43e2` |
| `stage12-before-altar.png` | `ba154c6bdd356b13adc8b0d9ffab23f96bc60f9cbf74574a4778a43b458987e7` |
| `stage12-before-cauldron-solved.png` | `f1383d3e010b1f8d2538410b49d46dcda218b03b8b70cdf3254577cd70f86368` |
| `stage12-before-cauldron.png` | `cc02e98617c86566b5572024fd653d61834497f6d23ad82e5fc7ab30fd655221` |
| `stage12-before-chalk-inspect.png` | `df7e7cb9327e1b09a57bb7fc6ab4041a27cbcf03bdcfe72d98cf456a859c18ce` |
| `stage12-before-chalk.png` | `dab8029d6c1526e875886538051bf2549c9f27704d1f65f5631b52d5aadc6abc` |
| `stage12-before-frame-I.png` | `aa54210427d4df472fed8227ebcfb9b94b9478120d2c3c6015338d2844366cca` |
| `stage12-before-frame-II.png` | `abd0a5f57271b6eb91c6dac13e2a632fda955e27e2b9db0a65ef4a760d341a98` |
| `stage12-before-frame-III.png` | `fb86c96e35180a3105c2103553544d184dde7e4489fa3bae39c606951d108dc3` |
| `stage12-before-frame-IV.png` | `33db89375c8ad290eacc74107a13c263826810353f8881b596b665c41e5ca165` |
| `stage12-before-recipe.png` | `3431aee34ca892e849ad1487d476baa953625509ad439b88abd2db8d1dedb138` |
| `stage12-before-room-darkest.png` | `9ec44a3095ab8b81084184577c1ad4e036aceb5f2257cef4760f8d973574efd8` |
| `stage12-before-room.png` | `78cbe02c4905ece021d8c55c85337e42aebbcaa58643c955b4915a6ca82e11ea` |
| `stage12-before-seal-hidden.png` | `d58ba26c2e6bd346cd5e800ab18e87e87c40eb5787da554fcfee60920ccc7e7f` |
| `stage12-before-seal-revealed.png` | `209239399679aee2ac0c02b0db5b0b5502dcec90ced18e9379edc77c53d7ab08` |
