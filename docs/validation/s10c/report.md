# S10c — 3차 아트 패스

2026-11-12 KST, Unity 6000.3.28f1 / URP 17.3.0, Windows. 기준 씬은 S10b이며 `ClassroomBuilder.S10c()`가 S10b부터 재구성한다.

## 변경 내용

- `Assets/Editor/ClassroomThirdArtBuilder.cs`: 거의 검정인 푸른 ambient, 제한된 촛불·랜턴 빛 웅덩이, 액자별 그림 조명, 칠판·소켓·가마솥 전용 조명. 창문은 별/달 emission 유리+납 창살이며 달빛 스포트, 소프트 가산 빛줄기 2장과 최대 55개 먼지를 연결했다.
- `AtmosphereMotion.cs`: 따뜻한 광원의 약한 flicker, 걷는 동안 **빛만** 미세하게 움직이는 카메라 부착 랜턴. 카메라 시점·입력을 변경하지 않는다. 봉인핵은 각진 흑요석 형태와 회전하는 두 룬 고리, emission 맥동, 작은 파티클, 포인트 라이트로 교체했다. 기존 SealStage를 읽어 준비/해제 색상만 바꾼다.
- `Art/MoonShaft.shader`, `NearRim.shader`: 소프트 가산 빛줄기와 2.5m 이내의 절제된 청록 림. 기존 GlowFeedback과 정답/오답 발광은 유지한다. 룬 고리의 작은 48개 조각은 고리당 하나의 메시로 합쳤다.
- 진홍 직물+금색 학원 문장/테두리 배너, 기둥 석재·받침, 랜턴 금속, 창가 선반 받침과 화분 재질을 정리했다. 소켓 수정과 겹치던 장식 촛불도 옮겼다. 장식에는 콜라이더를 추가하지 않았다.
- 기존 Quaternius 병 모델에 구별되는 내용물/색과 비늘 표식을 붙였다. 월광초는 꽃잎, 독버섯은 갓·줄기·반점, 수정은 6면체+작은 수정군으로 제작했다. 사파이어=파랑/루비=빨강 유지.
- 아이템 7종을 실제 3D 표현으로부터 Editor Camera/RenderTexture에 렌더한 256px 투명 PNG로 교체했다. `Assets/Art/S10c/Icons/`와 ItemData.icon 연결을 저장했다.
- 칠판은 원문을 유지하고 공백 위치에만 표시용 줄바꿈을 넣었다. 자동 줄바꿈을 끄고 TMP 폭·크기를 조정했다. 비문은 원문 문자열을 유지한 채 폭/크기를 조정했다. 핫바와 선택 이름은 우하단으로 이동해 봉인 받침대 중앙을 가리지 않는다.
- ACES/Bloom/Vignette 0.33/약한 Film Grain·Chromatic Aberration, 청록 그림자·주황 하이라이트 Split Toning, 낮춘 채도와 청록 Exponential Fog. HDR/Depth/Soft Shadows 활성화. 실시간 그림자는 달빛/플레이어 랜턴 **2개만**, 1024 atlas/각 Medium 해상도, 거리 16m로 제한했다.
- `RoomAmbience.cs`: CC0 드론·바람의 낮은 음량 루프와 드문 위치성 나무 삐걱임. 타이틀 제작자 표기와 `docs/CREDITS.md`, VISUAL_REFERENCES, 동봉 CC0 법률 원문/파일별 크레딧 추가.

