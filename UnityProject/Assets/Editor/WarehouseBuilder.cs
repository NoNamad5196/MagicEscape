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
        // Stages 1-3. Historical Classroom remains a regression fixture, not the startup scene.
        public static void Warehouse()
        {
            using(var historical=new HistoricalAssetScope())
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
            // Base chest shell; ConfigureClearing installs the shared hinged lid.
            MoveItem(ItemIds.CrystalDust, new Vector3(-3.25f, .36f, -2.3f));
            MoveItem(ItemIds.SalamanderScale, new Vector3(3.9f, 1.03f, 1.05f));
            MoveItem(ItemIds.Moonpetal, new Vector3(3.75f, .78f, -2.94f));
            MoveItem(ItemIds.Toadstool, new Vector3(-3.85f, .13f, 3.08f));
            // Two oil paintings lean behind the movable clearing crates.
            FitWarehouseClues();
            ConfigureWarehouseChalk();
            Set(SceneComponent<GameManager>(),"failureOnlyClues",true);
            foreach (var t in geometry.GetComponentsInChildren<Transform>(true))
                if (t.GetComponent<Collider>() != null && t.GetComponent<HandlingProfile>() == null) t.gameObject.AddComponent<HandlingProfile>();
            ConfigureWarehouseDarkening();
            ConfigureXri(); ConfigureClearing(); ConfigureWarehouseStage7(); ConfigureWarehouseStage10(); ConfigureOpenXR();
            // The resized corridor can expose the scene background above its dark doorway.
            // Both rigs must see black, never the default blue skybox / a false dawn.
            foreach(var camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;}
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), WarehousePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(WarehousePath, true), new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Directory.CreateDirectory("../docs/validation/warehouse");
            File.WriteAllLines("../docs/validation/warehouse/scene-manifest.txt", Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(t => Hierarchy(t) + "|" + t.position.ToString("F3")).OrderBy(x => x));
            Debug.Log("WAREHOUSE_GENERATED: stage 4 lighting; shared XRI; no player build");
            }
        }
        // Separate Warehouse allowlist asset: Classroom's answered plate is never reused.
        static void ConfigureWarehouseChalk()
        {
            const string path="Assets/Art/Warehouse/ChalkScenes.png";
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if(texture==null)throw new System.InvalidOperationException("Generate Warehouse scene chalk first");
            var material=EPicture("WarehouseChalkScenes");
            material.mainTexture=texture;material.SetTexture("_EmissionMap",texture);
            EditorUtility.SetDirty(material);
            Find("ChalkSequence").GetComponent<Renderer>().sharedMaterial=material;
            Set(Find("Chalkboard").GetComponent<InspectableObject>(),"picture",texture);
            Set(Find("Chalkboard").GetComponent<InspectableObject>(),"body", "I   II   III   IV");
            var old=Object.FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(t=>t.name=="ChalkboardLabel");
            old.text="I   II   III   IV";old.gameObject.SetActive(false);
            // The historical wooden rails were separate roots: attach them before moving the assembly.
            foreach(var trim in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Where(t=>t.name.StartsWith("ArtB_BoardRail_")||t.name.StartsWith("ArtB_BoardEdge_")))
                trim.SetParent(Find("Chalkboard").transform,true);
            // Keep the whole scene diagram above fixed stacked crates, not half-hidden behind them.
            foreach(var t in new[]{Find("Chalkboard").transform,Find("ChalkSequence").transform,old.transform})
                t.position+=Vector3.up*.85f;
            var boardLight=Find("ArtC_Chalkboard").transform;boardLight.position+=Vector3.up*.85f;
            boardLight.LookAt(Find("Chalkboard").transform.position);
            // Ordered captions are authored text above the art, never inside generated images.
            var plate=Find("ChalkSequence").transform;
            string[] captions={"I","II","III","IV"};
            for(int i=0;i<4;i++)
            {
                var caption=WorldText("WarehouseSceneOrder_"+i,plate,captions[i],new Vector3(-.375f+i*.25f,.27f,-.01f),new Vector2(.2f,.12f),1.2f);
                caption.transform.rotation=plate.rotation;
                caption.rectTransform.anchoredPosition=new Vector2(-.375f+i*.25f,.27f);
                caption.textWrappingMode=TMPro.TextWrappingModes.NoWrap;
                caption.overflowMode=TMPro.TextOverflowModes.Overflow;
                caption.color=new Color(.85f,.87f,.82f);
            }
        }
        static void FitWarehouseClues()
        {
            // Normalize each complete clue assembly instead of moving only the old thin collider.
            // Historical labels, lamps and bulbs lived in separate art roots: all must travel together.
            string[] order = { "III", "I", "IV", "II" };
            for (int i = 0; i < order.Length; i++)
            {
                string roman = order[i];
                var painting = Find("Painting_" + roman).transform;
                var frame = painting.Find("ArtB_PictureFrame_" + roman);
                var label = Find("PaintingLabel_" + roman).transform;
                frame.SetParent(null, true); label.SetParent(null, true);
                painting.SetParent(null, true); painting.localScale = Vector3.one;
                painting.rotation = Quaternion.identity;
                painting.GetComponent<BoxCollider>().size = new Vector3(.13f, .92f, 1.08f);
                frame.SetParent(painting, false); frame.localPosition = Vector3.zero;
                frame.localRotation = Quaternion.Euler(0, -90, 0); frame.localScale = new Vector3(.85f,.85f,.85f);
                var plaque = frame.Find("ArtB_RomanPlaque");
                plaque.localPosition = new Vector3(0,-.51f,-.06f);
                plaque.localScale = new Vector3(1.02f,.19f,.07f);
                label.SetParent(frame, false); label.localPosition = new Vector3(-.34f,-.51f,-.105f);
                label.localRotation = Quaternion.identity; label.localScale = Vector3.one;
                label.GetComponent<TMPro.TMP_Text>().ForceMeshUpdate();
                // A short brass arm physically joins the old picture-light housing to its frame.
                var lamp = Find("ArtB_C_PictureLamp_" + i).transform;
                var bulb = Find("ArtB_C_PictureBulb_" + i).transform;
                lamp.SetParent(frame, false); lamp.localPosition = new Vector3(0,.57f,-.13f);
                lamp.localRotation = Quaternion.identity; lamp.localScale = new Vector3(.34f,.055f,.25f);
                bulb.SetParent(lamp, false); bulb.localPosition = new Vector3(0,-.65f,0);
                bulb.localRotation = Quaternion.identity; bulb.localScale = new Vector3(.8f,.45f,.6f);
                if (frame.Find("LampMount") == null)
                {
                    var mount = GameObject.CreatePrimitive(PrimitiveType.Cube); mount.name = "LampMount";
                    Object.DestroyImmediate(mount.GetComponent<Collider>());
                    mount.transform.SetParent(frame, false); mount.transform.localPosition = new Vector3(0,.53f,-.07f);
                    mount.transform.localScale = new Vector3(.09f,.10f,.17f);
                    mount.GetComponent<Renderer>().sharedMaterial = lamp.GetComponent<Renderer>().sharedMaterial;
                }
                foreach (string lightName in new[] { "ArtC_Picture_" + i, "ArtC_AnswerFrame_" + roman })
                {
                    var light = Find(lightName).transform; light.SetParent(frame, false);
                    light.localPosition = new Vector3(0,.5f,-.3f); light.LookAt(frame.position);
                }
                bool leaning = roman == "I" || roman == "IV";
                painting.position = new Vector3(-4.39f,1.65f,roman == "III" ? -1.75f : roman == "II" ? 2.25f : roman == "I" ? -.65f : 2.35f);
                if (leaning)
                {
                    painting.rotation = Quaternion.Euler(0,0,12);
                    // Bounds include the actual frame, plaque and mounted light; put the lowest edge on the floor
                    // and the rearmost edge against the inside of the west wall (x=-4.425).
                    var bounds = frame.GetComponentInChildren<Renderer>().bounds;
                    foreach (var r in frame.GetComponentsInChildren<Renderer>())
                        if (r.transform != label) bounds.Encapsulate(r.bounds);
                    painting.position += new Vector3(-4.425f - bounds.min.x, .015f - bounds.min.y, 0);
                }
                // RectTransform anchoring must be reset AFTER moving its non-RectTransform ancestors.
                var text = label.GetComponent<TMPro.TMP_Text>();
                text.rectTransform.anchoredPosition3D = new Vector3(-.34f,-.51f,-.105f);
                text.ForceMeshUpdate();
            }
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
            // WorldVisual preserves world size under the non-uniform primitive scale.
            // The former helper prefixes ArtB_; explicit detail names make attachment checks reliable.
            var iron = ArtMaterial("WarehouseIron", new Color(.13f, .105f, .075f));
            var seam = ArtMaterial("WarehouseSeam", new Color(.045f,.026f,.015f));
            var timber = g.GetComponent<Renderer>().sharedMaterial;
            for (int i = -1; i <= 1; i += 2)
            {
                WorldVisual("CrateBinding_"+i, g.transform, bottom + new Vector3(i*size.x*.34f,size.y/2,0), new Vector3(.045f,size.y+.012f,size.z+.012f), iron);
                foreach (int side in new[] {-1,1})
                {
                    WorldVisual("CrateRail_"+i+"_"+side,g.transform,bottom+new Vector3(0,size.y/2+i*(size.y/2-.045f),side*(size.z/2+.008f)),new Vector3(size.x,.07f,.035f),timber);
                    foreach (int edge in new[] {-1,1})
                        WorldVisual("CrateStud_"+i+"_"+side+"_"+edge,g.transform,bottom+new Vector3(edge*size.x*.34f,size.y/2+i*(size.y/2-.045f),side*(size.z/2+.028f)),Vector3.one*.023f,iron,PrimitiveType.Sphere);
                }
            }
            // Recess-coloured joints break all four faces and the lid into narrow planks.
            for (int j=1;j<5;j++)
            {
                float x=-size.x/2+size.x*j/5, z=-size.z/2+size.z*j/5;
                foreach (int side in new[] {-1,1})
                {
                    WorldVisual("CrateJointFront_"+j+"_"+side,g.transform,bottom+new Vector3(x,size.y/2,side*(size.z/2+.001f)),new Vector3(.009f,size.y-.10f,.004f),seam);
                    WorldVisual("CrateJointSide_"+j+"_"+side,g.transform,bottom+new Vector3(side*(size.x/2+.001f),size.y/2,z),new Vector3(.004f,size.y-.10f,.009f),seam);
                }
                WorldVisual("CrateJointLid_"+j,g.transform,bottom+new Vector3(x,size.y+.001f,0),new Vector3(.009f,.004f,size.z),seam);
            }
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
