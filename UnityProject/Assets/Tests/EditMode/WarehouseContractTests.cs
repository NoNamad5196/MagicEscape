using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.OpenXR;
public class WarehouseContractTests
{
    [Test] public void Stage10CoversAreSharedSupportedAndOutsidePuzzleCirculation()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        Assert.AreEqual(8,Object.FindObjectsByType<ClearingFeedback>(FindObjectsSortMode.None).Length);
        Assert.AreEqual(6,Object.FindObjectsByType<ClearingGate>(FindObjectsSortMode.None).Length);
        foreach(var feedback in Object.FindObjectsByType<ClearingFeedback>(FindObjectsSortMode.None))
        {
            Assert.AreEqual(1,feedback.GetComponents<XRBaseInteractable>().Length);
            Assert.NotNull(feedback.GetComponent<HandlingProfile>());
        }
        var books=GameObject.Find("ScaleBookStack").GetComponent<XriLooseProp>();
        Assert.AreEqual(1.20f,books.GetComponent<Collider>().bounds.min.y,.002f);
        Assert.That(books.parking.x,Is.InRange(3.38f,4.28f));
        var roll=GameObject.Find("DustPackingRoll").GetComponent<XriLooseProp>();
        var shelf=GameObject.Find("DustPackingSideShelf").GetComponent<Collider>().bounds;
        Assert.AreEqual(shelf.max.y,roll.parking.y-.08f,.002f);
        var dust=Object.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None).Single(i=>i.ItemId==ItemIds.CrystalDust).GetComponent<ClearingGate>();
        Assert.AreSame(roll,dust.books);Assert.NotNull(dust.blocker);
        foreach(var reveal in Object.FindObjectsByType<ClearingReveal>(FindObjectsSortMode.None))
        {Assert.NotNull(reveal.gate);Assert.NotNull(reveal.source);Assert.Greater(reveal.source.intensity,0);}
        Assert.Less(Vector3.Distance(new Vector3(3.75f,.78f,-2.94f),Object.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None).Single(i=>i.ItemId==ItemIds.Moonpetal).transform.position),.001f);
    }
    [Test] public void ProposalDoesNotChangeAnswersOrIds()
    {
        CollectionAssert.AreEqual(new[] { RuneId.Earth, RuneId.Water, RuneId.Fire, RuneId.Light }, RunePuzzleLogic.DefaultAnswer);
        CollectionAssert.AreEqual(new[] { "ing_moonpetal", "ing_salamander_scale", "ing_crystal_dust" }, PotionPuzzleLogic.DefaultRecipe);
        Assert.AreEqual("crystal_sapphire", SealPuzzleLogic.ExpectedCrystal(SocketSide.Left));
        Assert.AreEqual("crystal_ruby", SealPuzzleLogic.ExpectedCrystal(SocketSide.Right));
    }
    [Test] public void ProgressionStillRequiresAllThreePuzzles()
    {
        var flow = new ProgressionModel(); flow.StartGame(); Assert.False(flow.MarkSolved(PuzzleId.Seal));
        Assert.True(flow.MarkSolved(PuzzleId.Rune)); Assert.True(flow.MarkSolved(PuzzleId.Potion)); Assert.False(flow.ExitOpen);
        Assert.True(flow.MarkSolved(PuzzleId.Seal)); Assert.AreEqual(3, flow.SolvedCount); Assert.True(flow.ExitOpen);
    }
    [Test] public void InstalledVersionsAreTheUnity63ReleasedPackages()
    {
        Assert.AreEqual("3.3.2", PackageInfo.FindForAssembly(typeof(XRGrabInteractable).Assembly).version);
        Assert.AreEqual("1.16.1", PackageInfo.FindForAssembly(typeof(OpenXRLoader).Assembly).version);
    }
    [Test] public void WarehouseIsStartupAndClassroomIsRegressionFixture()
    {
        Assert.AreEqual("Assets/Scenes/Warehouse.unity", EditorBuildSettings.scenes[0].path);
        Assert.True(EditorBuildSettings.scenes[0].enabled);
        Assert.True(System.Array.Exists(EditorBuildSettings.scenes, s => s.path == "Assets/Scenes/Classroom.unity" && s.enabled));
    }
    static Bounds VisibleBounds(Transform t)
    {
        var renderers = t.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
        Assert.IsNotEmpty(renderers, t.name);
        foreach (var text in t.GetComponentsInChildren<TMPro.TMP_Text>()) text.ForceMeshUpdate();
        var b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);
        return b;
    }
    [Test] public void PictureAssembliesAreSupportedAndLabelsAndLightsStayAttached()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        float wallHeight = -1;
        foreach (string roman in new[] { "III", "I", "IV", "II" })
        {
            var painting = GameObject.Find("Painting_"+roman).transform;
            var frame = painting.Find("ArtB_PictureFrame_"+roman);
            var bounds = VisibleBounds(frame);
            Assert.That(bounds.min.x, Is.EqualTo(-4.425f).Within(.065f), roman+" must touch west wall");
            Assert.Less(bounds.max.y, 2.5f, roman+" must clear ceiling beams: "+string.Join(";",frame.GetComponentsInChildren<Renderer>().Select(r=>r.name+":"+r.bounds.ToString())));
            if (roman == "I" || roman == "IV")
            {
                Assert.That(bounds.min.y, Is.EqualTo(.015f).Within(.02f), roman+" must touch floor");
                Assert.That(Quaternion.Angle(painting.rotation, Quaternion.identity), Is.EqualTo(12).Within(.01f));
            }
            else
            {
                if (wallHeight < 0) wallHeight = painting.position.y;
                Assert.AreEqual(wallHeight, painting.position.y, .001f, "Wall frames share height");
                Assert.Less(Quaternion.Angle(painting.rotation, Quaternion.identity), .01f);
            }
            var plaque = frame.Find("ArtB_RomanPlaque");
            var label = GameObject.Find("PaintingLabel_"+roman).transform;
            Assert.AreEqual(frame, label.parent, "No detached numeral labels");
            Assert.That(label.localPosition.x,Is.EqualTo(-.34f).Within(.001f));
            Assert.That(label.localPosition.y,Is.EqualTo(plaque.localPosition.y).Within(.001f));
            Assert.IsTrue(GameObject.Find("ArtC_AnswerFrame_"+roman).transform.IsChildOf(frame));
            var lamp = frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("ArtB_C_PictureLamp_"));
            Assert.IsTrue(frame.Find("LampMount").GetComponent<Renderer>().bounds.Intersects(lamp.GetComponent<Renderer>().bounds), "Lamp must touch its mount");
            Assert.IsTrue(lamp.GetComponentInChildren<Transform>().GetComponentsInChildren<Renderer>().Length >= 2);
        }
    }
    [Test] public void StorageCratesHavePhysicalSupportAndAttachedDetail()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        var crates = UnityEngine.Object.FindObjectsByType<HandlingProfile>(FindObjectsSortMode.None)
            .Where(p=>p.name.StartsWith("StackedCrate_") || p.name.StartsWith("SouthStorage_") || p.name=="RewardStand")
            .Select(p=>p.GetComponent<BoxCollider>()).ToArray();
        Assert.AreEqual(13, crates.Length);
        foreach (var crate in crates)
        {
            var b=crate.bounds;
            Assert.IsTrue(Mathf.Abs(b.min.y)<.02f || crates.Any(other=>other!=crate &&
                Mathf.Abs(other.bounds.max.y-b.min.y)<.02f &&
                other.bounds.min.x <= b.min.x+.01f && other.bounds.max.x >= b.max.x-.01f &&
                other.bounds.min.z <= b.min.z+.01f && other.bounds.max.z >= b.max.z-.01f), crate.name+" floats without floor/stack support");
            var detail=crate.GetComponentsInChildren<Renderer>().Where(r=>r.transform!=crate.transform).ToArray();
            Assert.Greater(detail.Length,25,"Plank seams, rails, bindings and studs required");
            var support=b; support.Expand(.075f);
            foreach(var r in detail) Assert.IsTrue(support.Intersects(r.bounds),r.name+" detached from crate");
        }
    }
    [Test] public void ClearingPropsHaveSupportedHomesAndDynamicAttachedVisuals()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        foreach(var moving in UnityEngine.Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None))
        {
            Assert.AreEqual(1,moving.GetComponents<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>().Length);
            foreach(var t in moving.GetComponentsInChildren<Transform>())Assert.False(t.gameObject.isStatic,t.name+" must not remain in static render batch");
            if(moving.kind==ClearingKind.Slide)
            {
                var b=moving.GetComponent<Collider>().bounds;Assert.That(b.min.y,Is.EqualTo(0).Within(.01f));
                var attached=b;attached.Expand(.08f);
                foreach(var r in moving.GetComponentsInChildren<Renderer>())Assert.True(attached.Intersects(r.bounds),r.name+" detached");
            }
        }
        var books=GameObject.Find("MovableBookStack").GetComponent<XriLooseProp>();
        var source=GameObject.Find("SouthStorage_0").GetComponent<Collider>().bounds;
        var destination=GameObject.Find("SouthStorage_1").GetComponent<Collider>().bounds;
        Assert.That(books.GetComponent<Collider>().bounds.min.y,Is.EqualTo(source.max.y).Within(.01f));
        Assert.That(books.parking.y-.13f,Is.EqualTo(destination.max.y).Within(.01f));
        Assert.IsNull(GameObject.Find("ElementTable"));Assert.IsNull(GameObject.Find("BookStackClue"));
        Assert.NotNull(GameObject.Find("BookbindingBoard"));
        var cloth=GameObject.Find("PaintingIIICloth");
        var frame=GameObject.Find("ArtB_PictureFrame_III");
        Assert.True(cloth.GetComponentInChildren<Renderer>().bounds.Intersects(VisibleBounds(frame.transform)),"Cover touches frame");
        Assert.AreSame(cloth.GetComponent<XriClearable>(),GameObject.Find("Painting_III").GetComponent<ClearingGate>().blocker);
        var lid=GameObject.Find("ChestLidHinge");var back=GameObject.Find("ChestBack").GetComponent<Collider>().bounds;back.Expand(.08f);
        Assert.True(back.Intersects(lid.GetComponentInChildren<Renderer>().bounds),"Lid physically touches back hinge edge");
    }
    [Test] public void ChalkboardContainsScenesWithoutElementNamesOrPictograms()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        const string allowed="Assets/Art/Warehouse/ChalkScenes.png";
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(allowed);
        Assert.NotNull(texture);
        using(var sha=System.Security.Cryptography.SHA256.Create())
            Assert.AreEqual("685387D8997F950289F0D53E14B37E1652565E25199A09707CDFF6178FE2875D",
                System.BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(allowed))).Replace("-",""),"Reviewed scene-only chalk pixels must not gain element names or pictograms");
        var board=GameObject.Find("Chalkboard");
        Assert.AreSame(texture,new SerializedObject(board.GetComponent<InspectableObject>()).FindProperty("picture").objectReferenceValue);
        Assert.AreEqual("I   II   III   IV",new SerializedObject(board.GetComponent<InspectableObject>()).FindProperty("body").stringValue);
        var oldLabel=Object.FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(t=>t.name=="ChalkboardLabel");
        Assert.AreEqual("I   II   III   IV",oldLabel.text);
        Assert.False(oldLabel.gameObject.activeSelf);
        var plate=GameObject.Find("ChalkSequence").GetComponent<Renderer>();
        Assert.AreSame(texture,plate.sharedMaterial.mainTexture,"World plate must actually have its approved texture");
        Assert.AreSame(texture,plate.sharedMaterial.GetTexture("_EmissionMap"));
        foreach(var mat in plate.sharedMaterials)
            foreach(var property in mat.GetTexturePropertyNames())
            {
                var tex=mat.GetTexture(property);if(tex!=null)Assert.AreSame(texture,tex,property);
            }
        Assert.IsEmpty(board.GetComponentsInChildren<RuneStone>(true));
        Assert.That(board.transform.position.y,Is.EqualTo(1.87f).Within(.001f));
        foreach(string side in new[]{"1","-1"})
            foreach(string trim in new[]{"ArtB_BoardRail_","ArtB_BoardEdge_"})
                Assert.AreSame(board.transform,GameObject.Find(trim+side).transform.parent,"No detached chalkboard border");
        foreach(var t in board.GetComponentsInChildren<Transform>(true))
            Assert.False(t.name.Contains("RuneName")||t.name.Contains("CornerRune")||t.name.Contains("Story_"));
        foreach(var inspect in Object.FindObjectsByType<InspectableObject>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            var pic=new SerializedObject(inspect).FindProperty("picture").objectReferenceValue;
            if(inspect.name.StartsWith("Painting_"))Assert.That(pic.name,Does.StartWith("Story_"));
            else if(inspect.name=="Chalkboard")Assert.AreSame(texture,pic);
            else if(inspect.name=="RecipeBook")Assert.AreEqual("PictureRecipe",pic.name);
            else Assert.IsNull(pic,"Unexpected alternate inspection picture: "+inspect.name);
        }
    }
    [Test] public void GeneratedStoriesShareWorldAndInspectionTextures()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        string[] romans={"I","II","III","IV"};
        for(int i=0;i<4;i++)
        {
            string path="Assets/Art/Warehouse/Textures/Story_"+(i+1)+".png";
            var picture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Assert.NotNull(picture);
            Assert.LessOrEqual(picture.width,1024);Assert.LessOrEqual(picture.height,1024);
            var frame=GameObject.Find("ArtB_PictureFrame_"+romans[i]).transform.Find("ArtB_StoryIllustration").GetComponent<Renderer>();
            Assert.AreSame(picture,frame.sharedMaterial.mainTexture);
            Assert.AreSame(picture,new SerializedObject(GameObject.Find("Painting_"+romans[i]).GetComponent<InspectableObject>()).FindProperty("picture").objectReferenceValue);
            var caption=GameObject.Find("WarehouseSceneOrder_"+i).GetComponent<TMPro.TMP_Text>();
            Assert.AreEqual(-.375f+i*.25f,caption.rectTransform.anchoredPosition.x,.001f);
            Assert.AreEqual(.34f,caption.rectTransform.anchoredPosition.y,.001f);
        }
    }
    [Test] public void FrameNameplatesDoNotCoverIllustrations()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        foreach(string roman in new[]{"I","II","III","IV"})
        {
            var frame=GameObject.Find("ArtB_PictureFrame_"+roman).transform;
            if(roman=="III")
            {
                var tex=frame.Find("ArtB_StoryIllustration").GetComponent<Renderer>().sharedMaterial.mainTexture;
                Assert.AreEqual("Assets/Art/Warehouse/Textures/Story_3.png",AssetDatabase.GetAssetPath(tex),"No baked corner glyph in Warehouse III art");
                Assert.AreSame(tex,new SerializedObject(GameObject.Find("Painting_III").GetComponent<InspectableObject>()).FindProperty("picture").objectReferenceValue);
            }
            var glyph=frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("ArtB_CornerRune_"));
            var name=frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("RuneName_"));
            foreach(var t in new[]{glyph,name})
            {
                Assert.Less(t.localPosition.y+t.localScale.y/2,-.41f,"Must remain below the .82m illustration");
                Assert.That(t.localPosition.y,Is.EqualTo(-.51f).Within(.001f));
                Assert.AreEqual("MagicEscape/WarehouseIncision",t.GetComponent<Renderer>().sharedMaterial.shader.name);
            }
        }
    }

    [Test] public void WarehouseUsesPictogramsWithoutAlchemicalResourcesOrFrameNumerals()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        foreach(string roman in new[]{"I","II","III","IV"})
        {
            var label=GameObject.Find("PaintingLabel_"+roman).GetComponent<TMPro.TMP_Text>();
            Assert.IsEmpty(label.text,"No visible numeral on a frame");
            var frame=GameObject.Find("ArtB_PictureFrame_"+roman);
            foreach(var text in frame.GetComponentsInChildren<TMPro.TMP_Text>(true))Assert.IsEmpty(text.text);
        }
        foreach(var renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            foreach(var mat in renderer.sharedMaterials)
                if(mat!=null)foreach(var property in mat.GetTexturePropertyNames())
                {
                    string path=AssetDatabase.GetAssetPath(mat.GetTexture(property));
                    foreach(RuneId id in System.Enum.GetValues(typeof(RuneId)))
                        Assert.AreNotEqual("Assets/Art/S10b/"+id+".png",path,renderer.name+" alchemical mask reference");
                }
        var dependencies=AssetDatabase.GetDependencies("Assets/Scenes/Warehouse.unity",true);
        foreach(RuneId id in System.Enum.GetValues(typeof(RuneId)))
            CollectionAssert.DoesNotContain(dependencies,"Assets/Art/S10b/"+id+".png","No indirect alchemical dependency");
        foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
            Assert.AreEqual("Assets/Art/Warehouse/Pictogram_"+stone.Rune+".png",
                AssetDatabase.GetAssetPath(stone.transform.Find("ArtB_RuneIcon_"+stone.Rune).GetComponent<Renderer>().sharedMaterial.GetTexture("_BaseMap")));
    }
    static bool Touching(Bounds a,Bounds b)
    {
        a.Expand(.004f); // 2mm contact tolerance for float-transformed primitive surfaces.
        return a.Intersects(b);
    }
    [Test] public void ScaleWorkbenchAndDressingHaveSupportedNoninteractiveHomes()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        var root=GameObject.Find("WarehouseIngredientWorkbench").transform;
        var top=GameObject.Find("WarehouseIngredientShelf").GetComponent<Renderer>().bounds;
        Assert.AreEqual(1.20f,top.max.y,.002f);
        var legs=root.GetComponentsInChildren<Renderer>().Where(r=>r.name=="WarehouseShelfLeg").ToArray();
        Assert.AreEqual(4,legs.Length);
        foreach(var leg in legs){Assert.AreEqual(0,leg.bounds.min.y,.002f);Assert.AreEqual(top.min.y,leg.bounds.max.y,.002f);}
        Assert.AreEqual(3,root.GetComponentsInChildren<Renderer>().Count(r=>r.name=="ArtB_WorkbenchPlankJoint"));
        foreach(var r in root.GetComponentsInChildren<Renderer>())
        {
            if(r.name=="ArtB_WorkbenchDecorJar"||r.name=="ArtB_WorkbenchMortar")Assert.AreEqual(top.max.y,r.bounds.min.y,.002f,r.name+" floats");
            if(r.name.StartsWith("ArtB_Workbench"))
                Assert.True(root.GetComponentsInChildren<Renderer>().Any(other=>other!=r&&Touching(other.bounds,r.bounds)),r.name+" detached from furniture/dressing");
            Assert.IsNull(r.GetComponentInParent<ItemPickup>());
            Assert.IsNull(r.GetComponentInParent<InspectableObject>());
            Assert.IsNull(r.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>());
        }
        var item=Object.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None).Single(i=>i.ItemId==ItemIds.SalamanderScale);
        Assert.Less(Vector3.Distance(item.transform.position,new Vector3(3.65f,1.30f,.05f)),.001f);
        var frame=GameObject.Find("ArtB_PictureFrame_I").transform.Find("ArtB_StoryIllustration").GetComponent<Renderer>();
        var pic=new SerializedObject(GameObject.Find("Painting_I").GetComponent<InspectableObject>()).FindProperty("picture").objectReferenceValue;
        Assert.AreSame(frame.sharedMaterial.mainTexture,pic);
        Assert.AreEqual("Assets/Art/Warehouse/Textures/Story_1.png",AssetDatabase.GetAssetPath(pic));
        var water=Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None).Single(r=>r.Rune==RuneId.Water);
        var waterIcon=water.transform.Find("ArtB_RuneIcon_Water").GetComponent<Renderer>();
        var plaque=GameObject.Find("ArtB_PictureFrame_II").GetComponentsInChildren<Renderer>().Single(r=>r.name.StartsWith("ArtB_CornerRune_"));
        Assert.AreSame(waterIcon.sharedMaterial,plaque.sharedMaterial,"One shared drop design");
    }
    [Test] public void AllItemHomesHaveReachableHeightAndPersistentLocalLighting()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        var items=Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        Assert.AreEqual(7,items.Length);
        var dark=Object.FindFirstObjectByType<WarehouseDarkening>();
        foreach(var item in items)
        {
            // User explicitly preserves the .78m moonflower. It is the sole height exception.
            if(item.ItemId==ItemIds.Moonpetal)Assert.AreEqual(.78f,item.transform.position.y,.001f);
            else Assert.That(item.transform.position.y,Is.InRange(.8f,1.6f),item.ItemId);
            var beacon=Object.FindObjectsByType<WarehouseItemBeacon>(FindObjectsSortMode.None).Single(b=>b.pickup==item);
            Assert.Less(Vector3.Distance(item.transform.position,beacon.source.transform.position),.5f,item.ItemId);
            CollectionAssert.Contains(dark.localLights,beacon.source,"Darkness must not attenuate item guide");
        }
    }
}
