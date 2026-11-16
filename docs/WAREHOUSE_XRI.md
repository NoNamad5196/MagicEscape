# Proposal stages 1-4: warehouse and shared XRI interaction

## Scope and scene
Proposal source: presentation/proposal/generate-proposal.mjs and preview slides 04/06/07/13/14. Core answers, IDs and P1 -> P2 -> P3 remain unchanged. Stages 3 clearing and 4 progressive darkening are implemented. S11 player build remains out of scope.

Warehouse.unity is the startup scene: 9x7m, ceiling 3.06m. Exit centred north, clear 2.25m-wide doorway. Seal device beside the exit on the right, cauldron east, altar slightly west of centre. West-wall paintings I and IV lean behind handled sliding crates; III is under a linen cover. Each must be cleared before inspection. Existing brass glyphs, an empty I-IV chalkboard diagram, candle palette and parchment UI retained; matching Korean names now make symbols explicit.

Classroom.unity remains a separate historical regression fixture, not the startup scene. Its tests retain their original assertions. Old 12x10m/PC-only documentation is superseded by this document for the new scene; puzzle rules are not superseded.

| P2 ID | Position (m) | Warehouse location |
|---|---|---|
| ing_moonpetal | (3.75, .78, -2.94) | southeast window planter |
| ing_salamander_scale | (3.9, 1.03, 1.05) | east storage shelf glass bottle |
| ing_crystal_dust | (-3.25, .36, -2.3) | inside southwest chest; open hinged lid first |
| ing_toadstool | (-3.85, .13, 3.08) | northwest floor corner |

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

## P1 named symbols and clearing (stages 5–6)
- Warehouse chalkboard uses only I ○ II ○ III ○ IV ○, a hand-drawn empty-circle diagram. The world plate and E inspection share Assets/Art/Warehouse/ChalkBlanks.png, generated independently by Tools/Generate-Warehouse-Chalk.py. No element glyph, story illustration or element name is attached. Classroom's old answered ChalkSequence is historical only.
- The board assembly and its lamp sit .85m higher on the same west wall (board centre y=1.87m), so fixed storage stacks no longer obscure the first two empty slots. No clearing prop or frame location changes for this adjustment.
- All four frames put numeral, matching brass glyph and Gowun Batang name on one lower brass plaque, below the illustration; no floating name label covers the art. Frame-order mappings must be collected from the four frames only.
- Warehouse Tab no longer bypasses failure thresholds with Chalkboard/Recipe/InscriptionRevealed prose. It displays GetCurrentHint only (P1 3 failures, P2/P3 2). Classroom retains its historical regression behavior. Recipe picture remains P1-gated; inscription remains potion-gated. No loading screen carries P1 order art.
- Paintings I / IV: two handled crates in independent west-wall rails. Centres x=-3.83, y=.51; I z=-.65..+.35, IV z=1.35..2.35. Their .85 x 1.02 x .86 m bodies cannot reach walls, the chest, stacked loads, altar, cauldron, sockets or exit. Heavy props never lift or acquire physics velocity. Scuffs and handles identify only movable crates; decorative storage remains fixed. ResetProp restores authoring pose for recovery tools/tests; normal play needs no reset because rails cannot jam.
- VR uses the same XRI select event and tracked attach-point displacement to pull; 9cm tracked palm contact can also push. Tracking jumps >30cm/frame are ignored for contact. Real HMD ergonomics remain unverified.
- Southwest chest: fixed lid pivots on its back edge, opening 165 degrees; crystal dust stays at its original ID/position and is gated until fully open.
- Liftable books on SouthStorage_0 cover a plain bookbinding board (non-clue dressing, no inspection). Grab uses the same XRGrabInteractable; release parks on adjacent SouthStorage_1 (supported top), with no throw/gravity. This dressing does not occupy inventory.
- No element dictionary, duplicate manuscript, dictionary inspection texture/material or dictionary-specific tests remain. Six altar stones and all four lower frame plaques explicitly carry the matching Korean element name in Gowun Batang Regular: 대지 / 물 / 불 / 바람 / 빛 / 그림자 (Earth / Water / Fire / Wind / Light / Shadow). The answer still comes only from matching symbols/names in frame order I-IV, not interpreting the story illustration.
- Painting III remains at (-4.39, 1.65, -1.75), at the same wall-mounted height as II. A folded linen cover drapes from its top edge. Selecting/clicking/E gathers it upward; ClearingGate blocks III collider, XRI and direct inspection until fully cleared. I/IV still require sliding their crates; crystal dust still requires opening the chest.
- Names are original raster incision masks rendered with the same brass AlchemyEngraving shader as symbols (not a Gothic UI font). Stone name surface .33 x .11 m, 112px Gowun Batang baked into 384 x 128 masks; matching frame labels .36 x .12 local (.306 x .102 m world). Name and symbol share per-instance fill and emission feedback. Slate visuals grow to .36 x .43 m without changing hit colliders, RuneId or positions.
- ClearingGate disables target colliders and XRI components while covered; InspectableObject, ItemPickup, XriPress and XriItem also reject direct adapter calls. Clearing never modifies puzzle state. Ingredients respawn under the same open lid after errors.
- All frame plaque motifs use FGlyph, exactly the same brass material/vector as altar stones, at .18 local width beside the numeral and name. Glyph vector strokes 7 -> 11px clarify Earth/Wind crossbars. III uses cracked ground, drooping leaves and heat shimmer, not IV's flower. Tools/Generate-Clearing-Art.py --warehouse-iii reproduces this art without the old baked Fire glyph; only its lower plaque carries the glyph. The E overlay uses the same unannotated illustration. RuneId, item IDs and Earth -> Water -> Fire -> Light remain unchanged.
- Original procedural art source: Tools/Generate-Clearing-Art.py (no third-party imagery).

