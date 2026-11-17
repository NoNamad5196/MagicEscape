# Stage 11 - Warehouse visual and typography corrections

## Changes

1. **No missing operation-prompt glyphs.** Added a Warehouse-only static Gowun Batang TMP asset at `UnityProject/Assets/Art/Warehouse/GowunBatang-Warehouse-SDF.asset`. The old shared atlas was restored after historical construction, discarding recently baked prompt glyphs; Warehouse now owns its own atlas, independent of that preservation scope. Generation collects all Korean characters in runtime/editor source, all authored TMP text, all GameText constants (including punctuation/circled digits), and ASCII. It rejects any missing character before saving. Every Warehouse TMP component uses the resulting font/material. The final atlas contains 529 characters; all 428 unique Korean source characters are present, including the click syllable, with zero missing. `WarehouseStage11Tests.EveryAuthoredAndDynamicKoreanUiCharacterIsBakedWithoutFallback` independently rereads sources/constants and checks all scene fonts without runtime additions or fallback. All lift/open/push/cloth prompts are covered, not just the originally missing click syllable.
2. **Round lid and reachable barrel interior.** Replaced the old solid cylinder with a hollow 32-stave barrel, open segmented iron hoops and a raised inner floor. The .70m circular lid matches the rim, leaves a .01m leak gap and opens 110 degrees about an east-edge hinge. The mushroom remains at its original `(3.50,1.10,-1.25)` home, below the 1.18m rim and visible inside when opened. ClearingGate, ResetProp, shared PC/VR XRI and prompts remain intact. The neutral leak/reveal lights are retained.
3. **Four actual books.** Replaced timber slabs with differently sized, offset, slightly rotated brown/russet/dark-green books, each with two covers, a spine, raised spine bands, cream pages and fine page edges. Scale-only highlight intensity is reduced so the different bindings survive hover lighting; other props keep their former highlight defaults. Books park flat on the same workbench at `(3.96,1.335,.04)`. Fixed dressing jars move a few centimetres toward the outer workbench edge to avoid intersections; bottle/item homes and puzzle geometry do not move. Before/after evidence uses a diagonal standing view to separate the bottle and parked stack visually.
4. **Completed image correction, not skipped.** Built-in image editing changed Story_1, Story_3, Chalk_1 and Chalk_3: the eye-like cavity is replaced by continuous earth around a half-buried seed; III retains its tiny sprout. Reviewed all four generated outputs. Same full-bleed composition, oil/chalk media, mound, right brazier and elemental effects; no procedural substitute. Story_2/4 and Chalk_2/4 remain byte-identical.
5. **Completed chalk layout correction, not skipped.** Increased the four composite pictures from 360x240 to 360x510 in the existing 1536x768 surface. Reduced blank lower area, with I-IV still above the individual pictures. World captions move to local y=.34. E inspection shares the same composite and now also has four separate authored Roman labels, not generated/baked writing. The optional ordered-picture setting is enabled only on the Warehouse chalkboard; historical picture overlays keep their existing behavior.

## Files and contracts

- Reproducible generation: `UnityProject/Assets/Editor/WarehouseBuilder.cs`, `WarehouseStage7Builder.cs`, `WarehouseStage10Builder.cs`, new `WarehouseTypography.cs`; `Tools/Generate-Warehouse-Art.py`.
- Runtime: `UIManager.cs`, `InspectableObject.cs`, `ClearingFeedback.cs`, mirrored under `CodeDrafts/Runtime/`.
- New tests: `Assets/Tests/EditMode/WarehouseStage11Tests.cs`, `Assets/Tests/PlayMode/WarehouseStage11VisualTests.cs`. Existing Warehouse contract checks update only the reviewed composite hash and Roman-caption height. Historical Classroom tests are not edited.
- Generated scene: `UnityProject/Assets/Scenes/Warehouse.unity`; generated Warehouse-only font/materials and corresponding Unity-created meta files; `scene-manifest.txt`.
- `Tools/Run-Unity.ps1` preserves all historical tracked captures, including stage10, while allowing stage11 captures. No serialized Unity YAML was hand-edited. Unity-generated YAML/meta files retain Unity's standard empty-field whitespace and are excluded from the source whitespace check.
- Answer Earth -> Water -> Fire -> Light, RuneId/item IDs, Korean plaques, rune motifs, frame positions, moonflower, darkening stages, combined dust gate, reset behavior and shared XRI architecture remain unchanged.

