# Warehouse stage 4 - progressive darkness

## Changes and design decisions
- Warehouse enables progressiveDarkening. WarehouseDarkening owns the shared PC/VR presentation; Core answers, IDs, reward transactions, clearing gates and P1 -> P2 -> P3 are unchanged.
- Lighting data: UnityProject/Assets/Settings/WarehouseLighting.asset, created/serialized by Unity from WarehouseLightingProfile. No scene/prefab/ScriptableObject YAML was authored by hand.
- Smooth 1.6-second transitions; no input lock, camera shake, brightness flash or new audio clue. Flame height shrinks to zero; a four-particle thin wisp marks extinction. Remaining flames/light have bounded low-frequency flicker.
- Floor candles around the completed altar go first, followed by the brew-side floor candles. Wall sconces participate. One dim navigation wick survives at the exit end.
- Ambient RGB = (.70, .80, 1) * [ .15, .105, .065, .035 ].
- Reflection = [ .65, .40, .22, .10 ]; directional fill = [ .22, .15, .085, .035 ].
- Background multiplier = [ 1, .62, .30, .10 ]; surviving candle multiplier = [ 1, .78, .53, .28 ]; flicker amplitude = [ .025, .045, .065, .085 ]. Existing fog is unchanged.
- After its gold success feedback, the solved cauldron emission fades to 6% so it does not compete with P3. Legacy AtmosphereMotion/FireFlicker light writers do not overwrite Warehouse attenuation. Answer-frame lights are attenuated even if initially disabled.
- Six local lights stay available: cauldron liquid surface, moonpetal planter, scale bottle, gated crystal-dust chest, inscription/sockets, exit. Each uses profile intensity .65, range 2.1 m, color (.35, .55, .70); these do not indicate answers or bypass clearing.
- No dawn light/window/rays are activated in Warehouse. Both cameras clear to black (the open doorway previously exposed a blue background). Fade and black ending backgrounds stretch to the canvas.
- ShowPicture fits parchment inside 1120 x 500 reference pixels; CloseHelp is below it, at y=-290, and last in sibling order. Applies to every picture inspection, not only the element table.

## Regeneration noise (234 initial modified entries)
Warehouse previously called S10f(), which rebuilds Classroom and rewrites shared materials, prefabs, item assets, dynamic fonts, icons and historical audits. Regression tests also overwrite old screenshots/audits. Serialized scene object IDs, cached shader properties, accumulated material settings, line-ending reserialization and time-dependent captures caused unrelated differences.

These historical changes were not needed for stage 4 and were restored to HEAD, not committed as a new Classroom baseline. HistoricalAssetScope now snapshots/restores that output around Warehouse construction, including exceptional exits. Run-Unity.ps1 snapshots tracked historical files **before starting Unity**, covering initial-import serialization as well as test captures, and restores after the process exits. The Warehouse scene, new profile and stage4 evidence remain outputs.

All pre-existing untracked evidence is retained without staging/deletion. Local non-XML/TRX scratch captures/logs are excluded via .git/info/exclude; stage4 raw machine/environment XML/TRX and nested logs are ignored. The original XML/TRX set remains visible in git status. Source mirrors in CodeDrafts/Runtime match Assets/Scripts/Runtime.

## Actual verification
| Check | Passed | Failed | Local evidence |
|---|---:|---:|---|
| Full Warehouse batch regeneration | exit 0 | - | stage4-release-generate.log |
| EditMode, full | 7 | 0 | stage4-release-editmode.xml |
| PlayMode, full legacy + Warehouse | 35 | 0 | stage4-release-playmode.xml |
| Added darkening tests, focused rerun | 3 | 0 | stage4-release-smoke.xml |
| Pure C# | 44 | 0 | stage4-core.trx |

The original 36 untracked XML/TRX files have unchanged SHA-256 hashes. All 234 initial modified historical files were restored; they are not included in feature commits.

See stage4-summary.json for sanitized final suite results, test names, screenshot SHA-256 and luminance measurements. Raw XML/TRX/logs remain local. Executed on Windows, Unity 6000.3.28f1; only one Unity Editor at a time.

The three added PlayMode tests cover:
1. Actual P1/P2/P3 completion, decreasing representative light sum, expected enabled wicks from authoring data, ambient stage values, all six local lights alive, same frozen overview camera, both cameras black and no dawn.
2. Shared XRI chest selection/opening and ingredient consumption **during** the 1.6s blend, complete puzzle flow, passage ready, black fade, black ending.
3. Every picture texture inspection keeps its help rectangle outside the parchment, including element table, paintings and recipe.

The full legacy + Warehouse PlayMode suite also exercises actual keyboard/mouse XRI, controller simulation, clearing/ingredient gates, original puzzle answers and a collision-tested W-only exit.

## Visual inspection
Directly opened proposal slide 09 and all four original darkness JPGs, then opened the actual generated 1280x720 captures. Overview captures use the same player (0,0,-2.3), target (.1,1.5,3.2), with time frozen during capture after transitions/feedback settle. No screenshot postprocessing or gamma edits.

- stage4-start.png / stage4-p1.png / stage4-p2.png / stage4-p3.png: progressively reduced warm room light; final exit stays dark, no blue sky rectangle.
- stage4-p1-cauldron.png: next brew surface and surrounding shelf are readable.
- stage4-p2-inscription.png: next tablet and socket assembly remain readable.
- stage4-final-sockets.png: final material colors and navigation remain visible.
- stage4-element-table-close.png: help text is visibly separated below the parchment.
- stage4-black-fade.png / stage4-ending.png: black fade and black-background ending.

Overview mean 8-bit luminance: 26.765 -> 19.044 -> 11.489 -> 6.932. This is a screenshot diagnostic, not a photometric light-unit measurement. Black fade mean: .011/255.

The proposal is an illustrative painted scene, not a pixel-matching target; its warm-to-dark stage progression is retained while local task visibility is deliberately stronger.

## Reproduction
```powershell
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage4-release-generate -Method MagicEscape.EditorTools.ClassroomBuilder.Warehouse
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage4-release-smoke -Tests -TestFilter WarehouseDarkeningTests
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage4-release-editmode -Tests -TestPlatform EditMode
powershell -ExecutionPolicy Bypass -File Tools/Run-Unity.ps1 -Name warehouse/stage4-release-playmode -Tests
dotnet test Tests/MagicEscape.Core.Tests --logger "trx;LogFileName=stage4-core.trx" --results-directory docs/validation/warehouse
```
Use fresh output names for later runs, rather than replacing acceptance evidence.

## Remaining / not verified
No automated failure remains in the accepted suites. Real HMD visual comfort, brightness perception, headset-specific UI coverage and physical VR playthrough remain unverified; synthetic controller tests are not hardware validation. No S11 standalone player build was performed. Notion was not updated. Unity licensing refresh and no-HMD OpenXR initialization warnings remain environmental; their presence is not represented as a hardware pass.