## 실제 검증

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10c-release-build -Method MagicEscape.EditorTools.ClassroomBuilder.S10c
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10c-release-repeat -Method MagicEscape.EditorTools.ClassroomBuilder.S10c
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10c-tests-release -Tests
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10c-tests-final -Tests
```

- 최종 빌더 두 번 exit 0, 구조 manifest SHA256 동일: `idempotency.txt`. 기존 콜라이더 JSON/월드 위치·스케일/레이어와 기존 gameplay MonoBehaviour 설정·참조가 S10b 대비 동일한지 빌더에서 비교했다: `builder-audit.txt`.
- `s10c-tests-release`: **13/13**, 실패/skip 0, exit 0. 최종 실행의 식별정보 제거 결과는 `results.json`, 실제 Error/Exception/Assert 이벤트 수는 `console-audit.txt`에 저장한다. 원본 로그/XML은 ignored이며 커밋하지 않는다.
- 마지막 `s10c-tests-final`도 **13/13**, 실패/skip **0**, exit **0**, 실제 Error/Exception/Assert **0**. 최종 4개 빌더/테스트 로그의 compiler/exception/shader 오류 및 그림자 atlas 축소 경고 **0**. `python Tools/Audit-S10c.py`로 원본에서 식별정보 없는 결과·PNG hash·에셋 목록·소스 미러 동일성을 다시 생성할 수 있다. 임시 InitTestScene은 정상 종료 후 남지 않았다.
- 테스트에는 합성 키보드/휠/2.5m 중앙 Raycast E 풀이, W-only 중심/좌/우 출구 이동이 포함된다. 카메라 위치는 검사 장소별로 배치한다. 사람이 모든 구간을 걸어 플레이한 결과로 쓰지 않는다.
- 기존 emission 키워드/글리프 검사에 더해 새 shader 지원 여부, 그림자 라이트 2개 제한, 루프 소스 실제 재생 상태·클립 연결, 룬 고리 회전, 아이콘 크기, 칠판 원문·표시 폭·NoWrap을 검사했다. 비문 공개 후 글리프 bounds와 아이콘/문장 분리 검사를 유지했다.
- Unity 실행은 모두 Run-Unity.ps1의 `FreeVirtualMemory/1MB >= 6GB` 및 기존 Editor 프로세스 없음 게이트를 통과했다. 마지막 빌더/반복/테스트 여유는 각각 13.32/13.87/13.72GB, 마지막 감사 경로 정리 후 테스트는 13.67GB였다. 동시 Editor 실행 없음.
- Core/GameText/PlayerInteraction/PlayerController/퍼즐 로직/방 생성/패키지 파일 변경 없음을 git diff로 확인했다. 정답·itemId·2.5m·12×10×4.5m·3m 통로 유지. Runtime 새 파일 두 개와 Editor/테스트 소스는 CodeDrafts에도 복사했다. **Assets/Scripts가 정본**이다.

## 스크린샷과 직접 시각 점검

실제 Camera.main + HUD를 1280×720 RenderTexture로 렌더했다. SceneView나 별도 홍보 카메라가 아니다. 01~07은 S10b와 동일한 위치/타깃을 사용한다. PNG 14장을 직접 열어 검토했다.

| 파일 | 확인 |
|---|---|
| 01-classroom ~ 07-student-desks-window | 기존 7구도: 교실/액자벽/제단/가마솥/공개 비문/출구/책상·창가 |
| 08-corridor-lantern-only | 다른 실시간 조명과 ambient를 캡처 중에만 끈 복도. 창 유리 자체 emission은 유지. ExitTrigger만 일시 비활성화해 클리어 UI가 덮이지 않도록 한 뒤 복구 |
| 09-window-moonlight | 창살 유리, 밤하늘/달, 빛줄기, 먼지, 꽃잎·선반 |
| 10-painting-closeup | 액자/로마숫자. 02에서 III/I/IV/II 전체 순서 구별 가능, 10에서 I 및 주변 숫자·문양 선명 |
| 11-chalkboard-closeup | 문장 전체 읽힘. ‘순서로’, ‘액자의’ 단어 내부 분리 없음 |
| 12-inscription-closeup | 문장 전체 읽힘. 물/불 문양과 글자가 겹치지 않음 |
| 13-seal-ready-crystal-colours | 왼쪽 파란 수정/오른쪽 붉은 수정 구분, 준비 상태 룬 발광, 받침대와 핫바 분리 |
| 14-hotbar-model-icons | 7종 아이콘. 테스트에서 카탈로그 아이템을 채워 전체 실루엣/색을 확인 |

**가독성 판정:** 액자 숫자, 칠판 원문, 공개 비문, 수정 색상 모두 해당 일반/근접 구도에서 읽히고 구별된다. 어두운 모서리·천장과 달리 퍼즐 영역은 별도 빛/텍스트/발광으로 남았다. 실제 프로젝터의 검정 레벨과 주변광까지 보증한 결과는 아니다.

## 작업 중 발견·수정

- URP additionalLightsShadowResolutionTier가 읽기 전용이라 중간 빌더에서 CS0200 1종 발생. Editor SerializedObject API로 설정하도록 수정했다. 최종 빌더/테스트에는 컴파일 오류 없음.
- 최초 atlas 설정에서 그림자 해상도 자동 축소 경고를 확인해 두 광원을 Medium으로 제한하고 Soft Shadows를 켰다. 최종 로그에 해당 경고 없음.
- 최초 복도 캡처가 ExitTrigger를 밟아 클리어 UI에 가려지는 것을 직접 보고 캡처 격리/복구로 고쳤다. 실제 탈출 기능/테스트는 변경하지 않았다.
- 아이콘 수정 꼭짓점이 잘리던 구도, 지나치게 회색이던 병, 소켓 수정과 촛불의 겹침, 뒤집힌 복도 창 UV를 수정하고 다시 생성/촬영했다.
- Editor 초기 licensing bootstrap의 access-token 갱신 메시지는 남지만 Editor는 entitlement 확인 후 exit 0이었다. 이를 게임 Console Error 수와 혼동하지 않는다. 라이선스/환경 식별정보가 포함된 원본 로그는 배포하지 않는다.

## 남은 확인

- 실제 프로젝터, 일반 노트북 **60fps**, 사용자 마우스 조작감·10~15분 전체 플레이, 실제 스피커/헤드폰의 음색·음량은 미검증이다. S11 실행 파일 빌드도 수행하지 않았다.
- 자동 오디오 검사는 clip/loop/isPlaying과 낮은 설정 음량까지다. 실제 사람이 청취했다고 주장하지 않는다.
- 기존 미추적 테스트 XML 10개는 건드리지 않았다. Unity의 임시 InitTestScene은 정상 테스트 종료 시 제거되었는지 확인한다. YAML은 Unity API로만 생성했으며 자동 직렬화의 빈 필드 trailing whitespace는 손으로 정리하지 않는다.

## 커밋

- `1546008` — CC0 앰비언트/라이선스/크레딧
- `d0e6e50` — 조명·Volume·창문·배너·소품·봉인핵·아이콘/생성 씬
- 검증/문서 커밋은 후속 git log와 최종 인계에 기록한다.