## Verification

| Final acceptance | Result | Evidence |
|---|---|---|
| Warehouse batch generation | Exit 0 | local `stage11-build.log` |
| Full EditMode | 16 passed / 0 failed / 0 skipped | `stage11-editmode.xml` |
| Full PlayMode, including Classroom and simulated VR regression | 42 passed / 0 failed / 0 skipped | `stage11-playmode.xml` |
| Focused standing physical-ingredient pickup / wrong-attempt respawn | 1 passed / 0 failed / 0 skipped | `stage11-target-playmode.xml` |
| Pure C# Core | 44 passed / 0 failed / 0 skipped | `stage11-core.trx` |
| Hand-authored source whitespace check and three runtime mirror comparisons | Passed | `git diff --check`; byte comparisons |
| Visual review | All 15 final captures reviewed | screenshots below |

Raw nested Unity logs remain local-only per repository policy; final test XML/TRX and screenshots are committed as evidence.

```powershell
git pull --ff-only
python Tools/Generate-Warehouse-Art.py
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage11-build -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage11-editmode -Tests -TestPlatform EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage11-playmode -Tests -TestPlatform PlayMode
dotnet test Tests/MagicEscape.Core.Tests --logger 'trx;LogFileName=stage11-core.trx' --results-directory docs/validation/warehouse
```

Initial compile validation caught an EditMode assembly attempting to reference editor-builder code; the regression test now independently reads the corpus with no cross-assembly reference. Visual iteration reduced book highlight washout and added inspection ordering labels. Final checks additionally cover GameText punctuation, not only Hangul. A full-suite iteration exposed the standing ray grazing the new barrel rim; the final rim is lowered to 1.18m (lid/hinge lowered with it), preserving the original mushroom home and a clear standing pickup ray. The focused physical-ingredient respawn test is rerun before full acceptance.

## Screenshots

All paths below are relative to `docs/validation/warehouse/`; all are actual game-camera/HUD 1280x720 batchmode renders, not SceneView or mockups.

| Requirement | Evidence |
|---|---|
| Lift control prompt | `stage11-operation-prompt.png`, `stage11-ScaleBookStack-before.png` |
| Open control prompt / closed barrel | `stage11-ToadstoolBarrelLid-before.png` |
| Push control prompt | `stage11-operation-push.png` |
| Open barrel, mushroom still inside | `stage11-ToadstoolBarrelLid-after.png` |
| Books before / supported flat parking after | `stage11-ScaleBookStack-before.png`, `stage11-ScaleBookStack-after.png` |
| Board near / E inspection | `stage11-chalk-close.png`, `stage11-chalk-inspect.png` |
| Frames I / III near | `stage11-frame-I-close.png`, `stage11-frame-III-close.png` |
| Additional context / regression | `stage11-frame-II-close.png`, `stage11-frame-IV-close.png`, `stage11-chalk-frames-wide.png`, `stage11-DustPackingRoll-before.png`, `stage11-DustPackingRoll-after.png` |

## Image edit prompt set

Tool: built-in image generation, edit mode, one call per target; no API-key/CLI fallback. Final outputs copied into the existing `Assets/Art/Warehouse/Textures/Story_1.png`, `Story_3.png`, `Chalk_1.png`, `Chalk_3.png` paths.

Shared precise-object-edit prompt: change only the tiny eye-like hole/oval seed in the foreground mound; fill the cavity with continuous coarse earth, showing a small natural seed half buried, lower half occluded by soil grains or chalk soil strokes. No cavity, eye outline, pupil or bright oval ring. Preserve the square full-bleed composition, mound, horizon and right-hand brazier; no text, numbers or new symbols. Story variants preserve dark umber/ochre oil brushwork; chalk variants preserve rough white chalk on black. I keeps the brazier unlit; III preserves the enormous flames and small two-leaf sprout. No sun or flowers.

