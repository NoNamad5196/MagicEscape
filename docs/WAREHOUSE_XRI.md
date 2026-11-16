# Proposal stages 1-3: warehouse and shared XRI interaction

## Scope and scene
Proposal source: presentation/proposal/generate-proposal.mjs and preview slides 04/06/07/13/14. Core answers, IDs and P1 -> P2 -> P3 remain unchanged. Stage 3 clearing is implemented. Stage 4 darkening and S11 player build remain out of scope.

Warehouse.unity is the startup scene: 9x7m, ceiling 3.06m. Exit centred north, clear 2.25m-wide doorway. Seal device beside the exit on the right, cauldron east, altar slightly west of centre. West-wall paintings I and IV lean behind handled sliding crates and cannot be inspected until those crates move. Existing brass glyphs, I-IV chalkboard diagram, candle palette and parchment UI retained.

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
- HandlingProfile.weight: Lift (books/small pickups), PushPull (handled crates), Fixed (hinged lid/cloth). XriClearable is one shared XRSimpleInteractable for both rigs, not a desktop-specific copy. Warehouse sets RoomPresentation.progressiveDarkening=false; seal lamps, existing exit and ending are retained.

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

## Stage 3 clearing and P1 visual dictionary
- Paintings I / IV: two handled crates in independent west-wall rails. Centres x=-3.83, y=.51; I z=-.65..+.35, IV z=1.35..2.35. Their .85 x 1.02 x .86 m bodies cannot reach walls, the chest, stacked loads, altar, cauldron, sockets or exit. Heavy props never lift or acquire physics velocity. Scuffs and handles identify only movable crates; decorative storage remains fixed. ResetProp restores authoring pose for recovery tools/tests; normal play needs no reset because rails cannot jam.
- VR uses the same XRI select event and tracked attach-point displacement to pull; 9cm tracked palm contact can also push. Tracking jumps >30cm/frame are ignored for contact. Real HMD ergonomics remain unverified.
- Southwest chest: fixed lid pivots on its back edge, opening 165 degrees; crystal dust stays at its original ID/position and is gated until fully open.
- Liftable books on SouthStorage_0 cover a small loose manuscript. Grab uses the same XRGrabInteractable; release parks on adjacent SouthStorage_1 (supported top), with no throw/gravity. This dressing does not occupy inventory.
- Element table: west wall (-4.35,1.6,.65), close to altar, supported by a wooden hanging dowel and initially covered with gathered cloth. Click/E after clearing enlarges its picture. Original sepia/hatched manuscript shows six symbol/referent pairs, with no text, numbering, arrows or solution order. A duplicate scrap sits under the books.
- ClearingGate disables target colliders and XRI components while covered; InspectableObject, ItemPickup, XriPress and XriItem also reject direct adapter calls. Clearing never modifies puzzle state. Ingredients respawn under the same open lid after errors.
- All corner motifs use FGlyph, exactly the same brass material/vector as altar stones, enlarged from .31 to .40 local width. Glyph vector strokes 7 -> 11px clarify Earth/Wind crossbars. III uses cracked ground, drooping leaves and heat shimmer, not IV's flower. RuneId, item IDs and Earth -> Water -> Fire -> Light remain unchanged.
- Original procedural art source: Tools/Generate-Clearing-Art.py (no third-party imagery).
