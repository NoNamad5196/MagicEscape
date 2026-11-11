# MagicEscape — 작업 현황

상태 표기: ✅ 완료 · 🔄 진행 중 · ⏳ 대기 · 🪟 Windows 작업 · ❔ 미검증

## Instabox (설계·코드) — P0~P4

| 단계 | 작업 | 상태 | 근거 |
|---|---|---|---|
| P0 | GitHub 저장소 clone, `.gitignore`, `AGENTS.md` 설계 정본 | ✅ | 커밋 |
| P0 | Notion 참고 문서(싼데비슷한·KUREND) 구조 분석 | ✅ | IDEA_LOG |
| P0 | 게임 기획서 `GAME_DESIGN.md`, 퍼즐 명세 `PUZZLE_SPEC.md` | ✅ | docs |
| P0 | 비주얼/에셋 조사 `VISUAL_REFERENCES.md` | ✅ | docs |
| P1 | 시스템 설계 `TECH_DESIGN.md` | ✅ | docs |
| P2 | Core 순수 C# 규칙 + dotnet 테스트 | ✅ | 34/34 통과 |
| P2 | Runtime MonoBehaviour 22파일 + Unity 스텁 컴파일 검사 | ✅ | New/Legacy 0 오류·0 경고 (실제 Unity ❔) |
| P3 | 씬·프리팹·Inspector 명세 `SCENE_SETUP.md` | ✅ | docs |
| P4 | Windows 인수인계 `UNITY_HANDOFF.md`, 테스트 계획 `TEST_PLAN.md` | ✅ | docs |
| P4 | Notion 허브 + 하위 문서 | ✅ | Notion |

## Windows (AyCode + Unity Editor) — UNITY_HANDOFF §4 S1~S11

| 작업 | 상태 |
|---|---|
| S1 Unity 6.3 LTS URP 프로젝트 생성(`UnityProject/`) 및 저장소 통합 | 🪟 중단: 설치 확인 완료, 프로젝트 생성 시 라이선스 없음 / 로그 exit 198 (2026-11-12); S1 미완료 |
| S2 CodeDrafts → Assets/Scripts 복사, 컴파일 확인 / 플레이어 | 🪟 ❔ |
| S3~S5 UI·ItemData·단서 | 🪟 ❔ |
| S6~S8 퍼즐 1·2·3 연결 | 🪟 ❔ |
| S9 전체 플레이 + Console 0 Error | 🪟 ❔ |
| S10 에셋·조명·사운드·VFX | 🪟 ❔ |
| S11 Windows 빌드, 시연 리허설, 스크린샷·영상(PPT) | 🪟 ❔ |
| 교수님 확인: VR 필수 여부 / 일반 PC 3D 허용 여부 | ❔ |

### Windows 설치 중단 — 2026-11-11

- Hub releases 조회로 6000.3.28f1 확인, 다운로드 완료 후 UAC 승격 단계에서 설치 실패(exit 1, Windows 1223 / ELEVATION_CANCELLED).
- 요청대로 S1 이전에서 중단. S1~S9 완료 아님, Unity 컴파일·PlayMode 테스트·Console Error 수 모두 미검증.
- UnityProject 및 스크립트 변경 없음. CodeDrafts와 Assets 간 차이 없음(복사 전). Runtime asmdef 판단도 미실행.
- 원문 로그/명령/단계별 상태: [Windows Unity 작업 보고](validation/windows-unity-report.md).
- 재개: 관리자 승인 가능한 상태에서 6.3 LTS 설치 완료 후 S1부터. 6000.6 대체 금지 유지.

### Windows 재개 중단 — 2026-11-12

- 설치된 6000.3.28f1 및 Windows Standalone Support 폴더 확인. 시작 git pull: Already up to date (184edaa).
- S1 `-batchmode -nographics -quit -createProject` 실행에서 **유효한 Unity Editor 라이선스 없음**, 로그 종료 코드 198. S1 미완료, S2~S9 미착수.
- UnityProject에는 생성 캐시 폴더만 남았으며 `.gitignore`를 추가해 커밋에서 제외했다. Assets/Packages/ProjectSettings 및 씬은 미생성. URP 대체 경로 시도만 했고 설정에는 도달하지 못함.
- PlayMode 미실행(0개), 게임 Console Error 미측정. 생성 로그 Error 9줄은 라이선스 오류이며 게임 테스트 결과로 치환하지 않는다.
- 스크립트 복사/수정 없음. Runtime asmdef 판단 전. 마우스 감도·조작감·UI 클릭·한글 렌더링 수동 확인 필요.
- 재개 조건/명령/오류 전문: [Windows Unity 보고서의 2026-11-12 재개 기록](validation/windows-unity-report.md).
