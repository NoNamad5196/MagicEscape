# MagicEscape 출구 및 S10 검증 인계

## 1부: 출구 통로
- `86c3dea` `fix(scene): 출구 통로 확장·봉인 장치 재배치` 커밋 및 origin/main push 완료. S10 착수 전에 완료했다.
- 문 유효 폭 3m, 북벽 3.3m 개구부, 외부 바닥 3.3×3m, 트리거 문 북쪽 1m, 봉인 장치 (-3,0,3.7). 기존 수정 보존 후 S9 빌더 재생성.
- 빌더 exit 0; `exit-tests-verified` PlayMode 12/12, 실패/skip 0. game Error/Exception/Assert 0. 중심/좌/우 W-only 탈출, E 입력/2.5m 중앙 Raycast 포함.
- OOM은 인계에서 확인된 Windows 커밋 메모리 고갈(페이지 파일 꺼짐) 환경 문제로 TEST_PLAN §3에 갱신. 실행 전 여유는 모두 6GB 이상(최초 약 9.1GB); 다른 앱/페이지 파일 설정을 변경하지 않았다.
- 최초 빌더 명령의 클래스 이름 오타를 올바른 namespace/메서드로 바로잡았다. 이후 정상 종료. 라이선스 handshake 로그는 초기에 남지만 Editor client 연결 이후 실제 빌더와 테스트가 완료됐다. 게임 Console 이벤트와 별개다.
- 수동: 마우스 체감/실제 UI 클릭/전체 플레이 외형. S11 빌드 없음.

## 2부: S10
- `UnityProject/Assets/Editor/ClassroomArtBuilder.cs`: S9부터 멱등 재구성. CC0 KayKit 테이블/벽선반/의자/양초, 자체 가마솥 메시, URP 재질·조명/창빛, Bloom/ACES/약한 Vignette, Depth Texture/HDR, 그림자 OFF, Emission 유지.
- 성공 반짝임/검은 연기/봉인 해제에 자체 제작 알파 텍스처와 ParticleSystem 튜닝. 외부 Asset Store VFX 미사용. AudioManager.clips에 SfxId 15종을 CC0 OGG 12개로 명시 연결.
- 기존 gameplay Collider 직렬화/월드 위치·크기/레이어 전후 일치 assertion. 기존 스크립트와 상호작용 참조를 보존(Audio clips 연결만 추가), 정답/itemId/GameText 변경 없음. YAML 수기 작성 없음. 모든 변경 Editor/테스트 C#은 CodeDrafts에 복사했다.
- `Tools/Run-Unity.ps1`: 커밋 여유 6GB 미만 또는 다른 Editor 실행 중이면 실행 거부. Editor 인스턴스는 항상 직렬 실행.
- 최종 emission 빌더 2회 구조 manifest SHA256 동일(idempotency.txt). source 재생성 결과이며 YAML 바이트 동일 보장은 아님.
- 테스트/캡처: `ArtTests.cs` 1건 추가, 기존 12건은 유지. 자동 검증 결과는 results.json 및 console-audit.txt. 4장은 실제 Camera.main+HUD의 1280×720 RenderTexture PNG다. batchmode에서는 화면 backbuffer 캡처가 파일을 쓰지 않아 이 방식으로 대체했으며 **네이티브 GameView 창을 촬영한 것은 아니다**.

