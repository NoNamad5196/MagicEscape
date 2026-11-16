# Warehouse Stage 8 — 작은 시각 오해 정리

## 변경 내용
- 칠판 I: 흙 아래 뾰족한 아몬드형 씨앗, 가운데 선, 위쪽의 작은 싹 한 가닥으로 교체했다. 사람의 머리/두 다리로 보이던 뿌리는 제거했다. II~IV와 같은 분필 색·선 체계를 유지한다. 씬 칠판과 E 확대는 같은 PNG를 사용한다.
- I 액자도 같은 씨앗 도안으로 맞췄다. Warehouse 전용 `Story_1.png`를 씬과 확대에 연결했으며 Classroom 원본 그림은 보존했다.
- 물 그림문자의 웃는 입 같은 곡선을 왼쪽 위 짧은 사선으로 교체했다. 제단과 액자 명판은 동일 재질/마스크를 공유한다. 나머지 다섯 그림문자도 직접 열어 얼굴처럼 읽힐 요소가 없는지 확인했다.
- 비늘 받침의 넓은 판 형태 지지대를 네 개의 독립 다리와 앞뒤 가로대로 바꿨다. 얇은 상판과 판자 이음새, 작은 무채색 병 두 개 및 막자사발/막자를 추가했다. 소품에는 픽업·조사·문양 컴포넌트가 없고 비늘 병의 붉은 비늘이 주목점으로 남는다.
- 비늘 위치 `(3.65, 1.30, .05)`, 상판 높이 1.20m 및 다른 모든 아이템 위치는 유지했다. 정답·RuneId·아이템 ID·퍼즐 흐름·치우기·어두워짐 코드는 변경하지 않았다.
- 현재 캡처 접두사를 `stage8-`로 옮기고 실행 래퍼가 stage7까지의 기존 증거를 보존하도록 했다.

## 실행한 검증
최종 수치와 캡처 SHA-256은 `stage8-summary.json`에 기록한다. Windows Unity 6000.3.28f1을 한 번에 하나씩 사용했고 씬·재질·메타는 Unity로 생성했다.

```powershell
python Tools/Generate-Warehouse-Art.py
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage8-generate -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage8-final-editmode -Tests -TestPlatform EditMode
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage8-playmode -Tests
dotnet test Tests/MagicEscape.Core.Tests --logger "trx;LogFileName=stage8-core.trx" --results-directory docs/validation/warehouse
```

새 EditMode 회귀 검사는 다리의 바닥 접촉, 상판과 다리 접촉, 판자 이음새 세 개, 소품의 지지·부착, 상호작용 컴포넌트 부재, 비늘 좌표, I 액자 씬/확대 텍스처 일치 및 물 문양 공유를 검사한다. 최초 실행은 테스트가 시각 오브젝트의 자동 `ArtB_` 접두사를 빠뜨려 11/12였다. 이어서 추가 부착 검사가 상판과 정확히 맞닿은 이음새의 부동소수점 경계 때문에 실패했다. 접두사와 2mm 접촉 허용오차를 적용한 뒤 전체 EditMode 재실행에서 모두 통과했다.

| 최종 검증 | 결과 |
|---|---|
| batchmode 씬 재생성 | 종료 0 |
| EditMode 전체 | 12/12, 실패 0 |
| PlayMode 전체 (Classroom 포함) | 38/38, 실패 0 |
| 순수 C# | 44/44, 실패 0 |
| 지지/부착 검사 | 통과 (접촉 허용오차 2mm) |
| 일곱 아이템 홈 | 이전 manifest와 동일 |
| 기존 미추적 XML/TRX 36개 | SHA-256 동일, 삭제/스테이징 안 함 |
| 기존 추적 캡처 및 Classroom 씬 | 변경 없음 |

## 직접 연 캡처
실제 게임 카메라와 HUD를 1280×720으로 렌더링했다. 아래 필수 다섯 장과 씬 칠판 정면을 직접 열어 확인했다. 씨앗은 두 다리 없는 씨앗으로, 물은 웃는 곡선 없는 물방울로 읽히고 비늘 아래에는 열린 다리 공간과 상판 이음새가 보인다.

- [칠판 확대](stage8-chalk-inspect.png)
- [씬 칠판](stage8-chalk-front.png)
- [I 액자](stage8-frame-I-close.png)
- [제단 PC 시점](stage8-altar-pc.png)
- [물 액자 명판](stage8-frame-II-close.png)
- [비늘 첫 시야](stage8-item-ing_salamander_scale.png)

기존 전체 캡처 테스트도 유지하므로 추가 액자·아이템·어두움 캡처가 함께 생성된다. 이 캡처는 실제 HMD 촬영이 아니다.

## 남은 범위
실제 HMD 착용·손 조작 및 S11 플레이어 빌드는 이번 범위 밖으로 미검증이다. 무기기 OpenXR 초기화와 라이선스 토큰 갱신 경고는 테스트 실패와 구분한다. 씬 YAML의 Unity 자동 생성 공백은 직접 편집하지 않았다.
