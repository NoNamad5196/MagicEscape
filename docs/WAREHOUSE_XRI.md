# Proposal stages 1-4: warehouse and shared XRI interaction

## Scope and scene
Proposal source: presentation/proposal/generate-proposal.mjs and preview slides 04/06/07/13/14. Core answers, IDs and P1 -> P2 -> P3 remain unchanged. Stages 3 clearing and 4 progressive darkening are implemented. S11 player build remains out of scope.

Warehouse.unity is the startup scene: 9x7m, ceiling 3.06m. Exit centred north, clear 2.25m-wide doorway. Seal device beside the exit on the right, cauldron east, altar slightly west of centre. West-wall paintings I and IV lean behind handled sliding crates; III is under a linen cover. Each must be cleared before inspection. The chalkboard orders four story scenes; unnumbered frame plaques map scenes to intuitive brass pictograms and enlarged Korean names. Candle palette and parchment UI remain.

Classroom.unity remains a separate historical regression fixture, not the startup scene. Its tests retain their original assertions. Old 12x10m/PC-only documentation is superseded by this document for the new scene; puzzle rules are not superseded.

## Stage 7 item homes (world metres)

| Item / ID | Position (x, y, z) | Height | Reason / support |
|---|---|---|---|
| Moonflower / `ing_moonpetal` | (3.75, .78, -2.94) | .78m | Existing southeast window planter, explicitly unchanged; sole exception to .8–1.6m rule. |
| Scale / `ing_salamander_scale` | (3.65, 1.30, .05) | 1.30m | Glass bottle behind a liftable book stack on the east workbench; neck and dim light peek above the books. |
| Dust / `ing_crystal_dust` | (-3.25, .96, -2.3) | .96m | Southwest chest raised on a .60m timber plinth. Open its existing lid, then lift the packing-linen bundle off the jar; both gate conditions must clear. |
| Toadstool / `ing_toadstool` | (3.50, 1.10, -1.25) | 1.10m | Damp east barrel beneath a hinged lid; cap and light show through the aisle-facing gap. |
| Sapphire / `crystal_sapphire` | (-.80, 1.12, -.30) | 1.12m | P1 reward directly above the altar centre after its lid opens. |
| Ruby / `crystal_ruby` | (2.50, 1.05, 1.65) | 1.05m | Cauldron-side reward stand, immediately beside the solved P2 work area. |
| Revelation potion / `potion_revelation` | (2.90, 1.05, 1.65) | 1.05m | Same cauldron-side reward stand, separated from ruby so both are visible. |

P1 sapphire is raised to the altar; P2 ruby/potion deliberately keep their already-correct adjacent cauldron stand rather than moving into the main aisle. Crystal reward opacity is .65 (formerly .28) so gems have visible bodies rather than faint edges. Warehouse recipe art is now a separate narrative drawing without the old baked Fire triangle.

Each home has a neutral point light .37m away, range 1.25m, base intensity .55, with a small .25 pulse every 4.8s. No outline or element-colour coding. These lights are excluded from progressive attenuation. Covered dust lights only after opening; hidden rewards light on appearance with a .65s flash and pickup sound. Ingredient `Awake` records these authored homes, so wrong-attempt respawn follows the new positions.

## Interaction architecture
PlayerInputReader -> DesktopXriDriver / VrXriDriver -> XRInteractionManager -> shared Interactable / Socket -> existing MonoBehaviour puzzle adapter -> pure C# Core.

