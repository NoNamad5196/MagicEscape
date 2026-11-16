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
- Explicit user proposal/VR instructions supersede historical AGENTS.md PC-only/XR prohibition and classroom locations. AGENTS.md/README.md were not edited; Notion was not synchronized; current specification and overrides are documented in WAREHOUSE_XRI.md and linked from the design/setup/testing documents.
- Pre-existing untracked validation XML was not altered or staged. Additional local diagnostic runs remain untracked; only final evidence is committed.

## Reproduce
See commands.txt and ../../WAREHOUSE_XRI.md. Run one Unity process at a time; do not build S11 for this scope.
