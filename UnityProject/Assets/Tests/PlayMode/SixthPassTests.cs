using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object=UnityEngine.Object;
public class SixthPassTests
{
 [UnityTest] public IEnumerator IncisionsAndMaterialEffectsStayPresentationOnly()
 {
  yield return SceneManager.LoadSceneAsync("Classroom");yield return null;GameManager.Instance.StartGame();GameManager.Instance.SkipPresentation();yield return null;
  var stones=Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None);Assert.AreEqual(6,stones.Length);
  foreach(var stone in stones)
  {
   Assert.NotNull(stone.transform.Find("AlchemicalSlab"));Assert.IsNull(stone.transform.Find("E_Element_"+stone.Rune));
   var pulse=stone.GetComponent<AlchemicalPulse>();Assert.NotNull(pulse);Assert.False(pulse.effect.activeSelf);
   Assert.AreEqual("MagicEscape/AlchemyEngraving",pulse.engraving.sharedMaterial.shader.name);
   Assert.That(stone.GetComponent<Collider>().bounds.size.x,Is.LessThan(.4f));
  }
  var earth=stones.First(s=>s.Rune==RuneId.Earth);earth.Interact(Scenario.Player);
  yield return new WaitForSecondsRealtime(.3f);Assert.True(earth.GetComponent<AlchemicalPulse>().effect.activeSelf);
  yield return new WaitForSecondsRealtime(1);Assert.False(earth.GetComponent<AlchemicalPulse>().effect.activeSelf);
  Assert.AreEqual(1,Object.FindFirstObjectByType<RunePuzzle>().Logic.ProgressCount);
  Assert.AreEqual("MagicEscape/ElementWater",GameObject.Find("Liquid").GetComponent<Renderer>().sharedMaterial.shader.name);
  Assert.NotNull(GameObject.Find("Liquid").GetComponent<Renderer>().sharedMaterial.GetTexture("_NormalMap"));
  Assert.False(Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Any(t=>t.name=="TongueOfFire"));
  foreach(var lamp in Object.FindFirstObjectByType<RoomPresentation>().sealLamps)
  {
   Assert.False(lamp.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.EndsWith("SealLampLens")).enabled);
   Assert.NotNull(lamp.transform.Find("SealLampEngraving"));
  }
  // Unity TestRunner automatically fails unexpected Error/Exception/Assert logs.
 }
}
