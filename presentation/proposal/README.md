# MagicEscape 제안서 7차

기준 커밋: `02cc7f0`(6차, 12장). 이번 구성은 14장이다. 결과 파일명은 `MagicEscape_제안서.pptx` / `MagicEscape_제안서.pdf`를 유지한다.

## 바뀐 내용
- 기획 배경 다음에 비교표를 넣었다. 세 작품의 조작, 헤드셋 필요 여부, 단서, 공간을 같은 표로 비교한다.
- 작품의 차이는 두 가지로 한정했다. 헤드셋 없이도 같은 규칙으로 끝까지 푼다. 물건·그림으로 단서를 주고 글 힌트는 실패 뒤에 나온다.
- 플레이 흐름 다음에 밝기 변화 장을 넣었다. 원본 한 장을 편집해 봉인등 3→2→1→0개, 촛불·주변 빛 감소, 문 뒤 검은 복도를 보인다. 실내 단서 조명은 보존한다.
- 치우기 그림을 기존 약 192×94pt에서 200×143pt로 키웠다. 손잡이·밧줄·끌린 자국으로 움직일 대상을 구분하는 설계를 추가했다. 장식은 잡히지 않고, 치운 자리에는 단서나 재료가 있다.
- P1은 액자 하나의 원소 하나, 액자 번호 I~IV, 칠판의 빈칸 도식을 연결했다. 그림의 이야기는 쓰지 않았다.
- 이야기에는 쓰지 않는 실습 도구를 보관하는 창고라는 설명을 넣었다.
- P2의 재료 위치는 정본의 창가 화분·동쪽 책장 유리병·교탁 서랍으로 되돌렸다. 치우기 설계가 정본의 아이템 위치를 바꾸지 않게 했다.
- 조작 장 하단의 API 목록을 기술 구조로 옮겼다. 검증 방침은 Simulator, 데스크톱 전 구간 플레이, 빌린 헤드셋 순으로 썼다. 수행 결과나 진척 수치는 넣지 않았다.
- 모든 사용 삽화를 밤 톤으로 다시 만들었다. 예전 쪽지·액자와 책·봉인등 크롭은 더 이상 사용하지 않아 제거했다.
- 그림은 JPEG, 최대 한 변 1600px, 품질 85다. 밝은 슬라이드 바탕, Paperlogy 7 Bold/Pretendard, 파란 강조색을 유지했다. 본문과 비교표는 20pt 이상이다.

## 장별 제목
1. MagicEscape (표지)
2. 기획 배경
3. 비슷한 작품과 비교
4. 작품 개요
5. 이야기
6. 창고 평면과 숨은 단서
7. 치우기 상호작용
8. 플레이 흐름
9. 풀수록 어두워지는 창고
10. P1 · 마법 문양 해독
11. P2 · 마법약 제조
12. P3 · 최종 봉인 해제
13. 조작 설계
14. 기술 구조

## 벤치마킹 근거
웹 확인일: 2026-11-14. 설명은 제작사 자료와 제작사가 제공하는 Steam 제품 정보로 확인했다. 타 작품에 글 단서가 없다는 식의 주장은 하지 않는다.

