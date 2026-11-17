# Stage 10 — element scenes and clearing feedback

## Delivered

- Replaced all eight P1 images with built-in image generation, using the first chalk image as the composition reference for the other chalk stages and each corresponding chalk stage as the reference for its oil painting. No procedural substitute for P1 art. Visually reviewed all eight outputs: no writing/numbers/glyphs, sun disc, flower, moon or wind motif. Soil, rain, brazier fire and light beams are the protagonists; seed/sprout stays small.
- Stable paths: `UnityProject/Assets/Art/Warehouse/Textures/Chalk_1.png`–`Chalk_4.png`, `Story_1.png`–`Story_4.png`. Native generated sources are 1254-square; Unity imports story textures at 1024. `ChalkScenes.png` is recomposited from the generated chalk images by the existing Python tool, not drawn by it.
- Answer Earth → Water → Fire → Light, RuneId/item IDs, Korean brass plaques, rune pictograms, board I–IV ordering and all frame positions remain unchanged. Existing failure-only P1 Tab hint already describes board-scene/frame/plaque matching, so it required no text change.
- Added three covers, keeping the existing four puzzle covers and non-inventory book stack: eight movable props total. Six target gates; dust has two blockers. Moonpetal and every ingredient pickup home stay unchanged.

| New object | Operation | Revealed object | Location / parking |
|---|---|---|---|
| ScaleBookStack | Lift, left click; right click to put down | Salamander scale | East workbench (3.40, 1.335, .04), parks at (3.95, 1.335, .04), rotated between existing jars |
| ToadstoolBarrelLid | Fixed hinge, left click / E | Toadstool | East barrel, hinge (3.86, 1.24, -1.25), visible supporting posts and hinge pins, opens 105 degrees |
| DustPackingRoll | Lift, left click; right click to put down | Crystal dust, only after existing chest lid also opens | Southwest chest (-3.25, 1.10, -2.3), parks on supported side shelf (-2.62, 1.04, -2.3) |

Dim local leak lights identify covered locations before interaction. Bottle area, cap gap and jar edge remain visibly suggested rather than turning into a pixel hunt. New covers do not occupy puzzle/exit circulation or obscure another frame after parking.

## Implementation

- `WarehouseStage10Builder.cs`: reproducible Warehouse-only covers, supports, gate links, feedback and target-local reveal lights. Invoked by `ClassroomBuilder.Warehouse` after stage 7 construction.
- `ClearingFeedback.cs`: shared XRI hover/selection or 1.3m proximity highlight, short control-only prompts through existing PlayerInteraction/HUD, procedural spatial wood friction and landing sounds, soft procedural dust particles. No external audio or license dependency. The procedural radial texture is VFX falloff only, never P1 illustration content.
- `ClearingReveal.cs`: each covered-to-accessible edge produces one reused pickup sound and short shimmer, with a smooth .65s local light rise from .12 to .85. No game progression mutation and no WarehouseDarkening attenuation.
- `XriLooseProp.cs`: ResetProp and optional supported parking rotation; shared XRGrabInteractable remains the only carry path. `UIManager.cs` accepts explicit control prefixes without prepending a second [E]. Runtime changes mirrored into CodeDrafts.
- Tests: new three-cover PlayMode coverage asserts collider/XRI/direct-adapter blocking before clearing, actual desktop ray selection of each cover, shared-manager selection of the exposed target, reveal/impact feedback, and blocking again after ResetProp. Combined dust gate is tested with the chest open but packing bundle still closed. Existing Warehouse fixtures now prepare the new covers; Classroom fixtures are not edited. EditMode checks count, support and unchanged moonpetal.
- README, AGENTS and Warehouse specification updated. `Run-Unity.ps1` preserves historical tracked evidence while allowing current `stage10-*` evidence.

## Commands and verification