## Stage 4 progressive darkness
- One shared PC/VR controller, WarehouseDarkening; RoomPresentation routes Warehouse progress without changing Core, item IDs or clearing gates.
- Tune Assets/Settings/WarehouseLighting.asset (WarehouseLightingProfile): 1.6s smooth blend; ambient RGB = (.70, .80, 1) times [.15, .105, .065, .035]; reflection [.65, .40, .22, .10]; directional fill [.22, .15, .085, .035]; background multipliers [1, .62, .30, .10]; candle multipliers [1, .78, .53, .28]; bounded low-frequency flicker amplitude [.025, .045, .065, .085]. Existing fog is unchanged.
- Six floor candles and two wall sconces shrink their flames before extinguishing, with a four-particle thin smoke wisp. West/altar candles go first, east/brew next, then north/exit; one dim navigation wick remains. Legacy AtmosphereMotion light writers are disabled only for Warehouse.
- Six cold local lights (intensity .65, range 2.1m) preserve cauldron liquid, three real ingredient locations (including the gated chest), inscription/sockets and exit. They are not clues or answer-order signals. No audio clue or new sound is added.
- Warehouse does not enable dawn, window replacement or rays after P3. Its transition never calls SetBlocked. Existing door timing, black fade and black ending are retained.
- ShowPicture caps parchment height at 500 reference pixels and puts CloseHelp below its bounds; applies to frames, recipes and all other picture overlays.
- Warehouse generation snapshots/restores historical Classroom, shared assets, fonts and S10 validation artifacts in a finally/Dispose scope. Run-Unity.ps1 similarly preserves tracked regression captures/audits around tests; new stage6 captures are retained. Existing local untracked evidence is not staged or deleted.

- Completed brew emission falls to 6% over the same blend (profile solvedBrewGlow), preserving its short gold success feedback without keeping the solved cauldron as the brightest target. Inactive answer-frame lamps are included in background attenuation when later enabled. Both rig cameras clear to black; Warehouse fade and ending rects stretch to the canvas, preventing skybox leaks and uncovered borders.
