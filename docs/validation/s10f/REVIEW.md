# S10f 자체 검토 / 인계

## 변경과 실제 검증
- 기준 `04d076d`, main. 기존 Core 판정/정답/ID/거리/공간/상태 머신 불변. 표시 전용 KoreanParticle 추가, Runtime 변경은 CodeDrafts 동기화.
- 동일 석판 6개/황동 각인, 세피아 액자·분필 칠판·레시피·비문. 액자 순서 III, I, IV, II 유지. 레시피 중앙 세로선/주황 괄호 제거.
- Unity Labs 플립북 불, 불씨/깜빡이는 라이트, 자체 노멀 물 셰이더·물줄기/드롭/링, 수정 내부 효과, 무쇠 가마솥. 봉인 진행마다 어두워지고 고요해짐. 차가운 야간 복도, 검정 페이드, 작은 금박 엔딩 카드/고운바탕. 기존 Ending/시간/버튼 보존.
- **Core 44/44, PlayMode 19/19, skipped 0, Console Error/Exception/Assert 0.** 셰이더 컴파일 오류 0. ColliderManifest 전후 동일. 기존 Core 소스가 기준 커밋과 동일, 바뀐 Runtime 미러 바이트 일치.
- 매회 메모리 게이트/단일 에디터 확인, 실행 전 최소 10.44GB. Windows RTX3080 batchmode로 확인했으며 노트북 FPS 테스트 아님.
- 01~34 기존 구도 + 35 실제 석판과 같은 .30m/.28m 시각 크기의 정면 2m 기호 전시 + 36 탈출 직전 복도 + 37 페이드 중간. gameplay Camera+HUD 1280×720. 35 전시물은 테스트 임시 메시로 저장 씬에 없음.
- 마지막 캡처 예산: emitter 22, active light 33, 실시간 그림자 light 2. 성능 통과 근거는 아님.

## 자체 판정 — 사용자 승인과 구분
1. **통일성 개선.** 원소별 가시/구체/고채도 오브젝트를 같은 석판/기호로 통일. 액자·칠판·레시피·비문이 같은 도안/세피아·황동으로 이어짐. 단, 자체 벡터/해칭은 실제 역사 사본 수준의 세밀함은 아니며 금박도 완전 PBR 묘사는 아님.
2. **2m 정면에서는 6종 구분 가능(35).** 실제 제단의 낮거나 반대편 각도에서는 위·아래 삼각형 방향을 읽어야 하므로 구분이 더 어렵다. 사용자 식별률/저해상도 기기 실험은 미실시.
3. **불은 확실히 개선.** 플립북의 자연스러운 경계/내부 구조가 05/13에서 보임. 초기 입자 방식의 보이지 않는 불꽃을 발견하고 한 장의 카메라 추종 카드로 교체/재캡처했다. 열 왜곡 미구현.
4. **물은 부분 개선, 아직 미흡.** 가마솥/수정의 반사·노멀/일렁임은 평평한 주황 원판보다 좋아졌다. 그러나 05의 물줄기는 가는 원통 실루엣이 강해 실사 물이라고 판정하지 않는다. scene-depth 투과·굴절 없음. 드롭/링 대비와 낙수 자연스러움은 추가 개선 후보.
5. **출구/엔딩 개선.** 새벽 주황광/백색 플래시/화면 가득 베이지 종이 제거. 차가운 복도와 단서 조명 유지, 검정 바탕 작은 금박 카드(28)/페이드(37). 안개·달빛은 정지 캡처에서 매우 약하다.
6. **s10e 대비 잃은 점:** 큰 원소 오브젝트/채도를 지워 즉각적인 색 구분은 감소(요구 의도). 비스듬한 각도에서 각인 식별은 큰 실루엣보다 주의를 요한다. 레시피는 명확하지만 전문 목판화의 장식 밀도에는 미달한다.

## 주황 사각형의 정체
UI가 아니라 `ExitDoor/SealLight_3/ArtB_C_SealLampLens`. S10d 빌더가 떠 있는 봉인등 구체를 대체하며 만든 주황 직사각 발광 렌즈(.09×.15×.025m), 위치 (.5, 3, 4.835). P1/P2 이후 세 번째 등만 남아 단독 사각형처럼 보였다. 의도된 진행 표시이므로 동작은 유지하고 렌즈 renderer를 끈 뒤 동일한 황동 불 기호로 교체. 마지막 봉인 해제 시 부모와 같이 소등된다.

## 새 에셋/라이선스
- Unity Labs Paris / Thomas Iché **Flame02**, CC0. [공식 고지](https://unity.com/blog/engine-platform/free-vfx-image-sequences-flipbooks), [직접 다운로드](https://unity3d.com/files/labs/downloads/vfx/assets01/Flame02/Flame02-flipbooks.zip), [CC0 법문](https://creativecommons.org/publicdomain/zero/1.0/legalcode.en) 직접 확인. TGA→PNG, Asset Store 사용 없음.
- [Noto Sans Symbols 원본](https://github.com/google/fonts/tree/main/ofl/notosanssymbols), [OFL 1.1 원문](https://raw.githubusercontent.com/google/fonts/main/ofl/notosanssymbols/OFL.txt) 다운로드/검사. cmap상 U+2609 없음, 지정 나머지 5개 있음. 따라서 6종 모두 같은 선 굵기로 직접 벡터 작성. 원본/검사/법문은 `Tools/FontSource/NotoSansSymbols/`, SHA-256은 source-hashes.json.
- 물 노멀/그림/기호 자체 제작. ambientCG Water002 후보는 404로 채택하지 않음. PD 역사 이미지 사용 없음. 기존 배경/폰트 라이선스 유지.

## 실행/재현 및 한계
```powershell
python Tools/Fetch-S10f.py
python Tools/Generate-Art.py
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10f-material-refine -Method MagicEscape.EditorTools.ClassroomBuilder.S10fApply
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10f-final-tests -Tests
dotnet test Tests/MagicEscape.Core.Tests --nologo
python Tools/Audit-S10f.py
```
- 현재 씬 위 S10fApply 반복 적용으로 검증. S1부터 재현하는 S10f() 경로는 제공했으나 전체 연속 재빌드는 별도 실행하지 않음. Linux RuntimeCompileCheck 스텁 검사도 이번 미실행, 실제 Unity 컴파일/PlayMode 사용.
- XML/Editor log는 로컬만 보관(환경 메타데이터), 공유 결과 results.json. 시작 시 LicenseClient Access token 경고는 entitlement 복구 뒤 종료 0, PlayMode Console 오류와 구분.
- 초기 파이프 인코딩 실수는 250bea7에서 수정 후 Core 44/44. 첫 PlayMode 17/19에서 물 emission keyword 호환 및 일반 Log까지 검사하던 수동 NoUnexpectedReceived 호출을 수정. 기본 Error/Exception 검사는 유지, 최종 전체 19/19.
- **물줄기 실사 품질, 실제 사용자 식별 실험, 노트북 60fps는 완료 아님.** 열 왜곡/scene-depth 굴절 미구현. 전체 품질을 전부 통과로 쓰지 않는다.
- 작업 시작부터 있던 exit-tests*.xml / s10-tests*.xml / s10-walk-diagnostic.xml 등 기존 untracked 파일은 변경/커밋하지 않았다.