### 실행 명령
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name exit-build-verified -Method MagicEscape.EditorTools.ClassroomBuilder.S9
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name exit-tests-verified -Tests
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10-readable-build -Method MagicEscape.EditorTools.ClassroomBuilder.S10
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10-readable-repeat-build -Method MagicEscape.EditorTools.ClassroomBuilder.S10
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name s10-tests-final-accepted -Tests
```

### 에셋/커밋 정책
- KayKit Dungeon(Remastered 동봉 1.0): FBX 5종/아틀라스/CC0 LICENSE, Kenney Interface Sounds 1.0: 사용하는 OGG 12종/CC0 License, Unity meta를 커밋. 출처와 라이선스는 VISUAL_REFERENCES §7.
- 자체 메시/파티클 텍스처/머티리얼도 Unity API로 생성해 커밋. Asset Store 원본, 로컬 라이선스·세션 로그, 캐시 미커밋. 유료 구매 없음. 패키지 변경/XR 추가 없음.
- 사용자 다운로드 목록(선택): Cartoon FX Remaster Free, 무료/Standard Unity Asset Store EULA. Unity 계정 My Assets → Package Manager Download/Import. JMO Assets/ThirdParty/CartoonFX 경로는 gitignore. 없어도 자체 VFX로 정상 동작한다. Quaternius 미사용.

### 스크린샷
- `docs/validation/s10/01-classroom.png`
- `docs/validation/s10/02-rune-success.png`
- `docs/validation/s10/03-cauldron-smoke.png`
- `docs/validation/s10/04-seal-release.png`

### 수동 확인 및 남은 문제
- **외형·밝기·폰트 가독성·음색/음량·실제 GameView 창 표시·마우스/실제 UI 클릭/조작감은 수동 확인 필요**. 사람의 시연/게임 외형 최종 승인을 대신하지 않는다.
- 일부 메시/재료·문·제단은 단순 도형을 유지한다. S10은 재현 가능한 아트 베이스라인이며 최종 미술 품질은 사용자 평가 필요. Kenney interface 대체음은 기능별 의미/음색 수동 청취가 필요하다.
- 최초 S10 전체 실행은 11/13: batchmode backbuffer 캡처 실패와 S9 보행 1회 실패(z=4.32/실시간 5초 제한). 캡처는 게임 카메라 렌더로 수정. 보행은 콜라이더/허용 거리/시간을 바꾸지 않고 진단 계측만 추가했고, 단독 1/1 및 후속 전체 13/13을 반복 확인했다. 최초 보행 실패의 독립 원인은 확정하지 못했으며 재발 관찰 필요.
- CC0 폴더 전체 제거 후 재실행은 별도 미검증. 빌더의 null-safe fallback 및 선택 Asset Store 미설치 경로를 제공한다.
- S11 빌드/10~15분 사용자 평가는 범위 밖. 원래 미추적 exit-resume 보고서/크래시 기록은 로컬에 보존.

- 추가 시각 결함: URP가 직렬화된 EmissionColor=black 머티리얼의 _EMISSION 키워드를 제거해 반짝임만 보이고 문양 발광이 안 나왔음. S10에서 Reactive 머티리얼의 미세 비검정 기본값/Emissive GI 플래그를 설정하고 런타임 색은 기존 GlowFeedback PropertyBlock이 계속 제어하도록 수정. ArtTests에 모든 GlowFeedback 렌더러의 저장된 _EMISSION 키워드 검증 추가.


- 에셋 커밋: 982f978 (art: CC0 가구·효과음 필수 에셋 추가), origin/main push 완료. 통합 구현/검증 커밋은 본 보고를 포함하는 art 커밋 참조.
- 발광 수정 후 캡처에서 비문판이 흰색으로 clipping되어 GlowFeedback의 시각 강도만 비문판 0.08/나머지 0.8로 낮췄다. Runtime 스크립트 코드/참조는 그대로다.


### 최종 승인 기준 실행 결과
- `s10-readable-build`, `s10-readable-repeat-build`: 각각 exit 0; 구조 manifest 동일, 기존 Collider/레이어/월드 변환 불변.
- `s10-tests-final-accepted`: exit 0, **13/13 통과, failed/skipped 0**, 게임 Console **Error/Exception/Assert 0**. 변경 후 Emission 키워드 검사 및 Game camera+HUD 캡처 4장 포함.
- 모든 Unity Editor 종료 확인. 원본 로그와 XML은 로컬 보존, 저장소에는 식별정보 없는 results.json/console audit/스크린샷/구조 manifest만 포함.