```powershell
git pull --ff-only
python Tools/Generate-Warehouse-Art.py
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage10-build -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage10-playmode -Tests -TestPlatform PlayMode
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage10-editmode -Tests -TestPlatform EditMode
dotnet test Tests/MagicEscape.Core.Tests --logger 'trx;LogFileName=stage10-core.trx' --results-directory docs/validation/warehouse
```

| Final acceptance | Result | Evidence |
|---|---|---|
| Warehouse batch generation | Exit 0 | `stage10-build.log` |
| Full PlayMode, including Classroom regression and simulated VR | 40 passed / 0 failed / 0 skipped | `stage10-playmode.xml`, `stage10-playmode.log` |
| Full EditMode | 14 passed / 0 failed / 0 skipped | `stage10-editmode.xml`, `stage10-editmode.log` |
| Pure C# Core | 44 passed / 0 failed / 0 skipped | `stage10-core.trx` |
| Focused new-cover PlayMode | 2 passed / 0 failed | `stage10-new-playmode.xml`, `stage10-new-playmode.log` |

Initial iterations caught inventory cleanup in the test, a reveal assertion preceding the smooth-light completion, and an exact-equality test on a serialized moonpetal coordinate; final checks use the full hinge-plus-light duration and 1mm positional tolerance. An exploratory frame-II cover was discarded because its support screened frame IV; the shipped third cover is the packing linen inside the existing chest, with no new frame obstruction. All final acceptance suites pass. Source diff whitespace check passes excluding Unity's generated Warehouse YAML (which retains Unity's standard blank-field whitespace); no YAML was hand-edited.

## Screenshot evidence

All captures use the actual gameplay camera and HUD rendered to 1280×720 in batchmode, not SceneView. The wide overview uses a raised eye; the board close-up narrows FOV for legibility. New-cover before/after views use the same standing location.

- `stage10-chalk-close.png`, `stage10-chalk-inspect.png`
- `stage10-frame-I-close.png`, `stage10-frame-II-close.png`, `stage10-frame-III-close.png`, `stage10-frame-IV-close.png`
- `stage10-chalk-frames-wide.png`
- `stage10-ScaleBookStack-before.png`, `stage10-ScaleBookStack-after.png`
- `stage10-ToadstoolBarrelLid-before.png`, `stage10-ToadstoolBarrelLid-after.png`
- `stage10-DustPackingRoll-before.png`, `stage10-DustPackingRoll-after.png`
- `stage10-operation-prompt.png`

All paths above are relative to `docs/validation/warehouse/`. Every final capture was visually inspected.

## Image generation prompt set

Shared: square illustration asset, fixed camera/composition: large foreground soil mound with a small buried seed, simple horizontal ground/horizon, brazier at right. No words, letters, numerals, standalone symbols, border, watermark, sun, moon, flowers or wind curls. Vegetation always subordinate. Chalk uses bold white hand-drawn strokes on pure black, legible at several metres. Oil uses full-bleed rich dark old-master umber/ochre candlelit brushwork, preserving the corresponding chalk reference layout.

1. Earth: soil mound and its coarse texture dominate; tiny buried seed; brazier unlit.
2. Water: dense thick vertical rain and splashes dominate; same mound now wet; brazier unlit.
3. Fire: enormous unmistakable flame tongues blaze upward from the same right-hand brazier and fill the upper half; no rain or sun; a tiny two-leaf sprout is secondary.
4. Light: broad straight ivory-white/gold shafts descend from above and dominate the upper half; modest two-leaf sprout; brazier unlit; no flowers or solar disc.

## Limits / open points

- No actual HMD or player build validation; existing desktop and simulated-controller Unity tests provide software coverage, not hardware acceptance.
- Audio sources, event execution and parameters were tested; subjective sound balance in a headset still needs a human playthrough. No puzzle information is encoded by the sounds.
- Existing batchmode licensing refresh/OpenXR-no-HMD messages are retained in logs; they do not imply hardware validation.
- Historical Classroom/shared asset regeneration noise is excluded from the commit. Pre-existing untracked validation XML/TRX files are not deleted, edited or staged.

