# 창고 5단계 — 기호의 뜻을 이름으로 명시

## 변경과 판단
- 문양석 6개와 액자 4개에 같은 황동 각인 셰이더로 대지·물·불·바람·빛·그림자 이름을 추가했다. 기존 기호와 I~IV 번호판, 씨앗의 일생 그림 파일은 유지했다.
- 폰트는 프로젝트의 **Gowun Batang Regular**. 112px 글자를 384×128 마스크로 굽고 2px 보강, 비압축·밉맵 없음으로 가는 명조 획의 소실을 막았다. 돌 이름 면은 33×11cm, 액자 이름 면은 월드 기준 30.6×10.2cm다. 돌의 시각 석판만 36×43cm로 늘렸으며 콜라이더·문양 위치는 그대로다.
- 이름은 기호와 같은 `_Fill`·`_EmissionColor`를 받아 누름/성공/실패 발광에 함께 반응한다. Runtime과 CodeDrafts 미러가 일치한다.
- 원소표 본체·낱장·확대용 그림/머티리얼·전용 생성/검사 코드를 제거했다. 책 아래는 조사 기능 없는 제본용 나무판 소품이다.
- III은 기존 **(-4.39, 1.65, -1.75)** 위치를 유지했다. II와 같은 벽걸이 높이이며 프레임 상단에 걸친 주름 천을 걷어야 조사할 수 있다. I·IV는 상자 밀기, 수정 가루는 궤짝 열기다.
- 3회 실패 힌트는 원소표를 언급하지 않아 **변경하지 않았다**: “벽에 걸린 액자의 숫자 순서를 따라가 보자.”
- 정답 대지→물→불→빛, RuneId/아이템 ID, P1→P2→P3, 4단계 어두워짐과 조명 프로필은 변경하지 않았다. AGENTS.md·README.md·WAREHOUSE_XRI.md를 갱신했다.

## 실제 검증
Windows / Unity 6000.3.28f1, 한 번에 Editor 하나만 실행했다.

| 검증 | 통과 | 실패 | 로컬 원본 증거 |
|---|---:|---:|---|
| 전체 씬 batchmode 재생성 | 종료 0 | — | stage5-generate4.log |
| 최종 EditMode 전체 | 7 | 0 | stage5-final-editmode.xml |
| 최종 PlayMode 전체: Classroom + Warehouse | 36 | 0 | stage5-final-playmode.xml |
| 순수 C# | 44 | 0 | stage5-core.trx |

- `DesktopClearingAllFourAndHiddenTargetsCannotBeBypassed`: III을 포함한 숨김 대상의 콜라이더/XRI/직접 조사 차단, 실제 E로 천 걷기, 걷은 뒤 III 확대 조사와 접근 허용.
- `NamedIncisionsAreReadableAndShareFillAndGlow`: 6개 이름, 액자/돌의 동일 이름 머티리얼, 이름의 실제 크기, 기호/이름의 채움·발광 동기화.
- `DesktopFullFlowTurnsOffThreeSealsAndOpensExit`: 실제 마우스/키보드 XRI로 치우기→P1→P2→P3→출구 통과. 기존 VR 합성 컨트롤러·공간 부착·4단계 조명 회귀 검사도 전체 통과.
- 기존 XML/TRX 82개의 SHA-256이 모두 그대로다. 처음부터 미추적이던 36개 XML/TRX는 삭제·스테이징하지 않았다. 이전 캡처·Classroom·공유 폰트/에셋은 실행 도구가 복원했다.
- 환경 정보 없는 테스트 이름·결과 및 최종 캡처 해시는 [stage5-summary.json](stage5-summary.json)에 기록했다. 원본 XML/TRX/로그는 로컬에만 남겼다.

## 캡처 직접 확인
1280×720 실제 게임 카메라/HUD 캡처를 모두 직접 열었다. 제단 중심에서 전방 1.4m PC 시점과 눈높이 1.75m 서 있는 시점에서 여섯 이름을 읽을 수 있다. 초기 얇은 글자 시안은 폐기하고 획 보강·비압축 마스크로 재검증했다. HMD 촬영은 아니다.

- [제단 앞 PC](stage5-altar-pc.png)
- [서 있는 눈높이](stage5-altar-standing.png)
- [III 천 걷기 전](stage5-III-before.png)
- [III 천 걷기 후](stage5-III-after.png)
- [III 근접: 기호·불·번호판](stage5-frame-III-close.png)

## 재현 명령
새 실행에서는 기존 증거를 덮어쓰지 않도록 `-Name`과 결과 파일명을 바꾼다.

```powershell
python Tools/Generate-Rune-Names.py
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage5-generate4 -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage5-final-editmode -Tests -TestPlatform EditMode
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage5-final-playmode -Tests
dotnet test Tests/MagicEscape.Core.Tests --logger "trx;LogFileName=stage5-core.trx" --results-directory docs/validation/warehouse
```

## 남은 범위
자동 검사 실패는 없다. 실제 HMD의 해상도·눈높이·가독성과 실물 VR 전체 플레이는 미검증이다. 서 있는 높이 캡처와 합성 컨트롤러 통과를 HMD 검증으로 간주하지 않는다. S11 플레이어 빌드와 Notion 갱신은 수행하지 않았다. 기존 라이선스 토큰 갱신 경고와 HMD 없는 OpenXR Display 초기화 경고는 남지만 batchmode 종료·검사 결과는 정상이다.
