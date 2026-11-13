# MagicEscape 제안서 6차 — GPT 컨셉 아트

기준 커밋은 `0726fae`(5차, 12장)다. 제출 파일명 `MagicEscape_제안서.pptx`, `MagicEscape_제안서.pdf`와 12장 구성을 유지한다.

## 변경
- 표지 오른쪽에 창고 키 아트를 크게 넣었다. 선 도형으로 만든 상자·천·궤짝·문 삽화 코드는 제거했다.
- 2장에 쪽지, 액자와 펼친 책, 상자 뒤 액자 발견 삽화를 각각 넣었다.
- 3장 봉인등은 4장 문 삽화의 등 3개를 잘라 사용한다. 기존 빗금 원 아이콘은 제거했다.
- 4장에 닫힌 철문과 켜진 봉인등 3개를 넣었다.
- 5장은 기존 공간의 출구/서쪽 액자/동쪽 선반/중앙 제단·가마솥/앞쪽 궤짝·창가 관계를 유지하는 아이소메트릭 삽화를 채택했다. 실물의 펜 선은 보이는 물건, 파란 점선은 치워야 드러나는 위치다. 라벨과 점선은 편집 가능한 PPT 개체다. 축척도나 구현 씬 캡처가 아니다.
- 6장 상자/액자와 천/비문판의 전후 두 쌍은 각각 전 그림을 이미지 편집해 후 그림으로 만들었다. 카메라와 배경을 유지했다. 표 아래 설명과 마지막 행 사이에 여백을 확보했다.
- 11장 VR 손·컨트롤러·유리병과 데스크톱 1인칭 유리병·커서를 나란히 넣었다.
- 전체의 '계획' 꼬리표를 제거하고 '가림 계획:'을 '숨긴 위치:'로 바꿨다. 12장 라벨과 설명은 같은 높이의 텍스트 상자 안에서 세로 가운데 정렬한다.
- 밝은 배경, Paperlogy 7 Bold/Pretendard, 파란 단일 강조색, 본문 20pt 이상을 유지했다. 표지는 날짜 없이 `1인 제작 · 김선우 (202321850)`로 표기한다.

## 이미지 생성 기록
- 도구: 내장 `image_gen.imagegen`(GPT 이미지 생성/편집). 도구 응답은 세부 모델 ID를 공개하지 않으므로 특정 모델 번호로 추정하지 않는다. 외부 이미지 API나 키는 사용하지 않았다.
- 공통 스타일: 크림·흰 종이 바탕, 세피아 잉크 펜과 절제된 연한 수채 담채, 비대칭 소품 배치. 같은 스타일 문장을 모든 생성·편집 프롬프트에 그대로 사용했다.
- 정확한 스타일 문장·장면 프롬프트·참조 관계·호출 횟수는 `art/prompts.json`에 기록했다.
- 12개 생성 결과 + 파생 크롭 1개를 `art/`에 PNG로 저장하고 모두 PPTX에 내장한다. 기본 생성 폴더를 런타임 참조하지 않는다.

| 그림 | 파일 | 생성/편집 호출 | 재추출 |
|---|---|---:|---:|
| 표지 창고 | cover-warehouse.png | 1 | 0 |
| 쪽지 | background-note.png | 1 | 0 |
| 액자와 책 | background-frame-book.png | 1 | 0 |
| 상자 뒤 발견 | background-discovery.png | 1 | 0 |
| 철문과 봉인등 | story-door.png | 1 | 0 |
| 상자 전 | clearing-crates-before.png | 1 | 0 |
| 상자 후 | clearing-crates-after.png | 편집 1 | 0 |
| 천 전 | clearing-cloth-before.png | 1 | 0 |
| 천 후 | clearing-cloth-after.png | 편집 1 | 0 |
| VR 조작 | interaction-vr.png | 1 | 0 |
| 데스크톱 조작 | interaction-desktop.png | 1 | 0 |
| 창고 아이소메트릭 | warehouse-isometric.png | 1 | 0 |
| 봉인등 3개 | seal-lamps.png | 문 그림 크롭 | 0 |

## 재생성
저장소 루트에서 실행한다. 기존 생성기의 DrawingML/ZIP 흐름을 유지하고 PNG 임베딩을 추가했다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-revision6' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
```

## 실제 확인
- PPTX 12장, PDF 12쪽, 1440×810 미리보기 12장을 다시 생성했다. PNG를 장별로 전부 직접 열었다. 마지막 5장 라벨 조정 후 해당 미리보기도 다시 열었다.
- 생성 원본들을 직접 확인했다. 이미지 속 글자·원소/연금술 기호·추가 손가락은 보이지 않는다. 쪽지는 글자 대신 해칭이며 비문판은 빈 홈이다. 전후 그림의 배경·카메라·주요 소품 대응을 확인했다.
- 본문 최소 20pt, 지정 글꼴, 페이지 경계, PDF 텍스트 span 겹침, PPTX/PDF 문구 일치, 금지어와 '계획' 0회를 검사했다.
- 검사기는 장별 이미지 개수와 13개 PNG의 원본 바이트 일치, 이미지 관계 참조·페이지 경계, 12장 중앙 정렬도 확인한다. 결과는 `validation.json`에 기록한다.
- AGENTS.md §5의 P1 정답, P2 재료 순서·실패 시 재생성, P3 수정 좌우·오답 보존, 글 힌트 기준 3/2/2회를 대조했다. 봉인등 목표는 모두 소등이다.
- PDF 내장 글꼴은 Paperlogy-7Bold와 Pretendard-Regular다. 복사한 Obsidian PDF와 저장소 PDF는 SHA-256을 비교한다.

## 범위와 남은 사항
- VR/OpenXR·클릭 리그와 재료 가림은 기존 제안 내용이다. AGENTS.md의 PC 기본·E키/슬롯·기존 재료 위치를 구현에서 바꾸지 않았다.
- 게임 코드와 AGENTS.md는 수정하지 않았으며 Unity와 게임 테스트는 실행하지 않았다.
- 렌더 검증은 LibreOffice PDF/미리보기 기준이다. PowerPoint 앱 렌더는 별도 확인하지 않았다. PPTX는 폰트를 내장하지 않으므로 다른 PC에서는 지정 폰트를 설치하거나 PDF를 사용한다.
- 추적되지 않은 `docs/validation/*.xml`은 건드리거나 커밋하지 않는다.
- 최종 PDF 복사: Obsidian Vault의 `.aycode/artifacts/magicescape-proposal.pdf`.
