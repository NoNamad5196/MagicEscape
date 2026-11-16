# 창고 6단계 — 칠판 정답 노출 차단과 액자 하단 명판

## 변경과 판단
- Warehouse 전용 `Assets/Art/Warehouse/ChalkBlanks.png`를 추가했다. 분필 손그림의 I~IV 번호와 빈 원만 있고 원소 기호·이름·이야기 그림은 없다. 씬 표면과 E 확대 UI는 **동일한 텍스처**를 참조한다. 비활성 옛 칠판 텍스트도 빈칸 도식으로 바꿨다. Classroom의 과거 칠판과 공유 그림 원본은 변경하지 않았다.
- 고정 상자가 I·II 빈칸을 가리는 것을 캡처에서 확인하여 칠판 본체·표면·등을 같은 벽에서 85cm 올렸다(중심 y=1.87m). 치우기 상자나 액자 위치는 바꾸지 않았다.
- 네 액자 모두 하단 황동 명판에 **번호 / 기호 / 이름**을 나란히 배치했다. 기호는 local .18m, 이름은 기존 .36×.12m 마스크이며, 중심 y=-.51로 그림 하단(-.41) 밖에 놓인다. 기존 AlchemyEngraving 및 Gowun Batang 이름 마스크와 문양석의 공유 머티리얼은 유지했다.
- III 그림 자체에도 Fire 삼각형이 구워져 있음을 추가로 발견했다. 기존 절차적 가뭄 그림 생성기에 `--warehouse-iii`를 추가하여 그 중복 기호만 없는 Warehouse 전용 그림을 생성했다. 씬과 III 확대는 이 그림을 공유한다. 기존 Classroom 원본은 보존한다.
- Warehouse의 Tab은 실패 횟수와 무관하게 Chalkboard/Recipe/InscriptionRevealed 문장을 출력하던 우회 경로였다. Warehouse에서만 `failureOnlyClues=true`로 설정하여 `GetCurrentHint()`만 표시한다. P1 3회/P2·P3 2회 실패 힌트는 유지하고 Classroom 회귀 동작은 바꾸지 않았다. Runtime/CodeDrafts 미러가 일치한다.
- 레시피는 P1 해제 후 열리는 재료 그림이며 P1의 4원소 대응을 포함하지 않는다. 비문은 물약 사용 이후 물/불 소켓을 알리는 P3 단서다. 다른 조사 화면의 그림 연결을 허용 목록으로 검사했다. 타이틀·시작 연출에는 P1 4개 번호-원소 대응을 표시하는 별도 로딩 화면이 없었다. 입력 성공에 따른 액자 점등 등 기존 판정 피드백은 유지했다.
- 정답 대지→물→불→빛, RuneId/아이템 ID, P1→P2→P3, I·IV 상자/III 천/궤짝 가루, 4단계 어두워짐, 문양석 이름 각인은 변경하지 않았다. AGENTS.md/README/WAREHOUSE_XRI.md에 현행 빈칸 규칙과 Classroom 예외를 명시했다.
- 재생성 시 Warehouse 전용 머티리얼까지 과거 공유 에셋으로 복원하던 보존 범위를 수정했다. 기존 캡처는 모두 보존하고 stage6 캡처만 새로 남긴다.

## 회귀 검사
- EditMode `ChalkboardResourcesContainOnlyApprovedEmptyDiagram`: 직접 검토한 빈칸 PNG SHA-256, 씬 main/emission 텍스처와 E picture 연결, 비활성 옛 텍스트, 조사 그림 허용 목록 검사. 그림 파일에 기호를 다시 구워 넣어도 해시가 달라져 실패한다.
- EditMode `FrameNameplatesDoNotCoverIllustrations`: 액자 4점의 기호·이름 위치와 셰이더, III의 중복 기호 없는 전용 그림 및 확대 연결 검사.
- PlayMode `EmptyChalkAndFailureOnlyHintsHaveNoAlternateAnswerRoute`: 실제 E→XRI 칠판 열기, 동일 UI 텍스처와 빈 본문, Tab의 사전 글 힌트 차단, 3회 실패 후 힌트, 치우기 후 액자 캡처, P2/P3의 사전 Tab 문장 차단 검사.

초기 생성에서는 비활성 칠판 라벨을 GameObject.Find로 찾던 오류를 고쳤다. 첫 전체 PlayMode는 새 검사 1건이 잘못된 촬영 위치의 캐릭터 충돌로 E를 칠판에 맞추지 못해 36/37이었다. 이후 안전한 위치·충돌 정착 후 재조준·실제 ray 대상 확인으로 수정했다. 중간 캡처에서 고정 상자의 칠판 가림과 분리된 나무 테두리를 발견하여, 테두리까지 칠판 자식으로 부착한 뒤 전체를 재생성하고 최종 전체 검사를 다시 실행했다. 해당 중간 결과를 최종 통과로 계산하지 않는다.

## 검증 명령
한 번에 Windows Unity 6000.3.28f1 Editor 하나만 실행했다. 원본 로그/XML/TRX는 로컬에 보존하고 환경 정보 없는 결과는 `stage6-summary.json`으로 기록한다.

| 최종 검증 | 통과 | 실패 | 로컬 증거 |
|---|---:|---:|---|
| 전체 씬 batchmode 재생성 | 종료 0 | — | stage6-final-generate.log |
| EditMode 전체 | 9 | 0 | stage6-accepted-editmode.xml |
| PlayMode 전체: Classroom + Warehouse | 37 | 0 | stage6-accepted-playmode.xml |
| 순수 C# | 44 | 0 | stage6-core.trx |

두 생성기의 PNG 재생성 해시도 일치했다. 기존 XML/TRX 88개의 SHA-256은 모두 그대로이며, 처음부터 미추적이던 36개 XML/TRX는 삭제·스테이징하지 않았다. 이전 Classroom·공유 아트·stage5 캡처도 변경하지 않았다.

```powershell
python Tools/Generate-Warehouse-Chalk.py
python Tools/Generate-Clearing-Art.py --warehouse-iii
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage6-final-generate -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage6-accepted-editmode -Tests -TestPlatform EditMode
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage6-accepted-playmode -Tests
dotnet test Tests/MagicEscape.Core.Tests --logger "trx;LogFileName=stage6-core.trx" --results-directory docs/validation/warehouse
```

새 실행에서는 기존 로컬 증거를 덮어쓰지 않도록 Name/결과 이름을 바꾼다.

## 캡처
실제 게임 카메라와 HUD를 1280×720으로 렌더링했으며 HMD 촬영이 아니다. 최종 5장을 모두 직접 열어 확인했다. 칠판의 네 빈칸과 테두리가 상자 위에서 전부 보이고, 확대에도 같은 빈칸만 나온다. III/II의 기호와 이름은 그림 밖 하단 명판에 있으며 넓은 화면에서도 칠판과 액자 I/III를 함께 확인할 수 있다.
- [칠판 정면](stage6-chalk-front.png)
- [실제 E 확대](stage6-chalk-inspect.png)
- [천을 걷은 III 근접](stage6-frame-III-close.png)
- [II 근접](stage6-frame-II-close.png)
- [칠판과 액자 넓은 화면](stage6-chalk-frames-wide.png)

## 남은 범위
실제 HMD 가독성/실물 VR 플레이와 S11 플레이어 빌드는 미검증이다. 기존 라이선스 토큰 갱신 및 HMD 없는 OpenXR 초기화 경고는 별도 하드웨어 검증 통과로 간주하지 않는다. Notion 갱신은 수행하지 않았다.
