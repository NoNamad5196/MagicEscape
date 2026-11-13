# MagicEscape 제안서 5차

제출본 이름은 `MagicEscape_제안서.pptx`, `MagicEscape_제안서.pdf`로 유지했다. 총 12장이다.

## 장별 제목
1. MagicEscape
2. 기획 배경
3. 작품 개요
4. 이야기
5. 창고 평면과 숨은 단서
6. 치우기 상호작용
7. 플레이 흐름
8. P1 · 마법 문양 해독
9. P2 · 마법약 제조
10. P3 · 최종 봉인 해제
11. 조작 설계
12. 기술 구조

## 수정 내용과 판단
- 배경을 약 9×7m의 마법학교 연금술 실습 창고로 바꿨다. 표지에 김선우 (202321850)를 넣고 날짜는 넣지 않았다.
- 이야기와 평면도를 분리했다. 새 상호작용 장까지 더해 기존 10장을 12장으로 구성했다. 20pt 본문과 그림 크기를 보존하기 위한 판단이다.
- `git show 35ec8b4:presentation/proposal/generate-proposal.mjs`의 3차 평면도를 참고했다. 벽 윤곽·출구·소품 배치를 선화로 되돌리고 좌표를 지웠다. 배치 개념도이며 시공용 축척도는 아니다.
- 보이는 물건은 남색 실선, 가려진 단서는 파란 점선이다. 벽면 선반, 쌓인 상자, 책더미, 천, 궤짝과 제단·가마솥을 그렸다.
- 치우기 전/후는 상자-액자, 천-비문판의 두 쌍을 크게 배치했다. 요청 범위인 2~4쌍 중 두 쌍을 선택해 아래 조작 표의 가독성을 확보했다. 책더미·궤짝 속 재료는 평면도와 P2의 가림 계획에 표시했다.
- 가벼운 물건, 무거운 물건, 고정 장치를 구분하고 VR/키보드·마우스 조작을 표로 정리했다. 무거운 물건은 밀기·끌기만 허용한다.
- 각 퍼즐은 기존 단서/조작/결과/실패 구조를 유지하고, 가림 위치와 봉인등 소등을 추가했다. 회색 카드 채움 대신 선과 여백으로 구분했다.
- 기획 배경은 문장 전달, 물건 읽기, 가림 걷기의 비교다. 실패 연출 다음 글 힌트만 표시했다. 정본에 없는 중간 단서 강조 단계는 추가하지 않았다.
- 조작 설계와 기술 구조는 4차를 유지하고 물리 오브젝트의 Rigidbody 무게 구분과 잡기·밀기·소켓 계획을 더했다.
- 밝은 배경, Paperlogy 7 Bold 제목, Pretendard 본문, 파란 단일 강조색을 유지했다. 본문은 20pt 이상이다. 원소 도안·원소별 색·클립아트·스크린샷은 사용하지 않았다. 도식은 편집 가능한 DrawingML 선과 도형이다.

## 재생성
저장소 루트에서 실행한다. PPTX를 PowerPoint에서 열어 둔 경우 먼저 저장하고 닫는다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-revision5' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
```

## 실제 검증
- 위 네 명령을 실행해 PPTX 12장, PDF 12쪽, 1440×810 PNG 12장을 생성했다.
- 미리보기 12장을 모두 직접 열었다. 상호작용 장의 바닥글 겹침과 기술 구조 설명의 영역 넘침을 고친 뒤 재생성·재확인했다.
- 검사기는 장수, 지정 폰트, 20pt 이상 본문, 도형/텍스트의 페이지 경계, PDF 텍스트 span 겹침, PPTX와 PDF 문구 일치, 금지어를 확인한다. 결과는 `validation.json`에 기록한다.
- 금지어와 이전 공간 표현, 잘못된 점등 목표가 없다. 봉인등은 시작 때 켜지고 퍼즐 해결 때 하나씩 꺼진다.
- P1 정답, P2 재료 순서·재생성, P3 수정 좌우·오답 보존, 글 힌트 기준 3/2/2회를 AGENTS.md §5와 대조했다.
- PDF 내장 폰트는 Paperlogy-7Bold와 Pretendard-Regular다. 대체 폰트는 없다.
- 복사한 Obsidian PDF와 저장소 PDF의 SHA-256 일치를 확인한다.

## 정본과 다른 제안 계획 / 남은 범위
- VR/OpenXR·마우스 클릭 리그는 4차에서 요청한 계획을 유지했다. 창고의 크기와 재료 가림/이동도 새 제안 계획이다. AGENTS.md의 PC 기본·XR 전환 금지, E키/슬롯 조작, 기존 재료 위치를 구현에서 변경한 것이 아니다.
- 정답·아이템 이름·보상·실패 규칙·힌트 횟수는 정본을 유지했다. 책더미/궤짝 안 재료의 개별 배치는 구현 설계에서 확정할 일이다.
- 게임 코드와 AGENTS.md는 수정하지 않았다. Unity·게임 테스트는 실행하지 않았다. 렌더 기준은 LibreOffice PDF이며 PowerPoint 앱에서의 최종 시각 검증은 별도로 하지 않았다.
- 생성 전 열려 있던 기존 PPTX는 저장 완료 상태를 확인하고 닫아 파일 잠금을 해제했다. 저장되지 않은 편집은 없었다.
- PPTX는 폰트를 내장하지 않는다. 다른 PC에서는 지정 폰트를 설치하거나 PDF를 사용한다.
- 추적되지 않은 `docs/validation/*.xml`은 수정하거나 커밋하지 않았다.
- 최종 PDF 복사 위치: Obsidian Vault의 `.aycode/artifacts/magicescape-proposal.pdf`.
