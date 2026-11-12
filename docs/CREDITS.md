# MagicEscape 크레딧

## S10e 자체 제작
`Tools/Generate-SealAudio.py`의 `SealLatch.wav`는 표본 입력 없이 감쇠 사인파/잡음으로 합성한 자체 금속 걸쇠 효과음이다(0.72초, mono 44.1kHz). 새벽 빛줄기 메시도 자체 생성이다. 새 외부 라이선스/음원은 없다.
현재 UI/엔딩 글꼴은 Gowun Batang Regular, 기존 분필 글꼴은 Nanum Pen Script Regular이며 둘 다 SIL OFL 1.1이다. 원문은 `Assets/Fonts/GowunBatang-Regular-OFL.txt`, `NanumPenScript-Regular-OFL.txt`에 동봉되어 있다. 새 외부 다운로드는 하지 않았다. 기존 Quaternius `License_Standard.txt`와 Kenney `License.txt`의 CC0 원문도 로컬 파일에서 재확인했다.
`Tools/Generate-Art.py`로 만든 굵은 판화 문양 6종, 명암 보강 씨앗 판화, 무문장 칠판/레시피, Soft/Spark/Drop/Leaf/Bubble 마스크 5종. 외부 이미지 입력/외부 파티클 팩/유료 에셋 없음. 원소 메시·물결 셰이더·효과는 프로젝트 자체 코드. 새 외부 라이선스는 없으며 기존 무료 모델·음향·OFL 글꼴의 동봉 라이선스/아래 크레딧을 유지한다.

## 사용한 무료 에셋

| 구분 | 에셋 / 제작자 | 라이선스 / 원문 |
|---|---|---|
| 가구·양초·기둥 | KayKit Dungeon — Kay Lousberg | CC0, [공식 배포](https://kaylousberg.itch.io/kaykit-dungeon-pack), ThirdParty/KayKitDungeon의 동봉 라이선스 |
| 책장·가마솥·병·랜턴 | Fantasy Props MegaKit Standard — Quaternius | CC0, [공식 배포](https://quaternius.com/packs/fantasypropsmegakit.html), 동봉 License_Standard.txt |
| 석벽·목재 | Stone Wall 02 / Wood Floor — Poly Haven | CC0, [라이선스](https://polyhaven.com/license) |
| 기존 효과음 | Interface Sounds — Kenney | CC0, [배포 페이지](https://opengameart.org/content/interface-sounds), 동봉 License.txt |
| 먼 바람 | wind1 — Luke.RUSTLTD | CC0, [원문·다운로드](https://opengameart.org/content/wind1) |
| 낮은 드론 | ambient sound creepy — DaGameKnower | CC0, [원문·다운로드](https://opengameart.org/content/ambient-sound-creepy) |
| 나무 삐걱임 | Wood Creak Single V1 — Rudmer_Rotteveel | CC0, [원문](https://freesound.org/people/Rudmer_Rotteveel/sounds/502505/); 공개 HQ preview 사용 |
| 한글 폰트 | Noto Sans KR — Google / Noto contributors | SIL OFL 1.1, Assets/Fonts의 동봉 라이선스 |

S10c 사운드 라이선스는 2026-11-12 원문에서 확인했다. CC0 법률 원문과 파일별 출처·가공 과정은 `UnityProject/Assets/ThirdParty/AtmosphereCC0/CC0-1.0.txt` 및 `CREDITS.md`에 포함했다. 드론·바람은 저역 필터와 양끝 오버랩으로 46초 루프로 만들었다. 삐걱임은 약 21–36초 간격으로 재생한다.

문양·그림·배너 문장·밤하늘 텍스처, 각진 수정·꽃잎·버섯, 빛줄기·림 셰이더와 연출은 프로젝트 자체 제작이다. 핫바 PNG는 같은 아이템 3D 표현을 Unity Editor의 카메라/RenderTexture로 렌더했다. game-icons.net 이미지나 별도 외부 아이콘은 사용하지 않았다.

유료 에셋 구매, Asset Store 원본 재배포, VR/XR 패키지 추가는 없다. 타이틀 하단에도 제작자와 라이선스를 표시한다. 상세 사용 목록은 [VISUAL_REFERENCES](VISUAL_REFERENCES.md).


## S10d ? ?? ?? ??? (2026-11-12)

- UI???????????: **Gowun Batang Regular**, Copyright 2021 The Gowun Batang Project Authors. [OFL ??](https://raw.githubusercontent.com/google/fonts/main/ofl/gowunbatang/OFL.txt), [??? ???](https://github.com/yangheeryu/Gowun-Batang).
- ??: **Nanum Pen Script Regular**, Copyright 2010 NHN Corporation. [OFL ??](https://raw.githubusercontent.com/google/fonts/main/ofl/nanumpenscript/OFL.txt). ??? Reserved Font Names? ??? ??? ????.
- ? ??? SIL OFL 1.1. ???? ?? TTF? ?? ??? `UnityProject/Assets/Fonts/*-OFL.txt`? ?? ????. TMP SDF? Editor API? ????. ???? URL?SHA-256? `validation/s10d/font-sources.json`; ?????? `Tools/Fetch-S10d-Fonts.py`. Noto Sans KR? ?? ?? ????? ??? S10d ?? ????? ???? ???.
- ?? ?? 4?: Quaternius Fantasy Props MegaKit **Standard FREE**? `Crate_Wooden`, `Candle_2`, `Scroll_2`, `Book_7`. ?? ?? ?? ?? ZIP? `License_Standard.txt` ??? ?? ????(CC0 1.0). [?? ??](https://quaternius.com/packs/fantasypropsmegakit.html). ZIP/PRO/Source/Asset Store ??? ???? ???.
- ? ???? ?? ?? 4???? ?? 6???? ? UI??? ????? ????????????????? ? ?? ???? ???? ?? ??. ?? ??, AI ???, ?? ??? ???? ???. 3D ?? ???? ?? ??? ????.