## Image hashes (SHA-256)

| Asset | SHA-256 |
|---|---|
| `Chalk_1.png` | `937f25e70b2d30464f945190b4f39b9694688ec6f36b42c3ed70bd65c1dc0912` |
| `Chalk_2.png` | `e5eca52cbb851e5125612b939852c54d4430bf350bf4308a0356dc1ec6d418f4` |
| `Chalk_3.png` | `47f938fec7fce6ce76756fd95ae0ee6f79835b057ae14f3517b0fc8fdf07cb62` |
| `Chalk_4.png` | `ce17854f2836848cd2b6ae87b8eab721cb3bb76b6bd04c434fdabaae92b28941` |
| `Story_1.png` | `9d38f1760a8c8472e0819da2a3c7bc9203dcc9acfc972afa9dadabae81835406` |
| `Story_2.png` | `f399150b35a0f963ecaccb4f3cc423f59c73a0a60be42d8045166050203672b9` |
| `Story_3.png` | `f0bb407b077a2b4932bfe3ab64a4f5cc3c9ef3d53b9000498fa717445bd8a773` |
| `Story_4.png` | `6ba78974666f9cb69f994abcce63a8f54e1727d7e6251e03dcf4d199642b799b` |

## Final capture hashes (SHA-256)

| Capture | SHA-256 |
|---|---|
| `stage10-chalk-close.png` | `4ed4e9e4a714d9434ebb25e95dc0f73ba5a9ba929b49dfec4e86b41a77539a53` |
| `stage10-chalk-frames-wide.png` | `e9b7bb315b83d86b5900847c07554e55d331b1cf3670bf0545e8053866a85111` |
| `stage10-chalk-inspect.png` | `8568f75c0d0555a2fa6acb0fecd997ac5c8a1b1d5262d4ab59e9e9b48d213713` |
| `stage10-DustPackingRoll-after.png` | `2c024408d3d8e82c90ec261bcf2faced6950aa2da1f7bad04f99c5cef6596a18` |
| `stage10-DustPackingRoll-before.png` | `ec6ab68ec3d7dfde81765f6f58ea797b3fa89e329c3dae47c8b464a551b98548` |
| `stage10-frame-I-close.png` | `6bde67aee155412654dc9e0d94c7c6e8e39413c8952231e9e695a6f454338a63` |
| `stage10-frame-II-close.png` | `290373cfcdff4707cce3c0874d1ebe77172f46ea2ab6c741c8172646336e4880` |
| `stage10-frame-III-close.png` | `d20182f81a2e3237221e2a166b08719f5345640f7a10a0b18a8105ced1f2e04b` |
| `stage10-frame-IV-close.png` | `64299e2d89410c3e68df2b91ae4f6ebcc83698428d0deecbe578ce23d0a6df27` |
| `stage10-operation-prompt.png` | `c64b2cd30bddc4236105063a0eb0a03db16a74635164f36d74decad60e9934b1` |
| `stage10-ScaleBookStack-after.png` | `e34a2185e32a4f52fb1a16e4dfadd84f625c142b11322faafd9d46bd9fcf6dc8` |
| `stage10-ScaleBookStack-before.png` | `cdbf8c3422df38c89885b8987e5fab0746d3f2716eb810fd80c5320fbd1ccec4` |
| `stage10-ToadstoolBarrelLid-after.png` | `ff36610fc3e03f9d436a64f21b183e35e72d7cb50432b9c1460e784a46e69422` |
| `stage10-ToadstoolBarrelLid-before.png` | `88c09c69fc30bfa364eb1f053019d60d796377c6c6d75e8154e1f9ccf3ce220f` |

Preservation check: historical Classroom scene and tracked historical validation captures have no diff. No shared generated asset changes remain.
