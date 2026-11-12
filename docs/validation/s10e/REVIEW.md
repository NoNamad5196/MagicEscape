# S10e 시각 우선 패스 — 검증·인계

**최종 채택:** `s10e-tests-final-accepted` PlayMode **18/18**, 게임 Error/Exception/Assert **0**, 누락 글리프 경고 **0**. Core dotnet **34/34**. PNG **34장**. 전체 재구성 release 빌더와 크레딧 보정 exit **0**. 소스·Core 동기화 0불일치. `results.json`을 최종 기계 결과로 사용한다.

## 범위와 변경
- Phase 1: 원소 6종 메시/실루엣·원소색·움직임, 자체 파티클 5종, 물결/투명 수정/연기 셰이더. 사파이어 내부 물방울/루비 내부 불씨, 월광초 국소 달빛/꽃가루, 병 속 불씨·별, 붉은 독버섯/녹색 독기. 재료 위치 불변. 가마솥 거품·김, 청백→주황→금, 실패 시 탁한 액체/검은 연기.
- Phase 2: 칠판 I→IV 씨앗 그림, 굵은 액자 판화/큰 원소 모서리 문양, 정답마다 대응 액자 조명/오답 리셋. 그림 레시피와 독버섯 X. 비문 공개 시 물/불 각인과 소켓 효과. E는 그림 확대, Tab은 글 안전망, 자동 실패 힌트는 기존 3/2/2 기준.
- Phase 3: 12초 무문장 오프닝, 촛불→봉인등→제단, 아무 키 건너뛰기. 봉인 소등 입자·소리/주변광·촛불 강도/앰비언스 변화/다음 퍼즐 신호. 봉인핵 빛 파편/따뜻한 출구 빛, 실제 출구 통과 후 백색 페이드/양피지 엔딩/시간/다시 시작·종료.
- Phase 4: 판화 대비, 서쪽 책 더미를 비정형 두 군집으로 이동, 등받침을 작은 금속 팔/컵으로 교체. 밝은 사각형의 추가 원인이던 이전 soot quad도 비활성화. 동쪽 책상 보조광 및 시작 ambient/directional fill 보강.

## 정본과 주요 파일
- `UnityProject/Assets/Editor/ClassroomFifthArtBuilder.cs`: 전체 `S10e()` 재구성 및 Phase1~4 체크포인트. 새 씬/에셋/메타는 Unity Editor API가 생성했으며 YAML을 손으로 쓰지 않았다.
- `Assets/Scripts/Runtime/Effects/{ElementMotion,VisualClues,RoomPresentation}.cs`, `Input/PlayerInputReader.cs`, `Managers/GameManager.cs`, `Interaction/InspectableObject.cs`, `UI/UIManager.cs`, `Puzzles/{RunePuzzle,PotionPuzzle,SealPuzzle}.cs`, 기존 ambience/glow/flicker 수정.
- `Assets/Shaders/Element{Water,Crystal,Smoke}.shader`, `Assets/Art/S10e/`, `Tools/Generate-Art.py`.
- Core/GameText/정답/ID는 수정하지 않음. 2.5m/12×10×4.5m/3m 출구 유지. 마지막 장식 단계 collider manifest 불변, 기존 실제 키보드/Raycast/출구 테스트 유지.
- Runtime/Editor/PlayMode C#는 동일 상대 트리의 CodeDrafts에 동기화. UnityStubs 복사, XR, 유료 에셋, force-push 없음.

## 실제 검증과 수정 이력
- Phase1/2/3 빌더 exit 0. Phase4 첫 실행은 CanvasGroup의 Unity fake-null 참조로 실패 → `??` 대신 Unity null 검사로 수정, phase4-fixed exit 0.
- `s10e-tests-first`: PlayMode 18/18, 실패/skip 0, 게임 Error/Exception/Assert 0.
- 추가 안정화 `s10e-tests-accepted`도 18/18, 실패/skip 0, 게임 Error/Exception/Assert 0. 이후 최종 새벽 창/빛줄기 및 자체 걸쇠 음원을 추가하고 release 빌드/테스트로 채택했다(results.json).
- `s10e-release-build` exit 0, `s10e-tests-release` 18/18. 로그를 추가 확인해 기존 FourthArtBuilder의 크레딧 구분자 오타(U+CA0C)로 인한 TMP 누락 글리프 경고를 발견했다. ASCII `/`로 바로잡고 `s10e-credit-fix` exit 0, ArtTests에 크레딧 글리프 검증도 추가했다. 마지막 결과는 `s10e-tests-verified`다.
- `s10e-tests-verified`도 18/18. 마지막 입력 감사에서 출구가 열리는 동안도 조작을 잠그도록 보강했다. 고정 초 타이머 대신 기존 DoorController의 실제 ExitTrigger 활성 상태(PassageReady)를 기다리며, 테스트용 SkipPresentation도 유지한다. 이 보강 뒤의 최종 채택 테스트 이름은 `s10e-tests-final-accepted`다.
- 전체 `s10e-full-rebuild` exit 0 뒤 `s10e-tests-final`: 18/18, 실패/skip 0, 게임 Error/Exception/Assert 0. 이 결과 뒤 밝기·연기·반응형 조명 복원을 추가 점검했다.
- 추가 밝기 패스의 `s10e-accepted-build`는 삭제 중 Transform 지연 LINQ 열거 때문에 실패 → 열거를 ToArray로 고정한 뒤 재실행. 최종 채택 결과는 `results.json`과 TASKS/TEST_PLAN의 마지막 항목을 기준으로 한다.
- `dotnet test Tests/MagicEscape.Core.Tests --nologo --verbosity minimal`: 34/34, 실패/skip 0.
- Unity 실행 전마다 Tools/Run-Unity.ps1의 6GB 가상메모리/중복 Editor 게이트 적용. 실제 시작 여유는 약 7.18~13.57GB(추가 채택 실행은 results.json 참고). Editor는 한 번에 하나.
- Editor 로그에는 기존 Unity Licensing access-token 갱신 진단이 반복되지만 실행은 완료된다. 이것을 게임 Console 0과 혼동하지 않았다. 원시 로그/XML에는 로컬 환경 정보가 있어 커밋하지 않고 요약 JSON/감사를 남긴다.

