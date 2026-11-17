# Stage 9 - image-generated Warehouse P1 clues

## Changes
- Replaced the four procedural chalk scenes and all four frame paintings with built-in image-generation outputs. All eight saved textures are 1024 square; no fallback scene drawing or external image API was used.
- Files under `UnityProject/Assets/Art/Warehouse/Textures/`: `Chalk_1.png`, `Chalk_2.png`, `Chalk_3.png`, `Chalk_4.png`, `Story_1.png`, `Story_2.png`, `Story_3.png`, `Story_4.png`. Prompt set and reviewed SHA-256 values are in that directory's `README.md`.
- Subjects stay seed with a large two-leaf shoot, rain, hot sun, open flower. Thick monochrome chalk silhouettes correspond to frame-filling dark oil paintings. Visually reviewed all eight: no fake writing, numbers, signatures, alchemical glyphs, faces or extra protagonists.
- `Generate-Warehouse-Art.py` now composites the generated chalk assets into `ChalkScenes.png`; it no longer draws P1 scenes/frame I procedurally. World-board Roman captions are separate authored text above the pictures, never inside generated art. Enlargement shows the same picture plate in left-to-right order.
- `WarehouseBuilder` and `WarehouseStage7Builder` assign generated art without changing the answer Earth -> Water -> Fire -> Light, frame/plaque/stone motifs, frame positions, existing lighting or III cloth. All four world paintings and inspection overlays share their corresponding story textures.
- Scene regenerated through `ClassroomBuilder.Warehouse`. Added an EditMode contract for all four generated references, 1024 sizing and separate caption positions; updated the approved chalk plate hash. Capture names now use stage9, and the runner preserves historical stage8 evidence.

## Actual verification

| Check | Result | Evidence |
|---|---|---|
| Unity 6000.3.28f1 Warehouse batch generation | Exit 0 | `stage9-build.log` |
| Full EditMode | 13/13 passed, no failures/skips | `stage9-editmode.xml`, `.log` |
| Full PlayMode | 38/38 passed, no failures/skips | `stage9-playmode.xml`, `.log` |
| Final focused visual capture after overview-camera tuning | 1/1 passed | `stage9-visualreview.xml`, `.log` |
| Pure C# Core | 44/44 passed | `stage9-core.trx` |

```powershell
python Tools/Generate-Warehouse-Art.py
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage9-build -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage9-editmode -Tests -TestPlatform EditMode
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage9-playmode -Tests -TestPlatform PlayMode
dotnet test Tests/MagicEscape.Core.Tests --logger 'trx;LogFileName=stage9-core.trx' --results-directory docs/validation/warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage9-visualreview -Tests -TestPlatform PlayMode -TestFilter WarehouseStage7Tests
```

## Screenshots
Actual gameplay-camera/HUD renders at 1280x720, visually inspected. Closeups show the chalkboard and each frame; an extra inspection capture verifies the board overlay. The overview uses a temporary elevated wide-angle camera only for capture and retains authored storage-crate occlusion; camera settings are restored afterward. No scene props or lighting were moved for the evidence.

| File | SHA-256 |
|---|---|
| [stage9-chalk-front.png](stage9-chalk-front.png) | `314c8a32f2296e0766406bf71d35aa35e10fd01d6adc2f14e7a375f92aef5764` |
| [stage9-frame-I-close.png](stage9-frame-I-close.png) | `1fe4b7c5b93e6c22f73aef0eb65e6ba7939301219d6b3731ea7e95a3cd345bcc` |
| [stage9-frame-II-close.png](stage9-frame-II-close.png) | `e27196601d719d4d951467413f01975aaf44fc81a99b0a9a57dc1911588e0791` |
| [stage9-frame-III-close.png](stage9-frame-III-close.png) | `5dd91bd0ab8c1810a21677c21686670027239b6ea93bb3c397c5b60eb437e746` |
| [stage9-frame-IV-close.png](stage9-frame-IV-close.png) | `3bc4156d3ac8eb6052941a1b8086137d35ddd1aa7c05351e4e3463253ecc5d01` |
| [stage9-chalk-frames-wide.png](stage9-chalk-frames-wide.png) | `ecc43b6f7b1f1739af3a4d2a29de623d23daab838bab55cb1f67a4fbdda8e84c` |
| [stage9-chalk-inspect.png](stage9-chalk-inspect.png) | `e0a75858ca0a65072d1d6e4f931f40f0af3a2b762a1b5bc0f7e68597b219333a` |

## Preservation and remaining limits
- Historical Classroom fixture, shared art/fonts/settings and old validation captures have no tracked content changes. Only Warehouse art, builders, related contracts/capture code, generated scene/materials and stage9 documentation/evidence are committed.
- Existing untracked XML/TRX files were neither deleted nor staged. Extra test-generated stage9 captures remain untracked; the required six plus inspection capture are the selected evidence.
- No known outstanding issue with this scoped replacement. Physical HMD and player build were not tested. Headless runs logged OpenXR Display initialization failures (no active HMD) and a licensing access-token refresh warning, but generation and every test run completed successfully.