`Generate-Warehouse-Art.py` only composites generated pictures; it does not draw replacement story/chalk content.

## Remaining limits

- Actual HMD/headset reach and subjective audio balance are not hardware-validated. Desktop/shared-manager and simulated-controller tests are software validation only.
- Player build remains outside this task's scope.
- Existing license-refresh/no-HMD OpenXR messages are retained in local logs; they do not imply missing tests or successful hardware initialization.
- Historical Classroom scene, shared font/prefab/data assets and historical tracked validation images are preserved. Pre-existing untracked XML/TRX evidence is neither changed, removed nor staged.

## Final asset and capture SHA-256

| File | SHA-256 |
|---|---|
| `Story_1.png` | `5448c28e2abd18e38be7bff49217007778a8746cad9329c99c05d463a21f12ab` |
| `Story_3.png` | `528e746cb3f6191056a9258490eb121c59657c98d1d59d4cff217d0e4e072ab9` |
| `Chalk_1.png` | `67d9d79834462fbdee9eec581a07a4b78089e0da634b6e18f5a4252e19503952` |
| `Chalk_3.png` | `749701482e23bdf044c615b6d7ec3be55ee5a3c20515c6adf2af75377b4f2335` |
| `ChalkScenes.png` | `9a847b4ab61dc93b7b4230722fed3185fd2fff0205279163ea7283268236436e` |
| `stage11-chalk-close.png` | `36096718bb2959837acf04a966a1664b504e944a8ed899349b2a21c4e200137b` |
| `stage11-chalk-frames-wide.png` | `218d6dd92b50f5ef994b949a8352410b444dbaa065ecf9c40b7e28ad1e972982` |
| `stage11-chalk-inspect.png` | `676883ef1c34d9ef01b040d8817330608e2cb8ef8d8ce43c5ef03e57dc075ca9` |
| `stage11-DustPackingRoll-after.png` | `4b57988730ec9d6dfb11b616a00e3943aff19c4cfca6fb5a6a342654aa8755fa` |
| `stage11-DustPackingRoll-before.png` | `3ddd47f4ff30dab797cbd49426f57a23324d349f45e5d28b491760c2df64e63e` |
| `stage11-frame-I-close.png` | `bbf267afdbf4d8079034be35180c4bdce4b42edd459d87150aef0642800d3021` |
| `stage11-frame-II-close.png` | `3b02bfacfc348cefaaffdf07af04990bf7f32b4d1de55e82b9dd871da0be75c6` |
| `stage11-frame-III-close.png` | `139e621a93590e54936481f13376f726092b0c1c21dc3e90741fb17785e625bc` |
| `stage11-frame-IV-close.png` | `d858938bf1663555d9a3e23178f891ef2556c757302cf614a195545ed6720472` |
| `stage11-operation-prompt.png` | `bb62ade8e38780dd186f7877b775f5466711cfb3f7c5f6ee50f70a8724f331a0` |
| `stage11-operation-push.png` | `fc88cf82b30b111bd0ccfa5b13beeb1b2352f7155e0048644cede84206ad204f` |
| `stage11-ScaleBookStack-after.png` | `12bc634901679632a9e5481f9dc6b490ef2705b388c19511b4ec6ce93be2e83d` |
| `stage11-ScaleBookStack-before.png` | `9cf3dfac286b27c809a95e483ca179fd52cf58b282a53ae60aa61a5157f335a7` |
| `stage11-ToadstoolBarrelLid-after.png` | `6a0345cc52294e5a9eb76f7254c60d371af288f7ad7c2aca6f528dda0880223a` |
| `stage11-ToadstoolBarrelLid-before.png` | `16add55cf8a8d7b6fb912c98699a643432a153bd89d03a6ab3458e79e1e04efa` |