| 사실 | 확인 출처 |
|---|---|
| I Expect You To Die: Schell Games, 2016년 출시 | [Schell Games의 2016-12-13 PSVR 출시 발표](https://schellgames.com/blog/i-expect-you-to-die-vr-game-goes-multi-platform-with-playstation-vr-launch), [공식 프레스 키트](https://schellgames.com/press/i-expect-you-to-die-press-kit/) |
| I Expect You To Die: VR 전용, 추적 컨트롤러, 염력 조작, 임무별 퍼즐 | [Steam 제품 정보](https://store.steampowered.com/app/587430/I_Expect_You_To_Die/)의 Developer, Tracked Controller Support, VR Only, About This Game, Requires VR Headset 항목 |
| The Room VR: A Dark Matter: Fireproof Games, 2020-03-26 출시, VR 전용·추적 컨트롤러 | [Steam 제품 정보](https://store.steampowered.com/app/1104380/The_Room_VR_A_Dark_Matter/) |
| The Room VR: 물건을 집어 살펴 숨은 단서를 찾음, 여러 장소, 헤드셋 필요 | [Fireproof Games 공식 소개·FAQ](https://www.fireproofgames.com/games/the-room-vr-a-dark-matter)의 Lifelike Objects, Believable Locations, VR exclusive 설명 |

주의: I Expect You To Die의 Steam 출시일은 2017-04-25다. 최초 출시 연도와 혼동하지 않도록 2016년 근거는 제작사의 당시 PSVR 발표를 사용했다.

## 이미지 생성·편집 기록
도구: 내장 `image_gen.imagegen`. 외부 유료 API나 키는 사용하지 않았다. 도구가 정확한 모델 ID를 노출하지 않아 특정 모델 번호를 추정하지 않는다.

공통 스타일 문장, 장면별 전체 프롬프트, 편집 원본 관계, 호출 횟수, JPEG 크기는 `art/prompts.json`에 있다. 모든 호출에 같은 공통 스타일 문장을 넣었다. 단순 파일 변환·축소만 Pillow로 처리했다. 새 PNG 원본은 저장소 밖 도구 기본 저장 위치에 두고 최종 JPEG만 저장소에 넣었다.

| 그림 | 최종 파일 | 호출 | 재추출 |
|---|---|---:|---:|
| 표지 창고 | cover-warehouse.jpg | 생성 1 | 0 |
| 상자 뒤 단서 발견 | background-discovery.jpg | 생성 1 | 0 |
| 이야기의 철문 | story-door.jpg | 생성 1 | 0 |
| 상자 치우기 전 | clearing-crates-before.jpg | 생성 1 | 0 |
| 상자 치운 후 | clearing-crates-after.jpg | 원본 편집 1 | 0 |
| 천 걷기 전 | clearing-cloth-before.jpg | 생성 1 | 0 |
| 천 걷은 후 | clearing-cloth-after.jpg | 원본 편집 1 | 0 |
| 양손 컨트롤러 VR | interaction-vr.jpg | 생성 1 | 0 |
| 데스크톱 조작 | interaction-desktop.jpg | 생성 1 | 0 |
| 창고 아이소메트릭 | warehouse-isometric.jpg | 생성 1 | 0 |
| 밝기: 시작 | darkness-start.jpg | 생성 1 | 0 |
| 밝기: P1 후 | darkness-p1.jpg | 시작 그림 편집 1 | 0 |
| 밝기: P2 후 | darkness-p2.jpg | 시작 그림 편집 1 | 0 |
| 밝기: P3 후·탈출 | darkness-p3.jpg | 시작 그림 편집 1 | 0 |

총 생성 9회, 편집 5회다. 두 전후 쌍은 각 '전' 그림을 편집했다. 밝기 단계 3개는 같은 시작 그림에서 각각 편집해 카메라와 소품 배치를 유지했다. 원본을 모두 직접 열어 밤 톤, 글자·원소 문양·연금술 기호의 부재를 살폈다. VR 양손은 모두 컨트롤러를 쥔다.

## 재생성
저장소 루트에서 실행한다. Node.js, Python의 pymupdf/Pillow, LibreOffice와 지정 글꼴이 필요하다. 생성기는 `art/prompts.json`의 실제 JPEG 크기로 종횡비를 계산한다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-revision7' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
```

## 실제 확인
- 14장의 최종 preview PNG를 직접 열었다. 비교표 하단과 출처가 겹치던 첫 렌더는 표 행간·제작사 행을 정리한 뒤 다시 렌더해 확인했다.
- 이야기·평면·치우기 그림은 크게 배치했다. 모든 사용 그림의 낮 햇빛과 밝은 종이 바탕을 없앴다.
- 플레이 흐름의 원과 단계 이름·설명은 같은 중심 좌표(128, 304, 480, 656, 832pt)를 사용한다.
- 원본 밝기 단계의 평균 휘도는 39.36, 23.46, 12.43, 5.77로 감소한다. 이는 이미지 점검 보조값이며 게임 플레이 수치가 아니다.
- 검사기는 14장·PDF 14쪽, PPTX 15,000,000바이트 이하, JPEG 크기·원본 바이트 일치, 그림 관계, 텍스트 경계·겹침, PPTX/PDF 문구 일치, 글꼴과 본문 크기, 금지어, 타임라인 중심, 전후 편집 관계, 밝기 감소를 확인한다.
- 정답·아이템·실패 시 보존/재생성·힌트 기준 3/2/2를 AGENTS.md §5 및 S10f와 대조했다. AGENTS.md가 기준 커밋과 같은지도 단언한다.
- 상세 결과는 `validation.json`에 있다. PDF에는 Paperlogy-7Bold와 Pretendard-Regular가 내장된다.
- 최종 PDF는 Obsidian Vault의 `.aycode/artifacts/magicescape-proposal.pdf`로 복사하고 SHA-256 일치를 확인한다.

## 범위와 남은 확인
- 게임 코드·AGENTS.md는 변경하지 않았다. VR·데스크톱 두 리그와 움직일 물건 식별은 제안 설계다. 정본의 PC 기본·E/휠 입력을 구현에서 바꾸지 않았다.
- Unity 실행, XR Device Simulator, 데스크톱 전 구간 플레이, 실제 헤드셋 플레이는 이번 문서 작업에서 수행하지 않았다. 슬라이드는 검증 방침만 적는다.
- PowerPoint 자체 렌더는 확인하지 않았다. LibreOffice PDF와 그 PNG를 확인했다. PPTX에 글꼴은 내장하지 않으므로 다른 PC에는 지정 글꼴이 필요하며, 제출 시 PDF로 모양을 보존할 수 있다.
- `docs/validation/`의 추적되지 않은 XML은 건드리거나 커밋하지 않는다.
