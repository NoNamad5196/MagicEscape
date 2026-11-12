# MagicEscape — 작업 현황

## S10e 5차 — 진행 기록
- Phase 2: E는 그림 확대, Tab은 단서 문장 안전망. 비문 자동 글 패널 제거, 액자 진행 조명/소켓 물·불 효과 추가. `s10e-phase2` 빌더 exit 0. 기존 테스트의 글 표시 기대만 변경; 흐름/정답/실패 검증 유지. PlayMode 전체 실행은 Phase 4에서 수행 예정.
- 사용자 요청으로 단서 표현을 시각 우선으로 변경.
- Phase 1: 원소 6종 메시/움직임/입자, 물결 셰이더, 수정/재료 입자, 가마솥 증기/거품/탁한 실패색 구현. 기존 재료 위치와 Core 불변.
- `s10e-phase1` Unity 빌더 exit 0. 이 시점 PlayMode/외형/노트북 성능은 미검증.
- 새 Runtime 및 Editor는 CodeDrafts 동기화. 새 외부 에셋 없음. 원본 자체 제작 마스크 5종/그림 2종, 기존 판화/문양 가독성 보강.

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
| S1 URP 교실·레이어·조명 | ✅ 자동 검증 1/1; 게임 Console Error 0; 수동 확인 별도 |
| S2 Core/Runtime·시스템·플레이어 | ✅ 자동 검증 2/2; 게임 Console Error 0; 수동 확인 별도 |
| S3 한글 TMP UI·버튼·핫바 | ✅ 자동 검증 3/3; 게임 Console Error 0; 수동 확인 별도 |
| S4 ItemData 7종·카탈로그 | ✅ 자동 검증 4/4; 게임 Console Error 0; 수동 확인 별도 |
| S5 액자·칠판 단서 | ✅ 자동 검증 5/5; 게임 Console Error 0; 수동 확인 별도 |
| S6 P1 문양 제단 | ✅ 자동 검증 6/6; 게임 Console Error 0; 수동 확인 별도 |
| S7 P2 마법약 | ✅ 자동 검증 7/7; 게임 Console Error 0; 수동 확인 별도 |
| S8 P3 최종 봉인·문·탈출 | ✅ 자동 검증 8/8; 게임 Console Error 0; 수동 확인 별도 |
| S9 전체 입력/물리 탈출·Console | ✅ 자동 검증 9/9; 게임 Console Error 0; 수동 확인 별도 |
| S10 에셋·조명·VFX·사운드 | ✅ 자동 검증/재생성; 외형·음향 수동 확인 필요 (validation/s10) |
| S11 Windows 빌드·시연·캡처 | 범위 밖, 미실행 |
| 교수님 VR/일반 PC 허용 확인 | ❔ 기존 대기 유지 |

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


### Windows 세 번째 시도 중단 — 2026-11-12

- HEAD 9af200e에서 git pull 완료. 지정 6000.3.28f1, -nographics 없이 프로젝트 생성 exit 0. 이전 라이선스 차단은 이번 생성에서 재현되지 않음.
- Hub 대신 createProject 후 설치 Editor 템플릿 manifest 버전을 참고해 URP/Input System/uGUI/Test Framework 추가. URP 실제 resolve 17.3.0. Input System 1.12.0의 BuildTarget.ReservedCFE 참조가 CS0117을 발생시켜 S1 빌더 실행 전에 exit 1. 지시대로 중단.
- S1 멱등 빌더/별도 asmdef PlayMode 테스트 초안 및 Tools/Run-Unity.ps1 보존. 씬/URP Asset 미생성, 빌더 소스 자체 미검증. PlayMode 미실행(0개), 게임 Console 미측정, 컴파일 오류 1종 존재. S1~S9 미완료.
- Assets/Scripts 복사 및 CodeDrafts 변경 없음. Runtime asmdef 미추가: 그 필요성을 판단할 S2/테스트 연결 단계에 도달하지 않음.
- 마우스 감도·조작감·UI 클릭·한글 렌더링 수동 확인 필요. S10/S11 미실행. 재개에는 6000.3.28f1 호환 Input System 버전 확인부터 필요.
- 명령/오류 전문/단계별 결과: [Windows 보고서 세 번째 시도](validation/windows-unity-report.md).

