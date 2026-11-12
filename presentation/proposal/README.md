# MagicEscape 제안서

제출본: `MagicEscape_제안서.pptx`, `MagicEscape_제안서.pdf` (동일 내용 12장).
미리보기: `preview/slide-01.png` ~ `slide-12.png`.

## 디자인과 폰트
- 흰 바탕, 남색 본문, 파란 강조색 하나. 제목·소제목 Paperlogy Bold, 본문 Pretendard.
- Windows 폰트 내부 이름은 `Paperlogy 7 Bold`, `Pretendard`이다.
- 두 폰트 파일은 사용자 폰트 폴더에 이미 있었다. Paperlogy는 등록이 빠져 있어 사용자 폰트 레지스트리에 등록하고 Windows에 폰트 변경을 알렸다. Pretendard는 기존 등록 유지.
- LibreOffice PDF의 실제 내장 폰트는 Paperlogy-7Bold, Pretendard-Regular/Bold뿐이다. 대체 폰트 없음.
- PPTX에 폰트 파일은 내장하지 않았다. 다른 PC에서는 같은 폰트를 설치하거나 PDF를 사용한다.

## 재생성
저장소 루트에서 실행. Node.js, Python(pymupdf), LibreOffice 및 위 폰트가 필요하다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-revision2font' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
dotnet test Tests/MagicEscape.Core.Tests --no-restore
```

## 2차 수정
- 표지 스크린샷, 전체 원소 기호·기호 도형, 안료 이름·팔레트, 본문 아이템 ID, 작업 문서 코드와 날짜를 제거했다.
- 3장은 GameText.Chalkboard의 실제 문장과 현재 칠판 도식·액자 점등의 전후 비교로 교체했다.
- 7장은 III/불, I/대지, IV/빛, II/물로 표시하고 칠판의 순서 틀과 액자의 정답 역할을 구분했다.
- 9장은 물 각인·불 각인 글자로 표시했다. 10장은 공통 문양 체계·반복 배치·연출·소리의 역할로 재구성했다.
- 12장은 완료 단계와 남은 개발·검증 단계로 교체했다. 제안서 제출 마감 일정은 넣지 않았다.
- 5장 출구 표기와 문 선, 7·8·10·11·12장 하단 여백을 조정했다.

## 실제 검증
- LibreOffice로 PDF 재생성 후 PyMuPDF로 12장 PNG 생성, 전 장을 개별로 열어 확인했다.
- PPTX 기본 도형 298개, 이미지 0개, 발표 노트 0개. 평면도·연결도·장치 도식은 편집 가능하다.
- 정답·재료 순서·수정 좌우·힌트 실패 횟수는 AGENTS.md §5와 대조했다. 칠판 문장은 GameText.cs와도 대조했다.
- 금지 문구·날짜·기호 글리프·삼각형 도형 검사 통과. 본문 PDF 경계는 y<490, 꼬리말은 y=497이다.
- 순수 규칙 테스트 44/44 재실행 통과. Unity 플레이 테스트는 소스 19개 및 기존 결과 기록과 대조했으며 이번 작업에서 Unity를 실행하지 않았다.
- 이 환경에 artifact-tool과 의존성 로더가 없어 기존 JavaScript OOXML 생성 방식을 유지했다. PowerPoint 앱에서의 별도 렌더 검증은 하지 않았다.
- 자세한 기계 검사 결과: `validation.json`.

## 남은 입력
표지의 이름/학번은 사용자 입력으로 남겨 두었다. 게임 빌드·처음 하는 사람 테스트·최종 발표 시연은 남은 개발 단계이며 완료한 작업으로 표기하지 않았다.
