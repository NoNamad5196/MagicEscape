using System.Collections;
using MagicEscape;
using MagicEscape.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
public class ItemsTests
{
    [UnityTest] public IEnumerator S4_CatalogHotbarSelection()
    {
        yield return SceneManager.LoadSceneAsync("Classroom"); yield return null;
        GameManager.Instance.StartGame(); yield return null;
        var inv=InventoryManager.Instance;
        string[] ids={ItemIds.Moonpetal,ItemIds.SalamanderScale,ItemIds.CrystalDust,ItemIds.Toadstool,ItemIds.CrystalSapphire,ItemIds.CrystalRuby,ItemIds.PotionRevelation};
        foreach(var id in ids) { Assert.AreEqual(id,inv.GetData(id).itemId); Assert.NotNull(inv.GetData(id).icon); Assert.True(inv.TryAdd(id)); }
        for(int i=0;i<7;i++) { inv.Select(i); Assert.AreEqual(ids[i],inv.SelectedItemId); Assert.True(GameObject.Find("Slot_"+(i+1)).transform.Find("Frame").gameObject.activeSelf); Assert.NotNull(GameObject.Find("Slot_"+(i+1)).transform.Find("Icon").GetComponent<Image>().sprite); }
        inv.SelectNext(1); Assert.IsNull(inv.SelectedItemId); inv.SelectNext(1); Assert.AreEqual(ids[0],inv.SelectedItemId);
        inv.Remove(ids[0]); Assert.AreEqual(6,inv.Model.Count);
    }
}