Runtime asmdef 추가 이유: 별도 PlayMode asmdef는 Assembly-CSharp를 참조할 수 없으므로 MagicEscape.Runtime을 명시 참조한다. Core/TMP/uGUI/InputSystem 참조만 추가. Assets/Scripts 정본, CodeDrafts 동기화. Tests/UnityStubs 복사 없음. S2에서는 미래 퍼즐 참조용 비활성 scaffold만 두고 S6~8에서 구성·활성화한다.








### 네 번째 시도 최종 상태 (2026-11-12)

- 지정 6000.3.28f1, URP 17.3.0/Input System 1.20.1/Test Framework 1.6.0 유지. S1~S9 자동 검증 완료, S10/S11 미실행.
- 별도 PlayMode asmdef가 Assembly-CSharp를 참조할 수 없어 Runtime asmdef를 추가했다(Core/TMP/uGUI/InputSystem만 참조). Assets/Scripts와 CodeDrafts를 동기화했으며 .cs 내용 차이는 없다. Tests/UnityStubs 복사 없음.
- Editor 빌더는 빈 씬에서 각 단계를 재구성하고 에셋을 재사용한다. 최종 S9 두 번 실행의 171개 오브젝트 구조 manifest 동일(전체 YAML 바이트 동일 보장은 아님).
- 실제 입력/Raycast/물리 경로가 발견한 콜라이더 문제를 수정했다. 정답/itemId/상태/문구·시각 배치/크기 불변. 상세 hull 조정과 테스트 결함은 TEST_PLAN §3 참조.
- 마우스 감도·조작감·UI 실제 클릭·한글 렌더링·받침대 뒤쪽 보행 체감은 수동 확인 필요. 일반 플레이어 10~15분 평가/빌드는 미실행.
- 명령·단계별 커밋·검증 한계: [Windows 최종 보고](validation/windows-unity-report.md#네-번째-시도--2026-11-12-kst).


### 출구 통로 재개 중단 — 2026-11-12 KST
- 기존 출구 확장/봉인 재배치/직진 3경로 테스트 변경 보존. 임시 InitTestScene 정리 완료.
- exit-tests-resumed: Unity TestRunner 시작 OOM 및 native crash, exit -536870911, 신규 XML 없음. 이번 확정 통과 0, 게임 Console Error 미측정. 원인 불명 오류 중단 규칙 적용.
- 1부 미검증·미커밋·미push. S10/S11 미착수. [인계 보고](validation/exit-resume-report.md), [오류 전문](validation/exit-resume-crash.md).

### 출구 통로 재검증 완료 — 2026-11-12 KST
- 출구 유효 폭 3m/북벽 3.3m 개구부/봉인 장치 (-3,0,3.7) 보존, S9 재생성 완료.
- exit-tests-verified PlayMode 12/12, Console Error/Exception/Assert 0. OOM 환경 원인과 해결은 TEST_PLAN §3 참조.
- 이전 중단 상태는 본 결과로 갱신. S10 다음 단계, S11 범위 밖.

### S10 구현 — 2026-11-12 KST
- 무료 CC0 KayKit 모델/아틀라스와 Kenney 효과음만 사용. 자체 메시/VFX 대체안 포함. 선택 Cartoon FX 사용자 다운로드 목록 및 라이선스는 VISUAL_REFERENCES §7.
- S10 빌더 재생성 및 기존 콜라이더/레이어/월드 변환 불변 검사. URP Depth Texture/HDR, Bloom/ACES/Vignette, 촛불·창빛, Emission, 그림자 비용 제한.
- 스크린샷은 docs/validation/s10/의 Game camera+HUD 렌더 4장. 외형과 음향은 **수동 확인 필요**. S11 미착수 유지.

- 최종 자동 검증: s10-tests-final-accepted **13/13**, Console Error/Exception/Assert **0**. 최종 S10 빌더 2회 구조 동일. 상세 결과·한계·명령은 validation/s10/report.md.


### S10b 2? ?? ?? ?? ? 2026-11-12 KST
- S10b ?? ??: ??/?? ?????, CC0 ??+?/???/?? ??, ?? ????/???/???, ?? ?? ?????/? ?, ????/? ?????? HUD ??. Runtime/Core/??/itemId/GameText ?? ??. CodeDrafts ???.
- ?? `s10b-accepted-build`/`s10b-accepted-repeat` exit 0, ?? SHA256 ??. ????/???/?? ?????? ? ?? ???? ?????? ?? audit ??, 3m ?? ??.
- `s10b-tests-accepted`: PlayMode **13/13**, failed/skipped **0**, exit **0**, ?? Error/Exception/Assert **0**. ?? ?? `s10b-tests`? 13/13. Emission ?????? glyph bounds/?? ? ??/HUD ??????? glyph ?? ??. ??? ??? ?? ??.
- ?? Camera.main+HUD PNG **7?**, 1280?720, `validation/s10b/`. Unity ?? ??? 6GB ??? ? ?? Editor ??? ??.
- **?? ??? ?? ?? ??**: ?? ??????2.5m ???/??????? ?????? ??/??. ?? ??/??/??? ??? ?? ?? ??. S11 ???.
- ????? ?????/????? ??: [S10b ??](validation/s10b/report.md), [?? ??](validation/s10b/results.json).

### S10c 3차 아트 패스 — 2026-11-12 KST

- 어두운 푸른 ambient/청록 fog, 촛불 flicker·플레이어 랜턴 sway, 달빛/빛줄기/먼지, 액자별 그림 조명·퍼즐 가독성 조명과 URP Volume 조정 완료.
- 창살 유리·진홍 금장 배너·기둥 석재/받침·봉인핵 룬 고리/맥동/파티클, 형태 있는 픽업/수정, 3D 렌더 핫바 아이콘 7종, 칠판 표시 줄바꿈·우하단 핫바 완료. 새 상호작용/아이템 없음.
- CC0 드론·바람·나무 삐걱임 추가. 원문 라이선스 확인/동봉, 타이틀/문서 크레딧 완료. VISUAL_REFERENCES §9 및 CREDITS.md 참조.
- `s10c-release-build`/`s10c-release-repeat` exit 0, 구조 hash 동일, 기존 collider/레이어/월드 변환/gameplay 설정·참조 불변 검사 통과. 정답·ID·GameText·2.5m·방 크기·3m 통로 유지.
- `s10c-tests-release` 및 마지막 `s10c-tests-final`: 각각 PlayMode **13/13**, 실패/skip 0, game Error/Exception/Assert **0**. 감사 결과는 [results.json](validation/s10c/results.json). PNG 14장 직접 검토, 액자 숫자/칠판/공개 비문/수정 색 가독성 확인.
- **Assets/Scripts가 정본**. 새 Runtime 2개 및 변경 Editor/테스트는 CodeDrafts와 동기화했다. 기존 Core/게임 로직 수정 없음.
- 미검증: 실제 프로젝터·일반 노트북 60fps·사람의 음향 청취/조작감·전체 플레이·S11 빌드. 자동 통과로 대체하지 않는다.
- 상세 파일·실행 명령·오류 수정·스크린샷: [S10c 보고](validation/s10c/report.md).


## S10d ? ?? ?? ??? 4? ?? (2026-11-12)

- [x] ???? ?? UI/?? + ????? ? ??, OFL 1.1 ??/TTF ??????. Static TMP atlas: ???? 518? / ?? 23?, ?? 0. ??? GameText ? ??? ?? ??.
- [x] ??? 4?/?? ??/? UI/?? ??/?? ???, ?? 2? ??. III/I/IV/II? Core/GameText ?? ??.
- [x] ?? 33?72?, ??? ??/?????, ?????????????????. ?/?? ???Fresnel ????? ???, ??/?? ???.
- [x] ?? ?? exit 0; ?? ????/?? ??. PlayMode **13/13**, Console Error/Exception/Assert 0, W-only ?? ??/?/? ?? ??. ?? ??/??? ??? ??? 18.23/17.64GB, Editor ?? ??.
- [x] s10c? ?? ??? 01~14, ??? ??? 1~5 ??? ?? ??. ??: [results](validation/s10d/results.json), [?? ?????](validation/s10d/REVIEW.md), [????](validation/s10d/).
- [x] Runtime/UI/UIManager.cs? ? ??? ???? CodeDrafts ???. FourthArtBuilder/ArtTests/FullInputTests/UITests? ?? ???.
- [ ] ?? ???? ?????? / ???? ?? ?? / ?? ??? ?? ??. ??? ?????? ?? ??? ??? ??? ?? ???? ?? ??? ???? ??.
- ?? 12/13 ? ?(? ?? ???, Dynamic atlas ???)? TMP ??? API ??? ??? ????. ?? ??? ??? ??? results/REVIEW? ??. Editor ???? token ???? ?? Console ??? ????.
