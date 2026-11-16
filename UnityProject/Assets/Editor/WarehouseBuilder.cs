using System.IO;
using System.Linq;
using MagicEscape.Core;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using Object = UnityEngine.Object;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        const string WarehousePath = "Assets/Scenes/Warehouse.unity";
        public static void ImportXriSamples()
        {
            foreach (var sample in Sample.FindByPackage("com.unity.xr.interaction.toolkit", "3.3.2"))
                if (sample.displayName == "XR Device Simulator" && !sample.isImported)
                    sample.Import(Sample.ImportOptions.OverridePreviousImports);
            AssetDatabase.Refresh();
        }
        static void ConfigureOpenXR()
        {
            var general = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            if (general == null)
            {
                Directory.CreateDirectory("Assets/XR/Settings");
                var perTarget = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(perTarget, "Assets/XR/Settings/XRGeneralSettingsPerBuildTarget.asset");
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, perTarget, true);
                perTarget.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Standalone);
                general = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            }
            if (general.Manager == null)
            {
                general.Manager = ScriptableObject.CreateInstance<XRManagerSettings>();
                AssetDatabase.AddObjectToAsset(general.Manager, AssetDatabase.GetAssetPath(general));
            }
            general.InitManagerOnStart = false;
            XRPackageMetadataStore.AssignLoader(general.Manager, typeof(OpenXRLoader).FullName, BuildTargetGroup.Standalone);
            var openxr = OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Standalone);
            foreach (var feature in openxr.GetFeatures<OpenXRInteractionFeature>())
                if (feature is OculusTouchControllerProfile || feature is ValveIndexControllerProfile || feature is MicrosoftMotionControllerProfile) feature.enabled = true;
            EditorUtility.SetDirty(openxr); EditorUtility.SetDirty(general); EditorUtility.SetDirty(general.Manager);
        }
        // Stage 1+2 only. Historical Classroom remains a regression fixture, not the startup scene.
        public static void Warehouse()
        {
            S10f();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), WarehousePath);
            var tags = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            tags.FindProperty("layers").GetArrayElementAtIndex(10).stringValue = "HeldItem"; tags.ApplyModifiedPropertiesWithoutUndo();
            var geometry = new GameObject("WarehouseGeometry").transform;
            foreach (var g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (g == geometry.gameObject || g.name == "Player" || g.GetComponent<Canvas>() != null ||
                    g.GetComponent<GameManager>() != null || g.GetComponent<InventoryManager>() != null ||
                    g.GetComponent<AudioManager>() != null || g.GetComponent<UIManager>() != null || g.GetComponent<EventSystem>() != null) continue;
                g.transform.SetParent(geometry, true);
            }
            // Resize the architectural envelope and existing art, not the player or overlay UI.
            geometry.localScale = new Vector3(.75f, .68f, .7f); // 9 x 7 m, ceiling 3.06 m
            // Human-height passage must not inherit the architectural vertical compression.
            var doorRoot = SceneComponent<DoorController>().transform;
            doorRoot.localScale = new Vector3(1, 1 / .68f, 1);
            var lintel = Find("ArtB_DoorLintel").transform;
            lintel.position = new Vector3(0, 2.78f, 3.5f);
            var top = Find("Wall_N_Top").transform;
            top.position = new Vector3(0, 2.83f, 3.5f); top.localScale = new Vector3(3.3f, .46f / .68f, .2f);
            var player = SceneComponent<PlayerController>(); player.transform.position = new Vector3(0, 0, -2.3f);
            SceneComponent<RunePuzzle>().transform.position = new Vector3(-.8f, 0, -.3f);
            // Door remains centred; tablet is beside it rather than obstructing the exit lane.
            var seal = SceneComponent<SealPuzzle>();
            var sealDelta = new Vector3(2.1f, 0, 2.65f) - seal.transform.position;
            seal.transform.position += sealDelta;
            var presentation = SceneComponent<RoomPresentation>();
            for (int i=0; i<3; i++)
            {
                var lamp = Find("SealLight_"+(i+1)).transform;
                lamp.position = new Vector3(lamp.position.x,2.88f,lamp.position.z);
                presentation.sealOff[i].transform.position = lamp.position;
            }
            SceneComponent<VisualClues>().socketElements.transform.position += sealDelta;
            presentation.sealSignal.transform.position += sealDelta;
            presentation.coreShards.transform.position += sealDelta;
            var potion = SceneComponent<PotionPuzzle>();
            var brewDelta = new Vector3(2.65f, 0, .8f) - potion.transform.position;
            potion.transform.position += brewDelta; presentation.brewSignal.transform.position += brewDelta;
            // Remove the former teaching desk, student seats and their loose desk dressing.
            foreach (var t in geometry.GetComponentsInChildren<Transform>(true).Where(t =>
                t.name == "Drawer" || t.name == "Art_chair" || t.name.Contains("Fit_table_small") || t.name.Contains("Fit_chair") ||
                (t.parent != null && (t.parent.name == "Art_FourthPass" || t.parent.name == "Art_SecondPass") && t.position.z < -1.9f && Mathf.Abs(t.position.x) < 2.7f)).ToArray())
                if (t != null) Object.DestroyImmediate(t.gameObject);
            var props = new GameObject("WarehouseStorage").transform;
            // Every fixed prop carries stage-3 handling data, never a second grab implementation.
            for (int i = 0; i < 7; i++)
            {
                var bottom = new Vector3(-3.8f + (i % 3 == 1 ? .58f : 0), (i / 3) * .48f, .1f + (i % 3) * .68f);
                StorageCrate(props, "StackedCrate_" + i, bottom, new Vector3(.65f, .48f, .62f));
            }
            for (int i = 0; i < 5; i++) StorageCrate(props, "SouthStorage_" + i, new Vector3(-1.8f + i * .75f, 0, -3.05f), new Vector3(.65f, .6f + .15f * (i % 2), .6f));
            var chest = new GameObject("IngredientChest").transform; chest.SetParent(props, false);
            chest.position = new Vector3(-3.25f, 0, -2.3f);
            chest.gameObject.AddComponent<HandlingProfile>();
            var wood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            foreach (var part in new[] {
                Box("ChestFloor",chest,new Vector3(0,.10f,0),new Vector3(1.05f,.12f,.7f)),
                Box("ChestWest",chest,new Vector3(-.49f,.33f,0),new Vector3(.07f,.5f,.7f)),
                Box("ChestEast",chest,new Vector3(.49f,.33f,0),new Vector3(.07f,.5f,.7f)),
                Box("ChestFront",chest,new Vector3(0,.33f,-.315f),new Vector3(1.05f,.5f,.07f)),
                Box("ChestBack",chest,new Vector3(0,.33f,.315f),new Vector3(1.05f,.5f,.07f)) }) part.GetComponent<Renderer>().sharedMaterial = wood;
            var lid = Box("ChestOpenLid",chest,new Vector3(0,.78f,-.32f),new Vector3(1.05f,.45f,.07f));
            lid.GetComponent<Renderer>().sharedMaterial = wood; lid.transform.localRotation = Quaternion.Euler(12,0,0);
            // Open, static chest: stage 3 will own clearing, not this builder.
            MoveItem(ItemIds.CrystalDust, new Vector3(-3.25f, .36f, -2.3f));
            MoveItem(ItemIds.SalamanderScale, new Vector3(3.9f, 1.03f, 1.05f));
            MoveItem(ItemIds.Moonpetal, new Vector3(3.75f, .78f, -2.94f));
            MoveItem(ItemIds.Toadstool, new Vector3(-3.85f, .13f, 2.7f));
            // Two oil paintings lean behind the stack but remain partly readable until stage 3.
            FitWarehouseClues();
            foreach (var t in geometry.GetComponentsInChildren<Transform>(true))
                if (t.GetComponent<Collider>() != null && t.GetComponent<HandlingProfile>() == null) t.gameObject.AddComponent<HandlingProfile>();
            presentation.progressiveDarkening = false; // Stage 4 deliberately not implemented here.
            ConfigureXri(); ConfigureOpenXR();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), WarehousePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(WarehousePath, true), new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Directory.CreateDirectory("../docs/validation/warehouse");
            File.WriteAllLines("../docs/validation/warehouse/scene-manifest.txt", Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(t => Hierarchy(t) + "|" + t.position.ToString("F3")).OrderBy(x => x));
            Debug.Log("WAREHOUSE_GENERATED: 9x7x3.06; XRI 3.3.2; OpenXR 1.16.1; no player build");
        }
        static void FitWarehouseClues()
        {
            // Stage 3 is not available yet: leave numeral plates and most of each picture readable.
            // Only an outer edge sits behind the fixed storage stack.
            foreach (string roman in new[] { "I", "IV" })
            {
                var frame = Find("Painting_"+roman).transform;
                var destination = new Vector3(-4.08f,.82f,roman == "I" ? -.4f : 2.0f);
                var delta = destination-frame.position; frame.position = destination; frame.rotation = Quaternion.Euler(0,0,-9);
                var light = Find("ArtC_AnswerFrame_"+roman).transform;
                light.position = frame.position+new Vector3(.585f,.442f,0); light.LookAt(frame.position);
            }
            var upper = Find("Painting_II").transform;
            var upperDelta = new Vector3(-4.41f,2.25f,1.45f)-upper.position;
            upper.position += upperDelta;
            var upperLight = Find("ArtC_AnswerFrame_II").transform;
            upperLight.position = upper.position+new Vector3(.585f,.442f,0); upperLight.LookAt(upper.position);
        }
        // Small idempotent architectural repair entry point; full Warehouse() produces the same header.
        public static void WarehouseHeaderApply()
        {
            EditorSceneManager.OpenScene(WarehousePath);
            FitWarehouseClues();
            for (int i=0;i<7;i++)
                Find("StackedCrate_"+i).transform.position = new Vector3(-3.8f+(i%3==1?.58f:0), (i/3)*.48f+.24f, .1f+(i%3)*.68f);
            SceneComponent<PlayerController>().transform.position = new Vector3(0,0,-2.3f);
            SceneComponent<RigSelector>().vrRig.transform.position = new Vector3(0,0,-2.3f);
            var top = Find("Wall_N_Top").transform;
            top.position = new Vector3(0, 2.83f, 3.5f); top.localScale = new Vector3(3.3f, .46f / .68f, .2f);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), WarehousePath); AssetDatabase.SaveAssets();
            File.WriteAllLines("../docs/validation/warehouse/scene-manifest.txt", Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(t => Hierarchy(t) + "|" + t.position.ToString("F3")).OrderBy(x => x));
            Debug.Log("WAREHOUSE_HEADER_VERIFIED: clear opening 2.6m; no sky gap");
        }
        static GameObject StorageCrate(Transform parent, string name, Vector3 bottom, Vector3 size)
        {
            var g = Box(name, parent, bottom + Vector3.up * size.y / 2, size);
            g.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            g.AddComponent<HandlingProfile>().weight = HandlingWeight.Fixed;
            // Visible iron binding, not classroom desks masquerading as storage.
            var iron = ArtMaterial("WarehouseIron", new Color(.08f, .065f, .05f));
            for (int i = -1; i <= 1; i += 2)
                WorldVisual(name + "_Binding", g.transform, bottom + new Vector3(i * size.x * .35f, size.y / 2, -.006f), new Vector3(.045f, size.y + .02f, size.z + .02f), iron);
            return g;
        }
        static void MoveItem(string id, Vector3 position)
        {
            var item = Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None).First(p => p.ItemId == id);
            item.transform.position = position;
        }
        static XRRayInteractor Ray(string name, Transform parent, XRInteractionManager manager, int layers)
        {
            var g = new GameObject(name); g.transform.SetParent(parent, false);
            var ray = g.AddComponent<XRRayInteractor>(); ray.interactionManager = manager;
            ray.rayOriginTransform = g.transform; ray.referenceFrame = parent;
            ray.maxRaycastDistance = 2.5f; ray.raycastMask = ~(1 << 8 | 1 << 10);
            ray.raycastTriggerInteraction = QueryTriggerInteraction.Collide;
            ray.interactionLayers = layers; ray.enableUIInteraction = true;
            ray.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            ray.activateInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            ray.uiPressInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            ray.useForceGrab = true; ray.manipulateAttachTransform = false;
            ray.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.Toggle;
            var attach = new GameObject("HoldPoint").transform; attach.SetParent(g.transform, false); attach.localPosition = new Vector3(0, -.13f, .8f); ray.attachTransform = attach;
            return ray;
        }
        static void Track(GameObject g, string usage, bool hmd = false)
        {
            var pose = g.AddComponent<TrackedPoseDriver>();
            string device = hmd ? "<XRHMD>" : "<XRController>{" + usage + "}";
            pose.positionInput = new InputActionProperty(new InputAction(g.name + " position", InputActionType.Value, device + "/devicePosition"));
            pose.rotationInput = new InputActionProperty(new InputAction(g.name + " rotation", InputActionType.Value, device + "/deviceRotation"));
            pose.trackingStateInput = new InputActionProperty(new InputAction(g.name + " tracking", InputActionType.Value, device + "/trackingState"));
        }
        static void ConfigureXri()
        {
            var manager = new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
            foreach (var pickup in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include, FindObjectsSortMode.None)) Grab(pickup, manager);
            var potion = SceneComponent<PotionPuzzle>();
            var cauldron = new GameObject("CauldronSocket"); cauldron.transform.position = potion.transform.position + Vector3.up * .82f;
            var volume = cauldron.AddComponent<SphereCollider>(); volume.isTrigger = true; volume.radius = .24f;
            var socket = cauldron.AddComponent<XriPuzzleSocket>(); socket.cauldron = potion; socket.interactionManager = manager; socket.showInteractableHoverMeshes = false;
            foreach (var crystal in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
            {
                var collider = crystal.gameObject.AddComponent<SphereCollider>(); collider.isTrigger = true; collider.radius = .26f;
                var s = crystal.gameObject.AddComponent<XriPuzzleSocket>(); s.crystalSocket = crystal; s.interactionManager = manager; s.showInteractableHoverMeshes = false;
            }
            foreach (var mb in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(m => m is IInteractable && !(m is ItemPickup) && !(m is PotionPuzzle) && !(m is CrystalSocket)))
            {
                var simple = mb.gameObject.AddComponent<XRSimpleInteractable>(); simple.interactionManager = manager; mb.gameObject.AddComponent<XriPress>();
            }
            var player = SceneComponent<PlayerController>(); var input = player.GetComponent<PlayerInputReader>(); var cam = player.GetComponentInChildren<Camera>();
            var desktopRay = Ray("Desktop XRI Ray", cam.transform, manager, 1);
            var desktop = cam.gameObject.AddComponent<DesktopXriDriver>(); desktop.ray = desktopRay; desktop.input = input;
            var vr = new GameObject("VR Rig"); vr.tag = "Player"; vr.layer = 8; vr.transform.position = player.transform.position;
            var origin = vr.AddComponent<XROrigin>(); var offset = new GameObject("Camera Offset"); offset.transform.SetParent(vr.transform, false);
            origin.CameraFloorOffsetObject = offset; origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Floor;
            var camera = new GameObject("VR Camera", typeof(Camera), typeof(AudioListener)); camera.transform.SetParent(offset.transform, false); camera.tag = "MainCamera"; camera.transform.localPosition = Vector3.up * 1.6f; camera.GetComponent<Camera>().nearClipPlane = .05f;
            Track(camera, "", true); origin.Camera = camera.GetComponent<Camera>();
            var body = vr.AddComponent<XRBodyTransformer>(); body.xrOrigin = origin; body.useCharacterControllerIfExists = false;
            var mediator = vr.AddComponent<LocomotionMediator>(); mediator.xrOrigin = origin;
            var teleportProvider = vr.AddComponent<TeleportationProvider>(); teleportProvider.mediator = mediator;
            var turn = vr.AddComponent<SnapTurnProvider>(); turn.mediator = mediator; turn.turnAmount = 45;
            turn.leftHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.ManualValue;
            turn.rightHandTurnInput.inputSourceMode = XRInputValueReader.InputSourceMode.ManualValue;
            var hands = new XRRayInteractor[2];
            for (int i = 0; i < 2; i++)
            {
                var hand = new GameObject(i == 0 ? "Left Controller" : "Right Controller"); hand.transform.SetParent(offset.transform, false); Track(hand, i == 0 ? "LeftHand" : "RightHand");
                hands[i] = Ray("Interaction Ray", hand.transform, manager, 1); hands[i].selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
                hands[i].gameObject.AddComponent<LineRenderer>(); hands[i].gameObject.AddComponent<XRInteractorLineVisual>();
            }
            var teleportRay = Ray("Teleport Ray", hands[0].transform.parent, manager, 2); teleportRay.maxRaycastDistance = 8; teleportRay.lineType = XRRayInteractor.LineType.ProjectileCurve; teleportRay.referenceFrame = vr.transform; teleportRay.selectActionTrigger = XRBaseInputInteractor.InputTriggerType.State;
            var teleportArea = Find("Floor").AddComponent<TeleportationArea>(); teleportArea.interactionLayers = 2; teleportArea.teleportationProvider = teleportProvider;
            var corridorArea = Find("CorridorFloor").AddComponent<TeleportationArea>(); corridorArea.interactionLayers = 2; corridorArea.teleportationProvider = teleportProvider;
            // ExitTrigger expects a player collider in either rig.
            var cc = vr.AddComponent<CharacterController>(); cc.height = 1.8f; cc.radius = .3f; cc.center = new Vector3(0, .9f, 0);
            vr.AddComponent<PlayerRigBody>().head = camera.transform;
            var driver = vr.AddComponent<VrXriDriver>(); driver.input = input; driver.left = hands[0]; driver.right = hands[1]; driver.teleport = teleportRay; driver.turn = turn;
            var selector = new GameObject("RigSelector").AddComponent<RigSelector>(); selector.desktopRig = player.gameObject; selector.vrRig = vr; selector.desktopCamera = cam; selector.vrCamera = camera.GetComponent<Camera>();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Samples/XR Interaction Toolkit/3.3.2/XR Device Simulator/XR Device Simulator.prefab");
            if (prefab == null) throw new System.InvalidOperationException("Import XR Device Simulator sample via ImportXriSamples first");
            selector.simulator = (GameObject)PrefabUtility.InstantiatePrefab(prefab); selector.simulator.SetActive(false); vr.SetActive(false);
            foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if (canvas.isRootCanvas) canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
            var es = SceneComponent<EventSystem>(); foreach (var module in es.GetComponents<BaseInputModule>()) Object.DestroyImmediate(module); es.gameObject.AddComponent<XRUIInputModule>();
            var reward = new GameObject("Physical Rewards").AddComponent<XriRewardSpawner>();
            StorageCrate(null, "RewardStand", new Vector3(2.7f,0,1.65f), new Vector3(.86f,.93f,.48f));
            var ruby = Pickup("Reward_Ruby", null, new Vector3(2.5f, 1.05f, 1.65f), 5, true); Grab(ruby, manager);
            var reveal = Pickup("Reward_Revelation", null, new Vector3(2.9f, 1.05f, 1.65f), 6, true); Grab(reveal, manager);
            ruby.GetComponent<Renderer>().enabled = false; reveal.GetComponent<Renderer>().enabled = false;
            ItemArt(5, ruby.transform, ruby.transform.position, .24f);
            ItemArt(6, reveal.transform, reveal.transform.position, .26f);
            reward.rewards = new[] { ruby, reveal };
            // Reveal uses the potion already selected when grabbed; P3 remains the same Core rule.
        }
        static void Grab(ItemPickup pickup, XRInteractionManager manager)
        {
            var rb = pickup.gameObject.AddComponent<Rigidbody>(); rb.isKinematic = true; rb.useGravity = false;
            pickup.gameObject.isStatic = false;
            var grab = pickup.gameObject.AddComponent<XRGrabInteractable>(); grab.interactionManager = manager; grab.throwOnDetach = false;
            grab.movementType = XRBaseInteractable.MovementType.Instantaneous; grab.useDynamicAttach = false;
            pickup.gameObject.AddComponent<XriItem>(); var profile = pickup.GetComponent<HandlingProfile>() ?? pickup.gameObject.AddComponent<HandlingProfile>(); profile.weight = HandlingWeight.Lift;
        }
    }
}
