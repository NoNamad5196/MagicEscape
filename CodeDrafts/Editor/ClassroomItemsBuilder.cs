using System.IO;
using MagicEscape.Core;
using UnityEditor;
using UnityEngine;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static readonly string[] ItemKeys={ItemIds.Moonpetal,ItemIds.SalamanderScale,ItemIds.CrystalDust,ItemIds.Toadstool,ItemIds.CrystalSapphire,ItemIds.CrystalRuby,ItemIds.PotionRevelation};
        static readonly string[] ItemNames={"월광초 꽃잎","불도마뱀 비늘","수정 가루","독버섯","푸른 수정","붉은 수정","계시의 물약"};
        static readonly Color[] ItemColors={Color.white,new Color(1,.3f,.15f),Color.cyan,new Color(.7f,.2f,.8f),Color.blue,Color.red,Color.yellow};
        static ItemData Item(int index)=>AssetDatabase.LoadAssetAtPath<ItemData>("Assets/Data/Items/"+ItemKeys[index]+".asset");
        public static void S4()
        {
            S3(); Directory.CreateDirectory("Assets/Data/Items");
            var catalog=new ItemData[7];
            for(int i=0;i<7;i++)
            {
                var d=Item(i); if(d==null) { d=ScriptableObject.CreateInstance<ItemData>(); AssetDatabase.CreateAsset(d,"Assets/Data/Items/"+ItemKeys[i]+".asset"); }
                d.itemId=ItemKeys[i]; d.displayName=ItemNames[i];
                if(d.icon==null)
                {
                    var tex=new Texture2D(32,32); tex.name=ItemKeys[i]+"-color";
                    var colors=new Color[1024]; for(int j=0;j<colors.Length;j++) colors[j]=ItemColors[i]; tex.SetPixels(colors); tex.Apply();
                    AssetDatabase.AddObjectToAsset(tex,d); var sprite=Sprite.Create(tex,new Rect(0,0,32,32),new Vector2(.5f,.5f)); sprite.name=ItemKeys[i]+"-icon"; AssetDatabase.AddObjectToAsset(sprite,d); d.icon=sprite;
                }
                EditorUtility.SetDirty(d); catalog[i]=d;
            }
            Set(Find("InventoryManager").GetComponent<InventoryManager>(),"itemCatalog",catalog); Save();
        }
    }
}
