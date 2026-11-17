using System.Collections;
using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object=UnityEngine.Object;
public class WarehouseStage12VisualTests
{
 [UnityTest] public IEnumerator ReviewAllSurfacesAndProgression()
 {
  yield return SceneManager.LoadSceneAsync("Warehouse");yield return null;
  Object.FindFirstObjectByType<RigSelector>().Apply(false);
  GameManager.Instance.StartGame();GameManager.Instance.SkipPresentation();yield return null;yield return null;
  string prefix=System.Environment.GetEnvironmentVariable("MAGICESCAPE_STAGE12_BEFORE")=="1"?"stage12-before-":"stage12-after-";
  ArtTests.View(new Vector3(0,0,-2.3f),new Vector3(.1f,1.5f,3.2f));yield return WarehouseDarkeningTests.Capture(prefix+"room");
  if(prefix.Contains("after"))
  {
   var moon=GameObject.Find("WarehouseWindowMoonlight").GetComponent<Light>();
   var bounce=GameObject.Find("WarehouseMoonBounce").GetComponent<Light>();
   Time.timeScale=0;moon.enabled=false;bounce.enabled=false;
   ArtTests.View(new Vector3(0,0,-2.3f),new Vector3(.1f,1.5f,3.2f));
   yield return WarehouseDarkeningTests.Capture(prefix+"room-moon-off");moon.enabled=true;bounce.enabled=true;
   ArtTests.View(new Vector3(0,0,-2.3f),new Vector3(.1f,1.5f,3.2f));
   yield return WarehouseDarkeningTests.Capture(prefix+"room");Time.timeScale=1;
   var diagnosticControl=Scenario.Player.GetComponent<PlayerController>();diagnosticControl.enabled=false;
   ArtTests.View(new Vector3(1.75f,0,-1.9f),new Vector3(3.8f,1.5f,-3.05f));
   yield return WarehouseDarkeningTests.Capture(prefix+"window");
   ArtTests.View(new Vector3(2.3f,0,-.65f),new Vector3(3.8f,1.25f,.1f));
   yield return WarehouseDarkeningTests.Capture(prefix+"workbench");
   ArtTests.View(new Vector3(-1.9f,0,-1.8f),new Vector3(-3.25f,.6f,-2.3f));
   yield return WarehouseDarkeningTests.Capture(prefix+"chest-support");diagnosticControl.enabled=true;
  }
  yield return Stage10Actions.ClearNew();
  var ray=Object.FindFirstObjectByType<DesktopXriDriver>().ray;
  foreach(var clear in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None))
  {
   if(clear.kind==ClearingKind.Slide)clear.MoveWithinRails(clear.name.EndsWith("IV")?clear.minPosition:clear.maxPosition);
   else if(!clear.IsCleared){clear.interactionManager.SelectEnter((IXRSelectInteractor)ray,clear);clear.interactionManager.SelectExit((IXRSelectInteractor)ray,clear);}
  }
  yield return new WaitForSeconds(1.5f);
  foreach(string roman in new[]{"I","II","III","IV"})
  {
   var frame=GameObject.Find("Painting_"+roman).transform;
   ArtTests.View(new Vector3(-2.9f,0,roman=="III"?-.85f:frame.position.z),frame.position);
   yield return WarehouseDarkeningTests.Capture(prefix+"frame-"+roman);
  }
  var control=Scenario.Player.GetComponent<PlayerController>();control.enabled=false;
  float fov=Camera.main.fieldOfView;Camera.main.fieldOfView=38;
  ArtTests.View(new Vector3(-2.4f,0,-3.05f),GameObject.Find("Chalkboard").transform.position);
  yield return WarehouseDarkeningTests.Capture(prefix+"chalk");Camera.main.fieldOfView=fov;control.enabled=true;
  GameObject.Find("Chalkboard").GetComponent<InspectableObject>().Interact(Scenario.Player);
  yield return WarehouseDarkeningTests.Capture(prefix+"chalk-inspect");UIManager.Instance.CloseRead();
  var altar=Object.FindFirstObjectByType<RunePuzzle>().transform;
  ArtTests.View(new Vector3(altar.position.x,0,altar.position.z-1.5f),altar.position);yield return WarehouseDarkeningTests.Capture(prefix+"altar");
  yield return Scenario.Rune();Scenario.Collect(ItemIds.CrystalSapphire);yield return new WaitForSeconds(3.2f);
  var potion=Object.FindFirstObjectByType<PotionPuzzle>();
  Scenario.Select(ItemIds.CrystalSapphire);
  if(prefix.Contains("after"))
  {
   foreach(string id in new[]{ItemIds.CrystalSapphire,ItemIds.CrystalRuby,ItemIds.PotionRevelation})
   {
    InventoryManager.Instance.TryAdd(id);Scenario.Select(id);
    Assert.That(potion.GetPrompt(Scenario.Player),Does.Not.Contain("넣기"));
    int fails=potion.Logic.FailCount;potion.Interact(Scenario.Player);
    Assert.True(InventoryManager.Instance.Contains(id));Assert.AreEqual(fails,potion.Logic.FailCount);
    if(id!=ItemIds.CrystalSapphire)InventoryManager.Instance.Remove(id);
   }
   foreach(string id in PotionPuzzleLogic.DefaultIngredients)
   {
    InventoryManager.Instance.TryAdd(id);Scenario.Select(id);
    Assert.That(potion.GetPrompt(Scenario.Player),Does.Contain("넣기"));InventoryManager.Instance.Remove(id);
   }
   Scenario.Select(ItemIds.CrystalSapphire);
  }
  yield return new WaitForSeconds(3.2f); // expire invalid-item feedback before clean evidence captures
  ArtTests.View(new Vector3(2.6f,0,-.65f),GameObject.Find("Liquid").transform.position);
  yield return WarehouseDarkeningTests.Capture(prefix+"cauldron");
  if(prefix.Contains("after")){yield return new WaitForSeconds(.9f);yield return WarehouseDarkeningTests.Capture(prefix+"cauldron-motion");}
  GameObject.Find("RecipeBook").GetComponent<InspectableObject>().Interact(Scenario.Player);
  yield return WarehouseDarkeningTests.Capture(prefix+"recipe");UIManager.Instance.CloseRead();
  ArtTests.View(new Vector3(2.1f,0,1f),new Vector3(2.1f,1.3f,2.65f));yield return WarehouseDarkeningTests.Capture(prefix+"seal-hidden");
  yield return Scenario.Potion();yield return new WaitForSeconds(3.2f);
  ArtTests.View(new Vector3(2.6f,0,-.65f),GameObject.Find("Liquid").transform.position);yield return WarehouseDarkeningTests.Capture(prefix+"cauldron-solved");
  Scenario.Select(ItemIds.PotionRevelation);Object.FindFirstObjectByType<SealInscription>().Interact(Scenario.Player);
  yield return new WaitForSeconds(1.0f);
  ArtTests.View(new Vector3(2.1f,0,1f),new Vector3(2.1f,1.3f,2.65f));yield return WarehouseDarkeningTests.Capture(prefix+"seal-revealed");
  foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
  {Scenario.Select(socket.Side==SocketSide.Left?ItemIds.CrystalSapphire:ItemIds.CrystalRuby);socket.Interact(Scenario.Player);}
  Object.FindFirstObjectByType<SealCore>().Interact(Scenario.Player);yield return new WaitForSeconds(4.2f);
  ArtTests.View(new Vector3(0,0,-2.3f),new Vector3(.1f,1.5f,3.2f));yield return WarehouseDarkeningTests.Capture(prefix+"room-darkest");
  Assert.AreEqual(3,Object.FindFirstObjectByType<WarehouseDarkening>().Stage);
 }
}