- PC: WASD/mouse look, left click grab/press/insert, right click drop. Camera XRRayInteractor is 2.5m. XRI moves Lift objects to its camera-front attach point. Hold left click and move/strafe to drag heavy crates along their floor rails; release stops them. Hinge/cloth click or E animates opening. Click a socket to transfer the selection to it. E aliases the same XRI click path in Warehouse; slots remain for inventory/inspection. Classroom alone retains its legacy input regression path.
- VR: XR Origin Floor tracking, two tracked controllers, grip/trigger selection, left primary button teleport (release to move), right stick 45-degree snap turn. Head-relative WorldSpace Canvas and TrackedDeviceGraphicRaycaster.
- RigSelector initializes OpenXR then checks XRSettings.isDeviceActive; no HMD selects desktop. Inspector RigMode and development F9 allow manual switching. Forced VR without an active HMD enables the official XR Device Simulator sample. Tracking, package UI and simulator internals use their own package drivers; gameplay intent is sampled only by PlayerInputReader and supplied to XRI manual readers.
- Items have one XRGrabInteractable; runes/inspection devices use XRSimpleInteractable. Cauldron and crystal pedestals share XriPuzzleSocket, a XRSocketInteractor subclass.
- Inventory remains the existing puzzle transaction ledger: grabbing registers/selects an ID, sockets invoke the original puzzle adapters. P2 still grants its original inventory rewards and additionally exposes physical ruby/potion pickups. Wrong ingredients respawn at their original warehouse positions. A shared press transaction also retires consumed physical potions when used by a free VR hand or selected inventory slot, preventing duplicate pickups. Socket consumption is deferred one frame after the XRI select event to avoid re-entrant manager mutation.
- HandlingProfile.weight: Lift (books/small pickups), PushPull (handled crates), Fixed (hinged lid/cloth). XriClearable is one shared XRSimpleInteractable for both rigs, not a desktop-specific copy. Warehouse sets RoomPresentation.progressiveDarkening=true; WarehouseDarkening owns its lighting, while the historical Classroom presentation remains unchanged. Exit corridor and ending stay dark.