## 스크린샷
- 실제 게임 Camera.main + HUD, 1280×720 RenderTexture. SceneView나 합성 모형이 아니다.
- `01~14`: S10d와 같은 카메라 위치/목표/게임 진행 구도.
- `15~16`: 오프닝 촛불/봉인등, `17`: 그림 칠판, `18`: E 확대 그림 레시피.
- `19~24`: 원소 6종 약 2m 거리 비교, `29~34`: 각 원소 근접. `05/12`: 물·불 공개, `25~27`: 액체 3단계, `28`: 엔딩.
- 19~24는 카메라 높이/제단 반경을 포함해 대략 1.8~2.1m의 시선 거리다. 정확한 2.000m 실험이나 실제 프로젝터 검증은 아니다.

## 자체 판정 (무문장 추론)
- **P1: 조건부 가능.** I~IV와 씨앗→비→열기→꽃, 큰 유색 모서리 문양/제단 실루엣으로 Earth→Water→Fire→Light를 연결할 수 있다. 한 컷이 아니라 칠판·액자·제단 컷을 함께 봐야 한다. 햇볕의 열기를 Fire로 연결하는 부분은 처음 보는 사람에게 여전히 추론을 요구한다.
- **P2: 가능.** 확대 그림의 달/꽃잎→불/비늘→별가루와 X 버섯이 순서와 제외 후보를 직접 보여 준다. 액체 색은 진행 확인이지 재료 이름을 독립적으로 설명하는 그림은 아니다. 3D 책만 멀리서 보는 것보다 E 확대를 쓰는 편이 확실하다.
- **P3: 가능.** 좌측 물 문양/물결과 청색 수정, 우측 불 문양/불씨와 적색 수정의 짝이 명확하다. 물약을 먼저 써야 한다는 초기 단계 자체는 인벤토리/상호작용 또는 Tab 목표에 의존한다.
- **6원소: 색+실루엣으로 구분 가능, 표현 품질은 보수적으로 평가.** 물=그릇/방울/파랑, 불=숯/주황 첨화, 바람=회녹 고리/잎, 빛=금 구체/광륜, 대지=갈색 돌/녹색 균열, 그림자=보라검정 연기. 대지의 균열과 그림자의 연기 밀도는 근접 컷에서 더 잘 보인다. 실사 수준 유체/체적 연기는 아니며 저비용 스타일라이즈드 표현이다.
- 이 판정은 개발자가 규칙을 이미 아는 상태의 시각 검토다. 무경험 플레이어의 성공률/소요 시간/2m 원소 명명 테스트를 대체하지 않는다.
- 밝기 비교: 07번의 오른쪽 상단 640×600 픽셀 평균 휘도(8비트 그레이)는 S10d 10.84 → 최종 S10e 15.40. 약 42% 상승했지만 여전히 어두운 모서리가 있다. 이 수치는 화면 측정/조도/가독성 보증이 아니라 같은 구도의 PNG 비교다.

## 라이선스
새 외부 다운로드/파티클 팩/유료 에셋 없음. 새 그림·마스크·셰이더·메시는 자체 코드 생성. 기존 Quaternius/Kenney CC0 및 Gowun Batang/Nanum Pen Script OFL 원문을 로컬 동봉 파일에서 확인. `docs/CREDITS.md` 참조.

## 커밋 체크포인트
- `b7d76b2` Phase 1 원소 표현
- `2d54e23` Phase 2 시각 단서
- `f974e68` Phase 3 오프닝/진행/엔딩
- `7f22f5c` Phase 4 마감/테스트
- 최종 통합 보강·34장 증거·검증 결과는 위 체크포인트 뒤의 `fix(s10e)` 커밋에 포함. 각 Phase는 main에 별도로 push했다.

## 남은 검증
- 일반 노트북 60fps, 실제 프로젝터/화면 밝기, 사람의 전체 플레이·무문장 정답 추론·소리 청취는 미검증. 자동 테스트 통과로 대신하지 않는다.
- 자물쇠 연출은 자체 합성 SealLatch를 사용한다. 실제 사슬 녹음은 아니며 사람의 청취 판정은 미검증. 무대별 앰비언스는 기존 드론·바람의 음량/피치 변주이지 새 음악 트랙이 아니다.
- Notion 동기화와 S11 독립 실행 빌드/배포는 이번 작업에서 하지 않았다.
