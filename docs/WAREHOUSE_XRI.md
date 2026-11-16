# Proposal stages 1-2: warehouse and shared XRI interaction

## Scope and scene
Proposal source: presentation/proposal/generate-proposal.mjs and preview slides 04/06/07/13/14. Core answers, IDs and P1 -> P2 -> P3 remain unchanged. No clearing/pushing (stage 3), new darkening (stage 4), or S11 player build.

Warehouse.unity is the startup scene: 9x7m, ceiling 3.06m. Exit centred north, clear 2.25m-wide doorway. Seal device beside the exit on the right, cauldron east, altar slightly west of centre. Some west-wall oil paintings lean behind stacked crates without being fully inaccessible. Existing brass glyphs, I-IV chalkboard diagram, candle palette and parchment UI retained.

Classroom.unity remains a separate historical regression fixture, not the startup scene. Its tests retain their original assertions. Old 12x10m/PC-only documentation is superseded by this document for the new scene; puzzle rules are not superseded.

| P2 ID | Position (m) | Warehouse location |
|---|---|---|
| ing_moonpetal | (3.75, .78, -2.94) | southeast window planter |
| ing_salamander_scale | (3.9, 1.03, 1.05) | east storage shelf glass bottle |
| ing_crystal_dust | (-3.25, .36, -2.3) | inside open southwest chest |
| ing_toadstool | (-3.85, .13, 2.7) | northwest floor corner |

## Interaction architecture
PlayerInputReader -> DesktopXriDriver / VrXriDriver -> XRInteractionManager -> shared Interactable / Socket -> existing MonoBehaviour puzzle adapter -> pure C# Core.

- PC: WASD/mouse look, left click grab/press/insert, right click drop. Camera XRRayInteractor is 2.5m. XRI moves selected objects to its camera-front attach point. Click a socket to transfer the selection to it. E aliases the same XRI click path in Warehouse; slots remain for inventory/inspection. Classroom alone retains its legacy input regression path.
- VR: XR Origin Floor tracking, two tracked controllers, grip/trigger selection, left primary button teleport (release to move), right stick 45-degree snap turn. Head-relative WorldSpace Canvas and TrackedDeviceGraphicRaycaster.
- RigSelector initializes OpenXR then checks XRSettings.isDeviceActive; no HMD selects desktop. Inspector RigMode and development F9 allow manual switching. Forced VR without an active HMD enables the official XR Device Simulator sample. Tracking, package UI and simulator internals use their own package drivers; gameplay intent is sampled only by PlayerInputReader and supplied to XRI manual readers.
- Items have one XRGrabInteractable; runes/inspection devices use XRSimpleInteractable. Cauldron and crystal pedestals share XriPuzzleSocket, a XRSocketInteractor subclass.
- Inventory remains the existing puzzle transaction ledger: grabbing registers/selects an ID, sockets invoke the original puzzle adapters. P2 still grants its original inventory rewards and additionally exposes physical ruby/potion pickups. Wrong ingredients respawn at their original warehouse positions. A shared press transaction also retires consumed physical potions when used by a free VR hand or selected inventory slot, preventing duplicate pickups. Socket consumption is deferred one frame after the XRI select event to avoid re-entrant manager mutation.
- HandlingProfile.weight exposes Lift/PushPull/Fixed data only. No clearing behaviour. Warehouse sets RoomPresentation.progressiveDarkening=false; seal lamps, existing exit and ending are retained.

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

Run only one Unity process. Preserve existing untracked XML in docs/validation. XRI integration is validated with real Unity, not the old XR-unaware RuntimeCompileCheck stubs. Detailed test counts and unverified hardware limitations: validation/warehouse/RESULTS.md.

AGENTS.md and README.md are synchronized with this warehouse/XRI specification. Historical classroom-only locations and PC-only restrictions do not apply to Warehouse.
