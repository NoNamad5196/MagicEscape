# MagicEscape — 아이디어·결정 일지 (Notion 백업)

> Notion 허브: 💡 아이디어 일지 › `MagicEscape — 마법학교 3D 방탈출 게임`. Notion 접근 실패 시 이 파일에 먼저 기록한다.

## 2026-11-10 — 기획·설계·코드 1차

### Notion 참고 문서 분석
- **싼데비슷한/SSANDEVISTHAN**: 정본 콜아웃 → 우선순위 → 범위(v1 필수/이후/제외) → 단계별 완료 조건 → 하위 '현행 문서'와 '과거 기록' 분리. "검증하지 않은 것은 unknown" 원칙. → MagicEscape는 메인 허브 + 하위 기술 문서, 계획/완료 구분, 미검증 표기를 채택.
- **KUREND**: 목적 콜아웃 → 체크리스트 형태의 작업 목록 → 작업 순서(단계별 체크) → 구조도(코드 블록) → "최종 기준" 콜아웃. → 퍼즐별 수용 기준 체크리스트, Windows 작업 단계표(S1~S11), "시연 가능 판정 기준"을 같은 형식으로 사용. KUREND 캐릭터·이미지는 재사용하지 않음.

### 주요 결정
| 결정 | 이유 | 대안 |
|---|---|---|
| Unity 6.3 LTS + URP | 현행 LTS(2025-12-04), 지원 기간 | 6.0 LTS(호환은 유지) |
| PC 1인칭, XR 보류 | VR 필수 여부 미확정 | 확정 시 입력 계층만 교체 |
| 규칙=Core 순수 C#, 연출=MonoBehaviour | 서버에서 테스트 가능, VR 전환 용이 | 단일 MonoBehaviour(테스트 불가) |
| PuzzleManager를 GameManager에 통합 | 진행 정본이 하나(ProgressionModel) | 별도 매니저(중복 상태 위험) |
| 입력 이중 백엔드(#if) | 템플릿마다 Active Input Handling 상이 | Input Actions 에셋(.inputactions는 서버에서 만들 수 없음) |
| 퍼즐 체인: P1→수정, P2→물약+수정, P3에서 사용 | "세 미니게임"이 아니라 하나의 탈출 과정 | 독립 퍼즐 3개 |
| 오답 시 즉시 리셋(P1), 재료 원위치(P2), 소모 없음(P3) | 실패 비용을 낮춰 10~15분 유지 | 시간 페널티 |
| Unity API 스텁 컴파일 검사 추가 | Unity 없이 Runtime 오타·시그니처 오류 조기 발견 | 검사 없음 |

### 진행 기록
- 저장소 clone(빈 저장소) → AGENTS.md 설계 정본 → Core + 테스트 34/34 → Runtime 22파일 + 스텁 컴파일(New/Legacy 0 오류) → 문서 9종.
- 문제: 서브에이전트 동시 실행 슬롯 부족으로 위임 작업이 큐에서 30분 시간 초과 → 메인 에이전트가 직접 작성으로 전환.
- 문제: Perplexity deep/quick 검색 시간 초과(에셋 조사) → Exa 웹 검색으로 에셋 페이지·라이선스 원문 직접 확인.
- 문제: 서버에 git push 자격 증명 없음 → 환경 변수 토큰을 일회성 credential helper로 사용(디스크 저장 없음).
- 발견: Unity는 MonoBehaviour 파일명=클래스명 필수 → SealInscription/SealCore를 별도 파일로 분리.

### 다음
- Windows: UNITY_HANDOFF S1~S11, TEST_PLAN 결과 기입, PPT 캡처.
- 교수님 확인: VR 필수 여부, 일반 PC 3D 허용 여부.
