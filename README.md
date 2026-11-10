# MagicEscape — 마법학교 방탈출

마법 실습 중 실수로 '삼중 봉인'을 발동시킨 견습 마법사가 교실의 단서를 조사하고 퍼즐 3개를 풀어 탈출하는 **3D 1인칭 퍼즐 방탈출** 게임입니다. (대학 '멀티미디어와 가상현실' 기말 작품)

| 항목 | 내용 |
|---|---|
| 엔진 | Unity 6.3 LTS · URP · uGUI/TextMeshPro · Input System(구 Input Manager도 지원) |
| 플랫폼 | Windows PC, 키보드 + 마우스 |
| 플레이 | 10~15분, 교실 1개, 퍼즐 3개 |
| 상태 | 설계·C# 코드 초안 완료 / **Unity 씬 제작·실행은 Windows에서 진행 예정(미검증)** |

## 퍼즐 흐름

1. **마법 문양 해독** — 벽의 액자 4점 번호를 읽고 제단 문양을 순서대로 누른다.
2. **마법약 제조** — 마법서 조합법대로 재료 3종을 가마솥에 넣는다(독버섯 함정).
3. **최종 봉인 해제** — 물약으로 비문을 드러내고 두 수정을 맞는 소켓에 끼운 뒤 봉인핵을 활성화한다.

## 저장소 구조

```text
AGENTS.md            작업 정책 + 설계 정본(정답·아이템 ID·상태)
CodeDrafts/Core      엔진 독립 규칙(순수 C#) — 퍼즐 3종, 진행, 인벤토리, 문구
CodeDrafts/Runtime   Unity MonoBehaviour — 플레이어, 상호작용, 퍼즐 연출, UI, 오디오
Tests/               dotnet xUnit(Core 34건), Unity API 스텁 컴파일 검사
docs/                기획서·퍼즐 명세·기술 설계·씬 구성·인수인계·테스트·에셋
```

## 문서

- [게임 기획서](docs/GAME_DESIGN.md) · [퍼즐 명세](docs/PUZZLE_SPEC.md) · [기술 설계](docs/TECH_DESIGN.md)
- [씬·Inspector 구성](docs/SCENE_SETUP.md) · [**Windows 인수인계**](docs/UNITY_HANDOFF.md) · [테스트 계획](docs/TEST_PLAN.md)
- [비주얼·에셋](docs/VISUAL_REFERENCES.md) · [작업 현황](docs/TASKS.md) · [아이디어 일지](docs/IDEA_LOG.md)

## 서버에서 검증하기

```bash
dotnet test Tests/MagicEscape.Core.Tests                           # Core 규칙 34건
dotnet build Tests/RuntimeCompileCheck -p:InputBackend=New         # Runtime 스텁 컴파일
dotnet build Tests/RuntimeCompileCheck -p:InputBackend=Legacy
```

Unity 적용 방법은 [UNITY_HANDOFF.md](docs/UNITY_HANDOFF.md)를 따릅니다. `Tests/RuntimeCompileCheck/UnityStubs.cs`는 Unity 프로젝트에 복사하지 마세요.
