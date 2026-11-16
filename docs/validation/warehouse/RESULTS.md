# Proposal stages 1-2: verified results

## Changes
- Warehouse startup scene: 9x7m, ceiling 3.06m; human-height 2.6m exit opening, clear central route, storage shelves/crates/open chest and partly stacked leaning paintings. Numeral plates remain exposed until stage 3 clearing exists.
- WarehouseBuilder Editor entry points generate/repair the scene in batchmode; no serialized Unity YAML authored by hand. Classroom remains an unchanged historical regression fixture.
- Shared XRI objects: Simple Interactable runes/devices, Grab items, cauldron/crystal Socket interactors. Camera-ray desktop carrying and controller VR use the same transaction adapters. PlayerInputReader owns gameplay input sampling; E aliases XRI in Warehouse.
- Automatic HMD selection, manual Inspector RigMode / development F9; XR Origin, two tracked controllers, teleportation/snap-turn providers, official XR Device Simulator sample. Desktop camera explicitly excludes stereo even if an OpenXR runtime remains loaded.
- HandlingProfile exposes Lift/PushPull/Fixed data only. No stage-3 clearing/pushing behaviour. Warehouse progressiveDarkening=false; no new stage-4 darkening. Existing seals/exit/ending retained. No S11/player build.
- CodeDrafts runtime mirror synchronized. Core rules, answers, IDs and original test assertions unchanged.

## Actual verification (Unity 6000.3.28f1)
| Run | Passed | Failed | Evidence |
|---|---:|---:|---|
| Unity EditMode | 4 | 0 | editmode.xml |
| Unity PlayMode (19 historical + 9 warehouse) | 28 | 0 | playmode-accepted-final.xml |
| Pure C# dotnet tests | 44 | 0 | core-summary.json; local raw warehouse-core.trx |

Final PlayMode end UTC: 2026-11-16 12:36:17Z. Unexpected Error/Exception/Assert events: 0 (console-audit.txt). Unity scene generation and header/clue placement application exited successfully. No OutOfMemory crash observed during this task.

New PlayMode coverage: desktop rune presses; grab/carry ingredients into the cauldron; left/right gem sockets; complete P1-P3 plus three extinguished seals, opened door and real W-input exit trigger; wrong physical ingredient batch respawn; shared physical potion retirement; automatic no-HMD fallback; rig/architecture switching; simulated XR controller rune/Grab input and hand-local carry placement. Screenshots overview.png / frames.png / chest.png were rendered from the game camera and visually inspected.

## Corrections found during validation
- XRRayInteractor initially derived its ray origin from the carry attach point: explicitly separated camera ray origin and hold point.
- Socket event transactions needed deferral and selection guards to avoid double SelectExit assertions.
- Preserved human-height doorway while reducing room height; closed the sky-visible header gap.
- Corrected chest lid approach, held-item/player collision and stale aim after test teleports.
- Shared potion consumption now removes the physical reward even when applied from inventory/free VR hand, preventing a duplicate pickup. E cannot bypass shared XRI transactions in Warehouse.
- Kept picture numeral plates exposed to side approaches because stage-3 box movement is deliberately absent. Moved spawn clear of the south storage row. Corrected the Editor clue-light lookup prefix before final scene application. Storage crates now stack in supported columns rather than floating tiers.
- No original test expectations were weakened or puzzle answers changed. Synthetic controller test release uses an explicit cleared button state.

## Main decisions
- XRI 3.3.2, OpenXR 1.16.1: official Unity 6.3 package listings, installed package metadata and actual Unity resolve/compile/EditMode version assertions. XR Device Simulator is the matching official XRI sample, not a separate package. Source links: ../../WAREHOUSE_XRI.md.
- P2 moonpetal: southeast window planter (3.75,.78,-2.94). Salamander scale: east shelf glass bottle (3.9,1.03,1.05). Crystal dust: inside southwest open chest (-3.25,.36,-2.3). Toadstool: northwest floor (-3.85,.13,2.7).
- Initialize OpenXR, allow subsystem state publication, select VR when XRSettings.isDeviceActive; otherwise desktop. F9/Inspector provide development override; forced VR without HMD enables the simulator.
- Inventory remains the original puzzle transaction ledger; P2 rewards also have physical objects. Classroom is not the shipped startup scene, only a regression fixture.

