# Warehouse Stage 7 — 장면으로 찾고 이름으로 누르는 퍼즐

## 변경 내용과 판단
- 액자 로마숫자를 제거했다. 칠판 번호별 장면 → 같은 액자 → 명판 원소 → 같은 문양석으로 푼다. 정답 대지→물→불→빛, RuneId·아이템 ID·P1→P2→P3는 변경하지 않았다.
- 칠판은 씨앗/비/가뭄/빛 속 꽃을 크게 그린 분필 장면이다. 원소 이름·독립 원소 그림문자는 없다. 씬과 E 확대가 같은 `ChalkScenes.png`를 참조한다.
- 산·물방울·불꽃·광선 해·바람결·초승달을 단색 11px 선과 동일 황동 각인으로 통일했다. III 명판은 불꽃/불, IV는 광선 해/빛이다.
- 문양석 이름 표면 .33×.11 → .38×.15m, 액자 이름 .36×.12 → .48×.16 local로 확대했다. Gowun Batang과 채움·발광은 유지한다. 시각 석판만 .41×.49m로 늘리고 입력 콜라이더·판정은 유지했다.
- 비늘은 동쪽 통로를 향한 유리병 선반, 독버섯은 축축한 나무통, 가루는 .60m 받침 위 궤짝으로 옮겼다. 푸른 수정은 제단 바로 위로 올렸다. 월광초는 요청대로 그대로다. 물건별 좌표·높이·이유는 [현행 위치표](../../WAREHOUSE_XRI.md#stage-7-item-homes-world-metres)에 있다.
- **P2 보상 좌표는 유지했다.** 기존 (2.50,1.05,1.65)/(2.90,1.05,1.65)가 이미 가마솥 바로 옆 받침이므로 통로로 옮기지 않았다. 대신 수정 불투명도 .28→.65, 국소 조명과 등장 신호로 가시성을 높였다.
- 모든 물건에 .55 중성 국소 빛과 4.8초 간격의 작은 펄스를 붙였다. 보상은 .65초 빛과 기존 픽업음으로 등장한다. 마지막 어두움에서도 유지한다. `WarehouseItemBeacon`이 어두움 컨트롤러 뒤에서 조명을 소유한다.
- 보상 스포너는 일회 등장으로 제한했다. 재료는 Awake에 기록된 새 홈으로 리스폰한다.
- 높인 궤짝의 165도 뚜껑이 III 명판 앞에 수평 가림판이 되는 것을 캡처에서 발견했다. 100도 열림으로 고쳤고, 열린 상태에서도 불꽃/불이 보이는 추가 캡처를 남겼다.
- Warehouse 봉인등·비문까지 예전 연금술 마스크를 교체했다. 조합 그림도 기호 없는 Warehouse 전용 재료 그림을 쓴다. 옛 빈칸 칠판 PNG/재질/생성기는 제거했다. Classroom의 역사적 리소스는 삭제하지 않았다.
- AGENTS·README·WAREHOUSE_XRI와 [Notion 허브](https://app.notion.com/p/4147cdef782d81da9895f389197b0300)의 날짜 붙은 현행 갱신을 동기화했다. 과거 교실판 기록은 보존했다.

## 실제 최종 검증
Windows Unity 6000.3.28f1 Editor를 한 번에 하나씩 실행했다. YAML·메타는 Unity가 생성했고 직접 작성하지 않았다.

| 검증 | 통과 | 실패 | 로컬 증거 |
|---|---:|---:|---|
| 전체 씬 batchmode 재생성 | 종료 0 | — | stage7-final-generate.log |
| EditMode 전체 | 11 | 0 | stage7-final-editmode.xml |
| PlayMode 전체, Classroom 포함 | 38 | 0 | stage7-final-playmode.xml |
| 순수 C# | 44 | 0 | stage7-core.trx |

원본 로그/XML/TRX는 로컬에만 두고 `stage7-summary.json`에 정제한 결과와 캡처 해시를 남긴다. 초기 전체 PlayMode는 새 리스폰 테스트의 Vector3 완전 동일 비교 때문에 37/38이었다. 미세 부동소수점 오차를 허용하도록 .001m 한계로 수정했다. 위 표는 최종 소스/재생성 씬으로 다시 실행한 결과다.

### 재발 방지
- 검토한 장면 전용 칠판 PNG SHA-256, 씬/확대 동일 참조, 원소 이름·문양 컴포넌트 부재.
- 네 액자의 모든 TMP 숫자 텍스트 부재와 그림 아래 명판 위치.
- 7개 물건 홈과 .5m 안의 광원, 어두움에서 감쇠되지 않는 조명 등록. **월광초 .78m는 위치 유지 요청에 따른 명시적 예외**, 나머지는 .8~1.6m.
- 렌더러뿐 아니라 `AssetDatabase.GetDependencies` 재귀 결과에도 옛 6개 연금술 PNG 참조 부재.
- 실제 XRI 오답 투입 후 새 나무통 위치와 기존 꽃 위치로 복구.
- 보상 등장 알림 각 1회, 플레이어 눈높이 물건 가시성, 마지막 어두움의 보상 가시성.
- Classroom 씬과 기존 검증 파일 237개는 최종 테스트 전후 해시 동일. 기존 미추적 XML/TRX 36개도 해시 동일, 삭제·스테이징하지 않았다. 테스트 래퍼는 예전 Warehouse 캡처도 보존하도록 경로 예외를 파일 영역으로 좁혔다.

```powershell
python Tools/Generate-Warehouse-Art.py
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage7-final-generate -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage7-final-editmode -Tests -TestPlatform EditMode
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage7-final-playmode -Tests
dotnet test Tests/MagicEscape.Core.Tests --logger "trx;LogFileName=stage7-core.trx" --results-directory docs/validation/warehouse
```

## 캡처 직접 확인
최종 19장을 전부 직접 열어 확인했다. 게임 카메라/HUD를 1280×720으로 렌더링했다. 서 있는 높이 캡처는 PC 카메라 피벗 1.75m이며 실제 HMD 촬영은 아니다. 물건은 각 물건을 처음 볼 수 있는 통로/작업 자리에서 촬영했다.

지도는 실제 씬의 진단용 컷어웨이다. 지붕 일부 숨김·물건 홈 복구·지도 전용 보조광/번호는 촬영 동안만 적용하며 씬에 저장하지 않는다. 번호는 1 붉은 수정 / 2 푸른 수정 / 3 가루 / 4 꽃 / 5 비늘 / 6 독버섯 / 7 물약이다.

- [stage7-altar-pc.png](stage7-altar-pc.png)
- [stage7-altar-standing.png](stage7-altar-standing.png)
- [stage7-chalk-frames-wide.png](stage7-chalk-frames-wide.png)
- [stage7-chalk-front.png](stage7-chalk-front.png)
- [stage7-chalk-inspect.png](stage7-chalk-inspect.png)
- [stage7-frame-I-close.png](stage7-frame-I-close.png)
- [stage7-frame-II-close.png](stage7-frame-II-close.png)
- [stage7-frame-III-chest-open.png](stage7-frame-III-chest-open.png)
- [stage7-frame-III-close.png](stage7-frame-III-close.png)
- [stage7-frame-IV-close.png](stage7-frame-IV-close.png)
- [stage7-item-crystal_ruby-darkest.png](stage7-item-crystal_ruby-darkest.png)
- [stage7-item-crystal_ruby.png](stage7-item-crystal_ruby.png)
- [stage7-item-crystal_sapphire.png](stage7-item-crystal_sapphire.png)
- [stage7-item-ing_crystal_dust.png](stage7-item-ing_crystal_dust.png)
- [stage7-item-ing_moonpetal.png](stage7-item-ing_moonpetal.png)
- [stage7-item-ing_salamander_scale.png](stage7-item-ing_salamander_scale.png)
- [stage7-item-ing_toadstool.png](stage7-item-ing_toadstool.png)
- [stage7-item-map.png](stage7-item-map.png)
- [stage7-item-potion_revelation.png](stage7-item-potion_revelation.png)

## 남은 범위
실제 HMD 착용 가독성·손 조작·VR 플레이테스트와 S11 플레이어 빌드는 미검증이다. OpenXR 무기기 초기화 및 라이선스 갱신 경고는 테스트 통과와 구분한다. 오래된 XRI 비대응 RuntimeCompileCheck 스텁은 이번 검증 수단으로 사용하지 않았다. 현재 확인된 자동 테스트 실패는 없다.
