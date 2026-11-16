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
    [Test] public void ChalkboardResourcesContainOnlyApprovedEmptyDiagram()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        const string allowed="Assets/Art/Warehouse/ChalkBlanks.png";
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(allowed);
        Assert.NotNull(texture);
        using(var sha=System.Security.Cryptography.SHA256.Create())
            Assert.AreEqual("A09AE7A9877824737B0D78F1009084A499D1A783981B70226B4F46986C3D13BC",
                System.BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(allowed))).Replace("-",""),"Reviewed numeral + empty-circle pixels must not silently gain answer art");
        var board=GameObject.Find("Chalkboard");
        Assert.AreSame(texture,new SerializedObject(board.GetComponent<InspectableObject>()).FindProperty("picture").objectReferenceValue);
        Assert.AreEqual("I \u25cb   II \u25cb   III \u25cb   IV \u25cb",new SerializedObject(board.GetComponent<InspectableObject>()).FindProperty("body").stringValue);
        var oldLabel=Object.FindObjectsByType<TMPro.TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(t=>t.name=="ChalkboardLabel");
        Assert.AreEqual("I \u25cb   II \u25cb   III \u25cb   IV \u25cb",oldLabel.text);
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
    [Test] public void FrameNameplatesDoNotCoverIllustrations()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        foreach(string roman in new[]{"I","II","III","IV"})
        {
            var frame=GameObject.Find("ArtB_PictureFrame_"+roman).transform;
            if(roman=="III")
            {
                var tex=frame.Find("ArtB_StoryIllustration").GetComponent<Renderer>().sharedMaterial.mainTexture;
                Assert.AreEqual("Assets/Art/Warehouse/Story_3.png",AssetDatabase.GetAssetPath(tex),"No baked corner glyph in Warehouse III art");
                Assert.AreSame(tex,new SerializedObject(GameObject.Find("Painting_III").GetComponent<InspectableObject>()).FindProperty("picture").objectReferenceValue);
            }
            var glyph=frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("ArtB_CornerRune_"));
            var name=frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("RuneName_"));
            foreach(var t in new[]{glyph,name})
            {
                Assert.Less(t.localPosition.y+t.localScale.y/2,-.41f,"Must remain below the .82m illustration");
                Assert.That(t.localPosition.y,Is.EqualTo(-.51f).Within(.001f));
                Assert.AreEqual("MagicEscape/AlchemyEngraving",t.GetComponent<Renderer>().sharedMaterial.shader.name);
            }
        }
    }
}