## Unverified / deliberate exclusions
- No real headset validation; comfort, tracking origin on hardware, controller mappings, teleport/snap-turn end-to-end, VR UI usability and performance still require headset/manual QA.
- Simulated XR controller events were tested; a full manual classic XR Device Simulator playthrough was not performed.
- No standalone/player build or S11 acceptance; no new stage-3 or stage-4 behaviour.
- Legacy RuntimeCompileCheck stubs do not model XRI and were not used as a substitute for actual Unity compilation.
- AGENTS.md and README.md are now synchronized with WAREHOUSE_XRI.md (see layout follow-up below). Notion was not synchronized.
- Pre-existing untracked validation XML was not altered or staged. Additional local diagnostic runs remain untracked; only final evidence is committed.

## Reproduce
See commands.txt and ../../WAREHOUSE_XRI.md. Run one Unity process at a time; do not build S11 for this scope.

## Layout follow-up — 2026-11-16

### Changes and root cause
- The four glowing bars were the existing S10c picture-light housings/bulbs (`ArtB_C_PictureLamp_0..3`, `ArtB_C_PictureBulb_0..3`), not spare gameplay objects. They were in a separate art root, so moving only Painting transforms left them floating. Each is now parented to its frame and joined by a short brass mount; original answer lights are attached too. No puzzle-light behaviour was removed.
- III/II wall paintings have identical 1.65m centres and no tilt, clear of beams. I/IV lean 12 degrees with their visible bottom at 0.015m and rear edge against the west wall (-4.425m). Their centres are computed from rendered geometry, not guessed collider pivots. Original fixed crates still partly obscure them; all four numerals are visible in frames.png.
- Reparented I-IV plaques/text to their own frame. Reset TextMeshPro RectTransform anchoredPosition3D after moving all ancestors: resetting only localPosition had left II's cached anchor/mesh displaced. This explains and prevents the detached II seen in overview.png.
- Added plank seams on all sides/lids, wooden edge rails, iron bindings and metal studs to all 13 warehouse storage/reward crates. Existing timber texture and warm/dark palette retained, no external assets.
- Added two EditMode regression tests for all four frame assemblies (floor/wall contact, matching wall height, beam clearance, numeral/light attachment) and all 13 crates (floor/full stack support, detail attachment). This is deliberately scoped to the changed decor, not a universal audit of every legacy prop.
- AGENTS.md / README.md now describe 9x7m Warehouse, shared XRI, automatic/Inspector/F9 rigs and exact P2 locations. Puzzle answers and item IDs unchanged; no runtime/Core code change.

### Final actual verification
| Run | Passed | Failed | Evidence |
|---|---:|---:|---|
| Unity EditMode | 6 | 0 | layout-editmode-final.xml |
| Unity PlayMode (all historical + warehouse) | 28 | 0 | layout-playmode-final.xml |
| Pure C# dotnet tests | 44 | 0 | layout-core-summary.json |

- Full Warehouse batchmode generation exited 0 using Unity 6000.3.28f1. Historical Classroom/art regeneration noise was restored before final test runs; original historical test assertions were not changed.
- overview.png / frames.png (and chest.png) were overwritten by the existing game-camera capture test, at unchanged camera coordinates and 1280x720 resolution. Both requested images were directly opened and visually inspected: attached lamps, aligned wall frames, floor-supported leaners, I-IV labels and crate detail confirmed. No detached II/bar remains in overview.
- Final PlayMode XML reports zero failed tests; no Exception/Assertion events found. The Editor log still contains a nonfatal licensing access-token update message and an OpenXR Display initialization failure without HMD; desktop fallback and simulated-controller tests pass. These are not real-headset validation.
- Existing untracked validation XML/TRX files were not overwritten, staged or removed. Final evidence uses new layout-prefixed filenames; raw logs/TRX remain in ignored UnityProject/Logs.
- Remaining: real HMD/comfort/manual simulator QA and standalone build are still unverified. No clearing/pushing, new darkening or S11 build was attempted.
