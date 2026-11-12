# MagicEscape 제안서

제출본: `MagicEscape_제안서.pptx`, 동일 내용 `MagicEscape_제안서.pdf` (12장).
미리보기: `preview/slide-01.png` ~ `slide-12.png`.

## 재생성
저장소 루트에서 실행. Node.js, Python(pymupdf), LibreOffice 필요. 별도 npm 의존성 없음.
고운바탕 TTF는 `UnityProject/Assets/Fonts`에 있으며 렌더링 PC의 사용자 폰트 폴더에 설치한다.
폰트는 PPTX에 내장하지 않았다. 다른 PC에서 외형 보존이 필요하면 PDF를 사용한다.

```powershell
node presentation/proposal/generate-proposal.mjs
& 'C:/Program Files/LibreOffice/program/soffice.com' '-env:UserInstallation=file:///C:/temp/magicescape-proposal-lo2' --headless --convert-to pdf --outdir presentation/proposal presentation/proposal/MagicEscape_제안서.pptx
python presentation/proposal/render-previews.py
python presentation/proposal/verify-proposal.py
```

## 제작·검증 범위
- 모든 글상자, 평면도, 연결도, 장치 도식, 연금술 삼각형·태양 기호, 표의 선·셀 텍스트는 PPT 기본 도형으로 편집 가능. 토성 기호는 Segoe UI Symbol 글리프다.
- 이미지는 표지의 기존 게임 배경 1장만 사용. 발표 노트 없음.
- 검증 정본: AGENTS §1·4·5와 S10e/f, GameText.cs, SCENE_SETUP의 출구 수정. 오래된 백색 페이드·1.6m 문·중앙 봉인 장치는 사용하지 않았다.
- 모든 12장 PNG를 개별로 열어 확인. 8·12장 하단 간격을 수정 후 재렌더·재검토했다. 한글과 원소 6기호 모두 보임, 빈 장 없음.
- PPTX 구조 검사 pass, 도형 경계/제목 검사 0 finding·0 warning. PDF 본문 경계 검사 및 7개 ID·정답·크기·거리·칸 수 대조 통과. 상세는 validation.json.
- Core 테스트는 이번 작업에서 44/44 실제 재실행. PlayMode는 소스의 테스트 19개를 직접 계수하고 기존 S10f results.json의 19/19 기록과 대조했다. Unity는 이번 작업에서 실행하지 않았다.
- 이 환경에 artifact-tool 런타임/의존성 로더가 없어 JavaScript에서 표준 OOXML 도형을 직접 작성했다. LibreOffice로 렌더링했다. PowerPoint에서의 폰트 대체 렌더링은 미검증.

## 사용자 입력과 후속 개발
표지 이름/학번은 사용자 입력. P1 그림 내용은 개편 예정임을 명시했고 특정 장면을 제안서에 고정하지 않았다. 11/13~15 일정은 계획이며 빌드·사용자 테스트·노트북 성능 검증 완료를 뜻하지 않는다.
