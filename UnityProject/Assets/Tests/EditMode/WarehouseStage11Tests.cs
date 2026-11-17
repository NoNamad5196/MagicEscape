using System.Linq;
using MagicEscape;
using MagicEscape.Core;
using System.IO;
using NUnit.Framework;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
public class WarehouseStage11Tests
{
    [Test] public void EveryAuthoredAndDynamicKoreanUiCharacterIsBakedWithoutFallback()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        string corpus=string.Concat(new[]{"Assets/Scripts","Assets/Editor"}.SelectMany(p=>Directory.GetFiles(p,"*.cs",SearchOption.AllDirectories)).Select(File.ReadAllText));
        Assert.That(corpus,Does.Contain("\ub9ad"));
        string constants=string.Concat(typeof(GameText).GetFields(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static).Where(f=>f.FieldType==typeof(string)).Select(f=>(string)f.GetValue(null)));
        string korean=new string(corpus.Where(c=>((c>=0xac00&&c<=0xd7a3)||(c>=0x3130&&c<=0x318f)||(c>=0x1100&&c<=0x11ff))).ToArray());
        foreach(var text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        {
            string required=korean+constants+text.text;
            string absent=new string(required.Where(c=>!char.IsControl(c)&&!text.font.HasCharacter(c,false,false)).Distinct().ToArray());
            Assert.IsEmpty(absent,text.name+" missing UI glyphs without runtime additions/fallback");
            Assert.AreEqual(AtlasPopulationMode.Static,text.font.atlasPopulationMode);
        }
    }
    [Test] public void BarrelHasAnOpenInteriorAndBooksHaveCoversSpinesAndPages()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Warehouse.unity");
        var barrel=GameObject.Find("WarehouseDampBarrel");
        Assert.IsNull(barrel.GetComponent<Renderer>(),"No solid cylinder cap");
        Assert.AreEqual(32,barrel.GetComponentsInChildren<Collider>().Length);
        var mushroom=Object.FindObjectsByType<ItemPickup>(FindObjectsSortMode.None).Single(i=>i.ItemId==ItemIds.Toadstool);
        Assert.AreEqual(1.10f,mushroom.transform.position.y,.001f);
        Assert.Less(mushroom.transform.position.y,1.18f);
        var lid=GameObject.Find("DampBarrelCover");
        Assert.AreEqual(lid.transform.localScale.x,lid.transform.localScale.z,.001f);
        Assert.AreEqual(.70f,lid.transform.localScale.x,.001f);
        Assert.AreEqual(-110,GameObject.Find("ToadstoolBarrelLid").GetComponent<XriClearable>().openEuler.z);
        var stack=GameObject.Find("ScaleBookStack").transform;
        Assert.AreEqual(4,stack.childCount);
        foreach(Transform book in stack)
        {
            Assert.NotNull(book.Find("Pages"));Assert.NotNull(book.Find("Spine"));
            Assert.AreEqual(2,book.Cast<Transform>().Count(t=>t.name=="Cover"));
        }
        Assert.Greater(Vector3.Distance(stack.GetComponent<XriLooseProp>().parking,mushroom.transform.position),1);
    }
}
