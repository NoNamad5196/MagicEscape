# MagicEscape 제안서 4차

제출본: `MagicEscape_제안서.pptx`, `MagicEscape_제안서.pdf` (10장).
미리보기: `preview/slide-01.png` ~ `slide-10.png`.

## 구성
1. MagicEscape (표지)
2. 기획 배경
3. 작품 개요
4. 이야기와 공간
5. 플레이 흐름
6. P1 · 마법 문양 해독
7. P2 · 마법약 제조
8. P3 · 최종 봉인 해제
9. 조작 설계
10. 기술 구조

기획 배경과 플레이어 행동을 앞세우고, 각 장의 중심을 비교도·평면도·흐름도·계층도로 바꿨다. 선택 항목인 완성 기준은 별도 장을 만들지 않았다. 개요의 시간 목표와 두 리그 지원 계획으로 범위를 전달한다.
흰 바탕·남색 본문·파란 강조색을 유지했다. 제목은 Paperlogy 7 Bold, 본문은 Pretendard다. 본문은 20~26pt, 장 제목은 32pt다. 표지의 이름/학번은 입력하지 않았으며 날짜도 없다. 도식은 PPT에서 편집 가능한 도형과 텍스트다.

## 재생성
저장소 루트에서 실행. 기존 Node → PPTX → LibreOffice PDF → PyMuPDF PNG 파이프라인을 유지한다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-revision4' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
```

PNG 생성기는 proposal/preview 안의 기존 slide-*.png만 지운 뒤 10장을 다시 만든다. 예전 slide-11.png는 제거했다.

## 실제 검증
- 위 네 명령을 실행했다. PDF 10쪽을 1440×810 PNG로 생성했다.
- PNG 10장을 모두 직접 열어 확인했다. P3 조작·결과 텍스트의 카드 밖 넘침과 평면도 하단 설명 위치를 수정하고 해당 장을 다시 열었다.
- 글자 넘침·겹침, 하단의 큰 빈 영역, 금지어·금지 항목을 점검했다.
- 검사기는 10장, 20pt 이상 본문, 지정 폰트, 전 텍스트의 페이지 내부 배치, 렌더된 텍스트 span 간 겹침을 검사한다. 금지 문구·스크린샷·발표 노트·원소 그림은 없다.
- P1 정답, P2 재료 순서·재생성, P3 수정 좌우·오답 보존, 힌트 기준 3/2/2회를 AGENTS.md §5와 대조했다.
- PDF 내장 폰트는 Paperlogy-7Bold와 Pretendard-Regular다. 대체 폰트는 없다.
- 기계 검사 결과는 validation.json에 기록했다. 시각 검토를 자동 검사가 대신하지는 않는다.
- 게임 코드나 설계 정본은 수정하지 않았다. Unity·게임 테스트·PowerPoint 앱 검증은 실행하지 않았다.

## 정본과 새 제안 방향의 차이 / 후속 작업
- 이번 명시 요청에 따라 VR(OpenXR) + 헤드셋 없는 데스크톱 조작을 **계획**으로 작성했다. 기존 AGENTS.md §4의 PC 기본·XR 전환 금지 및 §5의 E키·슬롯 조작과는 다르다. 실제 게임 구현 완료를 뜻하지 않는다.
- 이름/학번은 사용자 입력으로 남아 있다. PPTX는 폰트를 내장하지 않으므로 다른 PC에서는 지정 폰트를 설치하거나 PDF를 사용한다.
- 추적되지 않은 docs/validation/*.xml 파일은 수정하거나 커밋하지 않았다.
- 최종 PDF를 Obsidian Vault의 `.aycode/artifacts/magicescape-proposal.pdf`에 덮어써 복사한다.