## Package evidence - checked 2026-11-16
- [Unity 6.3 XRI package listing](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.xr.interaction.toolkit.html): XRI 3.3.2.
- [Unity 6.3 OpenXR package listing](https://docs.unity3d.com/6000.3/Documentation/Manual/com.unity.xr.openxr.html): OpenXR 1.16.1.
- [XR Device Simulator](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.3/manual/xr-device-simulator-overview.html): official XRI sample, not a separate UPM package. Only that sample is imported.
- Local installed package.json minimum Unity versions/dependencies inspected; actual Package Manager resolution and compilation on Unity 6000.3.28f1 used as verification. packages-lock.json committed.

## Batchmode entry points
1. MagicEscape.EditorTools.ClassroomBuilder.ImportXriSamples (once after package resolve).
2. MagicEscape.EditorTools.ClassroomBuilder.Warehouse (full reproducible scene generation; Unity creates serialized assets/meta).
3. Unity -batchmode -runTests -testPlatform PlayMode, results under docs/validation/warehouse.
4. Unity -batchmode -runTests -testPlatform EditMode and dotnet test Tests/MagicEscape.Core.Tests.

Run only one Unity process. Preserve existing untracked XML in docs/validation. XRI integration is validated with real Unity, not the old XR-unaware RuntimeCompileCheck stubs. Stage-3 test counts and unverified hardware limitations: validation/warehouse/STAGE3_RESULTS.md (stages 1-2 history: RESULTS.md).

AGENTS.md and README.md are synchronized with this warehouse/XRI specification. Historical classroom-only locations and PC-only restrictions do not apply to Warehouse.

## P1 scene-to-frame clues (stage 7 supersedes stages 5–6)
- World chalkboard and E enlargement share `Assets/Art/Warehouse/ChalkScenes.png`: I soil mound with a tiny buried seed, II heavy rain on that same mound, III large flames in the adjacent brazier, IV descending light shafts and a small sprout. Camera, mound, brazier and horizon remain fixed. Only scene drawings and I–IV occur; no element names or standalone element pictograms. Large clean chalk strokes on the same raised board (centre y=1.87m). `Tools/Generate-Warehouse-Art.py` reproduces pictogram masks and composites the generated chalk pictures; it never draws P1 scenes.
- No Roman numerals remain visible on any frame. Each lower plaque contains only a matching pictogram and enlarged Gowun Batang name. Internal `Painting_I` etc. identifiers are authoring keys, not visible labels. Frame order is deliberately shuffled.
- Solve path: numbered board scene → matching frame scene → plaque element name/pictogram → matching altar stone. Answer remains Earth → Water → Fire → Light; RuneId, item IDs and P1 → P2 → P3 unchanged.
- Pictograms are a mountain ridge (earth), single water drop, flame, twelve-ray sun (light), three curled wind strokes, crescent (shadow). One monochrome 11px stroke system, shared brass incision material and no toy/emoji colour differentiation. III depicts actual brazier fire (no sun), with flame / 불 plaque; IV depicts light shafts (no flower), with sun / 빛 plaque.
- Six stone slabs grow to .41×.49m; name surfaces .38×.15m (previously .33×.11m), with unchanged Gowun Batang source masks and shared press fill/glow feedback. Frame names .48×.16 local, pictograms .19m local. Hit colliders and answer logic are unchanged.
- Warehouse substitutes all six historical alchemical mask materials, including inscription and seal lamps, with Warehouse-only pictograms. Classroom keeps its historical resources and scene byte-for-byte. Previous `ChalkBlanks` asset/material and generator are retired.
- I/IV sliding crates and III cloth clearing remain. The raised southwest chest retains the same fully-open gate and 100-degree open hinge (steep enough not to screen frame III). All shared PC/VR clearing and direct-adapter safeguards remain.
- Warehouse Tab still respects P1 3 / P2 2 / P3 2 failure thresholds. Warehouse P1 hint now refers to numbered board scenes, not nonexistent frame numbers. Classroom retains its old hint.
- `WarehouseStage7Builder` applies Warehouse-specific visuals and placement after shared historical construction; Warehouse generation preserves historical assets and tests preserve old validation captures.

## Stage 4 progressive darkness
- One shared PC/VR controller, WarehouseDarkening; RoomPresentation routes Warehouse progress without changing Core, item IDs or clearing gates.
- Tune Assets/Settings/WarehouseLighting.asset (WarehouseLightingProfile): 1.6s smooth blend; ambient RGB = (.70, .80, 1) times [.15, .105, .065, .035]; reflection [.65, .40, .22, .10]; directional fill [.22, .15, .085, .035]; background multipliers [1, .62, .30, .10]; candle multipliers [1, .78, .53, .28]; bounded low-frequency flicker amplitude [.025, .045, .065, .085]. Existing fog is unchanged.
- Six floor candles and two wall sconces shrink their flames before extinguishing, with a four-particle thin smoke wisp. West/altar candles go first, east/brew next, then north/exit; one dim navigation wick remains. Legacy AtmosphereMotion light writers are disabled only for Warehouse.
- Six base cold local lights (plus seven stage-7 item beacons) (intensity .65, range 2.1m) preserve cauldron liquid, three real ingredient locations (including the gated chest), inscription/sockets and exit. They are not clues or answer-order signals. Stage 7 reuses the existing pickup sound to announce physical reward appearance.
- Warehouse does not enable dawn, window replacement or rays after P3. Its transition never calls SetBlocked. Existing door timing, black fade and black ending are retained.
- ShowPicture caps parchment height at 500 reference pixels and puts CloseHelp below its bounds; applies to frames, recipes and all other picture overlays.
- Warehouse generation snapshots/restores historical Classroom, shared assets, fonts and S10 validation artifacts in a finally/Dispose scope. Run-Unity.ps1 similarly preserves tracked regression captures/audits around tests; new stage7 captures are retained. Existing local untracked evidence is not staged or deleted.

- Completed brew emission falls to 6% over the same blend (profile solvedBrewGlow), preserving its short gold success feedback without keeping the solved cauldron as the brightest target. Inactive answer-frame lamps are included in background attenuation when later enabled. Both rig cameras clear to black; Warehouse fade and ending rects stretch to the canvas, preventing skybox leaks and uncovered borders.

## Stage 8 visual cleanup (2026-11-17)
- Board I now shows a pointed almond seed with a centre seam and one tiny upward shoot; no two-legged roots. World board and E still share `ChalkScenes.png`, with the same chalk tone/strokes as II-IV. Warehouse frame I uses its own matching `Story_1.png` in the scene and inspection; historical Classroom art stays unchanged.
- Water's inner arc is replaced by one short upper-left diagonal glint. The same material/mask is shared by altar stones and frame plaques. The other five motifs retain their non-facial silhouettes.
- Scale home remains `(3.65,1.30,.05)`. Its 1.20m-high support is now a thin plank worktop, four floor-supported legs and aprons, with visible plank seams. Two small muted fixed jars and a low mortar/pestle provide ingredient-workbench context; no new pickups, inspection clues or puzzle components. All other homes, clearing gates, darkness and puzzle contracts are unchanged.
- `Tools/Run-Unity.ps1` preserves all historical captures (including stage7); current visual evidence uses `stage8-*`. Results and capture hashes: [Stage 8](validation/warehouse/STAGE8_RESULTS.md).

## Stage 9 image-generated P1 pictures
- Eight reviewed built-in image-generation outputs live in `Assets/Art/Warehouse/Textures/`: `Chalk_1.png` through `Chalk_4.png` and `Story_1.png` through `Story_4.png`, all imported at 1024 square. No writing, numbers or alchemical glyphs occur inside those pictures.
- Stage 10 supersedes the original stage-9 subjects: soil mound, heavy rain, burning brazier, light shafts; the small seed/sprout stays subordinate. The oil paintings fill their frames and match their chalk protagonists/compositions.
- `Generate-Warehouse-Art.py` only composites the four generated chalk pictures into `ChalkScenes.png`; it no longer draws P1 scenes or frame I procedurally. Roman ordering is separate authored world text above the pictures, not painted into them. Inspection uses the same composite in left-to-right order.
- Warehouse builder assigns all four generated oil textures to both world illustrations and inspection. Brass names/pictograms, rune stones, frame positions, lighting and III cloth remain unchanged. Historical Classroom assets are not replaced.
- Current evidence: [Stage 9](validation/warehouse/STAGE9_RESULTS.md).

## Stage 10 - direct element scenes and tactile clearing

Eight new built-in image-generation illustrations replace Chalk_1-4 and Story_1-4 in `UnityProject/Assets/Art/Warehouse/Textures/`. Chalk is bold white on black; oil paintings are dark candlelit umber/ochre. All eight use the same fixed scene/camera: soil mound in the foreground and brazier on the right. I earth, II heavy rain, III large fire tongues, IV broad descending light beams. No writing, numbering, standalone glyphs, sun disc, moon, wind swirl or flowers inside the pictures. Small buried seed/sprout is secondary. Roman board text, brass Korean plaques, rune pictograms, RuneId, answer and frame positions are unchanged. Failure-only Tab hint already refers to board/frame/plaque matching and needed no text change.

### Clearing inventory (8 props, 6 gated targets)
| Prop | Shared handling / PC prompt | Revealed target | Home / supported parking (metres) |
|---|---|---|---|
| ClearingCrate_I / IV | PushPull / hold left click + WASD | Frames I / IV | Existing west-wall floor rails, unchanged |
| PaintingIIICloth | Fixed / left click to uncover | Frame III | Existing wall frame III, unchanged |
| ChestLidHinge | Fixed / left click to open | Crystal dust | Southwest raised chest (-3.25, .96, -2.3), unchanged |
| MovableBookStack | Lift / left click to lift | Dressing board (not a clue) | Existing south storage crates, unchanged |
| ScaleBookStack (new) | Lift / left click to lift | Salamander scale bottle | (3.40, 1.335, .04) -> (3.95, 1.335, .04), both on the 1.20m east workbench |
| ToadstoolBarrelLid (new) | Fixed / left click to open | Toadstool | Hinge (3.86, 1.24, -1.25), 105-degree outward opening on east barrel |
| DustPackingRoll (new) | Lift / left click to lift | Crystal dust, after opening existing chest lid | (-3.25, 1.10, -2.3) -> (-2.62, 1.04, -2.3), on a supported .96m side shelf |

Ingredient pickup homes and moonpetal are unchanged. Bottle-neck area, mushroom-cap gap and dust-jar edge plus low neutral leak lights make covered locations visible without searching. The dust has a combined gate: existing chest lid AND new linen bundle. Covers remain in storage bays, not in front of the exit, altar, cauldron or seal device; no new obstruction is placed in front of an existing frame. Lift props park on authored supported surfaces; ResetProp restores their homes and gates.

`ClearingFeedback` applies a reversible warm material highlight on XRI hover/selection or within 1.3m (PC and VR). PlayerInteraction uses its IInteractable prompt through the existing HUD; UIManager accepts an explicit control prefix without adding [E]. No puzzle-hint prose. E still aliases the same XRI transaction. All eight moving props use procedural spatial wood-friction/landing clips synthesized inside Unity (no external audio/license dependency), plus short dust emission. `ClearingReveal` lives separately at each target: .12 dim leak -> .85 local light in .65s, short shimmer and existing pickup sound on each covered-to-accessible transition. It neither changes progress nor bypasses ClearingGate, and remains independent of WarehouseDarkening. Collider/XRI/direct-adapter blocking and reset/re-covering are retained.

Evidence and image prompt specification: [STAGE10_RESULTS.md](validation/warehouse/STAGE10_RESULTS.md). Actual HMD and player build remain unverified/out of scope.

## Stage 11 - typography, believable clearing props and larger board pictures

- Warehouse uses its own baked static `Assets/Art/Warehouse/GowunBatang-Warehouse-SDF.asset`; historical Classroom fonts/assets are untouched. The builder collects all Korean syllables/jamo in runtime and editor source plus authored TMP text, bakes them without fallback, and fails generation if any are missing. This includes all four clearing prompts (notably the missing click syllable). EditMode independently checks the source corpus and every Warehouse TMP font without dynamic glyph additions or fallback.
- The damp barrel is now a hollow 32-stave shell with open hoop bands and a raised inner floor. Rim height is 1.18m; the mushroom home remains `(3.50,1.10,-1.25)` inside it. A matching .70m round lid sits with a .01m light-leak gap; its east-edge hinge `(3.90,1.21,-1.25)` opens outward 110 degrees. Existing shared XRI, ClearingGate, ResetProp and prompts are unchanged.
- ScaleBookStack has four slightly offset/rotated books with different dimensions, dark brown/russet/green covers, spines, raised spine bands and cream paper edges. Home remains `(3.40,1.335,.04)`; supported parking is `(3.96,1.335,.04)` without rotating the stack upright or into the scale bottle. It stays on the existing workbench.
- Built-in image editing replaces only the eye-like cavities in Story_1/3 and Chalk_1/3 with soil around half-buried seeds (III retains its small sprout). Composition, medium and elemental effects remain. No procedural replacement art. Other four images remain byte-identical.
- Chalk composite pictures expand from 360x240 to 360x510 in the existing 1536x768 surface, leaving only modest top/bottom margins. I-IV remain separate text above each picture, raised to local y=.34. World and E inspection still share one composite; E also displays four separate authored Roman labels above the pictures. Answers, IDs, plaques, rune motifs, frames, moonflower and progressive darkening are unchanged.
- Evidence, full commands and limits: [STAGE11_RESULTS](validation/warehouse/STAGE11_RESULTS.md).
