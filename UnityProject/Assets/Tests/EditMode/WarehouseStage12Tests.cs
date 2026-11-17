using System.IO;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object=UnityEngine.Object;
public class WarehouseStage12Tests
{
 [Test] public void CarvedSurfacesHaveNoBlackPanelsAndBevelFacesPointOutward()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
  Assert.AreEqual("달",RuneStone.KoreanName(RuneId.Shadow));
  foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
  {
   var slab=stone.transform.Find("AlchemicalSlab");var mesh=slab.GetComponent<MeshFilter>().sharedMesh;
   Assert.AreEqual("Assets/Art/Warehouse/WornSlab.asset",AssetDatabase.GetAssetPath(mesh));
   var v=mesh.vertices;var normals=mesh.normals;
   for(int i=0;i<v.Length;i++) Assert.GreaterOrEqual(Vector3.Dot(v[i],normals[i]),-.001f,"Inverted bevel face "+i);
   foreach(string name in new[]{"ArtB_RuneIcon_"+stone.Rune,"RuneName_"+stone.Rune})
   {
    var mark=stone.transform.Find(name).GetComponent<Renderer>();
    Assert.AreEqual("MagicEscape/WarehouseIncision",mark.sharedMaterial.shader.name);
    Assert.AreEqual(slab.position.y+.0288f,mark.transform.position.y,.0001f);
    Assert.That(AssetDatabase.GetAssetPath(mark.sharedMaterial.mainTexture),Does.StartWith("Assets/Art/Warehouse/"));
   }
  }
  Assert.AreEqual("MagicEscape/WarehouseChalk",GameObject.Find("ChalkSequence").GetComponent<Renderer>().sharedMaterial.shader.name);
  Assert.AreEqual("MagicEscape/WarehouseBrew",GameObject.Find("Liquid").GetComponent<Renderer>().sharedMaterial.shader.name);
  var moon=GameObject.Find("WarehouseWindowMoonlight").GetComponent<Light>();
  var dark=Object.FindFirstObjectByType<WarehouseDarkening>();
  CollectionAssert.Contains(dark.backgroundLights,GameObject.Find("WarehouseMoonBounce").GetComponent<Light>());
  CollectionAssert.Contains(dark.backgroundLights,moon);CollectionAssert.DoesNotContain(dark.localLights,moon);
  Assert.Greater(moon.color.b,moon.color.r);Assert.AreEqual(13,dark.localLights.Length);
 }
 [Test] public void SuccessNarrationIsNotEmittedAndPromptUsesCoreIngredientMembership()
 {
  foreach(string name in new[]{"RunePuzzle","PotionPuzzle","SealPuzzle"})
  {
   string source=File.ReadAllText("Assets/Scripts/Runtime/Puzzles/"+name+".cs");
   Assert.That(source,Does.Not.Contain("Say(GameText.RuneSolved)").And.Not.Contain("Say(GameText.PotionSolved)").And.Not.Contain("Say(GameText.SealReleased)"));
   if(name=="PotionPuzzle")
   {Assert.That(source,Does.Contain("Logic.IsIngredient(selected)"));Assert.That(source,Does.Not.Contain("Say(KoreanParticle.Object"));}
   Assert.True(File.ReadAllBytes("Assets/Scripts/Runtime/Puzzles/"+name+".cs").SequenceEqual(File.ReadAllBytes("../CodeDrafts/Runtime/Puzzles/"+name+".cs")),name+" mirror");
  }
 }
}
