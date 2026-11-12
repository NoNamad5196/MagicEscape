# MagicEscape 제안서

제출본: `MagicEscape_제안서.pptx`, `MagicEscape_제안서.pdf` (동일 내용 11장).
미리보기: `preview/slide-01.png` ~ `slide-11.png`.

## 3차 수정
- 12장 개발 단계·사용 에셋을 삭제했다. 대체 장은 없다.
- 개발 이력·진척 보고·테스트 통과 개수·도구와 에셋 출처 목록을 제거했다.
- 3장은 문장 설명과 이 작품의 그림 단서를 비교한다. P1 액자·칠판·점등, P2 번호 그림 레시피·가마솥 색 변화를 각각 한 쌍으로 배치했다.
- 7장은 순서 도식과 아래 설명 사이를 16pt 더 띄우고, ‘완료’를 ‘해결 시’로 바꿨다.
- 11장은 규칙·표현·씬 연결의 분리 설계와 자동 테스트 검증 계획을 설명한다.
- 쪽번호는 표지부터 `01 / 11` ~ `11 / 11`, 꼬리말은 전 장 동일하다.
- 흰 바탕·남색 본문·파란 강조색, 제목 Paperlogy Bold와 본문 Pretendard를 유지했다.

## 재생성
저장소 루트에서 실행. Node.js, Python(pymupdf), LibreOffice 및 위 폰트가 필요하다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-revision3' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
```

PNG 생성기는 이전 12장 미리보기를 삭제한다. 생성기와 검사기는 11장을 강제한다.

## 실제 검증
- LibreOffice로 PPTX를 PDF로 변환하고, 그 PDF를 PyMuPDF로 1440×810 PNG 11장으로 렌더했다.
- 최종 PNG 11장을 각각 직접 열어 확인했다. 넘침·의도하지 않은 겹침·과도한 빈 공간은 없었다. 표지의 여백은 기존 디자인을 유지했다.
- 7장 순서 도식과 설명의 간격, 3장의 두 비교 쌍, 11장 하단 검증 계획을 확인했다.
- PPTX 텍스트의 요청 금지어·날짜·11장 외 아이템 코드명 검사를 통과했다. 발표 노트와 삽입 이미지는 없다.
- P1 정답·액자 순서, P2 재료 순서·실패 시 재생성, P3 수정 좌우·오답 보존·출구 진입 조건, 힌트 기준 3/2/2회, 거리 2.5m와 인벤토리 8칸을 AGENTS.md §5와 대조했다.
- 시각 연출은 같은 파일 S10e/S10f의 우선 규칙을 대조했다. 가마솥 청백·주황·금색, 봉인 해제 시 주변광·촛불 감소와 단서 조명 보존, 야간 복도·검정 페이드를 유지했다.
- PDF 내장 폰트는 Paperlogy-7Bold, Pretendard-Regular/Bold이며 대체 폰트는 없다. 본문 경계는 y<490, 꼬리말은 y=497이다.
- 게임 코드는 변경하지 않았고 Unity·게임 테스트는 실행하지 않았다. PowerPoint 앱에서의 별도 검증은 하지 않았다.
- artifact-tool 패키지와 의존성 로더가 없어 기존 JavaScript OOXML 생성 방식을 유지했다.
- 기계 검사 결과: `validation.json`.

## 남은 입력
표지의 이름/학번은 사용자 입력으로 남겨 두었다. PPTX에는 폰트를 내장하지 않았으므로 다른 PC에서는 같은 폰트를 설치하거나 PDF를 사용한다.
