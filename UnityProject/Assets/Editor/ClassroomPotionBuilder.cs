using MagicEscape.Core;
using UnityEngine;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        public static void S7()
        {
            S6(); var p=SceneComponent<PotionPuzzle>(); p.transform.position=new Vector3(3,0,2.5f);
            Shape("CauldronBody",p.transform,new Vector3(0,.5f,0),new Vector3(1,.5f,1),PrimitiveType.Cylinder,new Color(.1f,.12f,.13f));
            var liquid=Shape("Liquid",p.transform,new Vector3(0,1.02f,0),new Vector3(.85f,.02f,.85f),PrimitiveType.Cylinder,new Color(.2f,.3f,.5f)); Set(p,"liquidGlow",Glow(liquid));
            Set(p,"bubbleVfx",Particles("VFX_Bubble",p.transform,new Vector3(0,1.1f,0),Color.cyan,true)); Set(p,"smokeVfx",Particles("VFX_Smoke",p.transform,new Vector3(0,1.2f,0),Color.black)); Set(p,"successVfx",Particles("VFX_Success",p.transform,new Vector3(0,1.2f,0),Color.yellow));
            var book=Shape("RecipeBook",null,new Vector3(4.2f,1.1f,3.6f),new Vector3(.6f,.1f,.4f)); var inspect=book.AddComponent<InspectableObject>(); Set(inspect,"title","계시의 물약"); Set(inspect,"body",GameText.Recipe); Set(inspect,"promptVerb","마법서 읽기"); Set(inspect,"requirePuzzleSolved",true);
            var barrier=Shape("Barrier",null,book.transform.position,Vector3.one*.7f,PrimitiveType.Sphere,Color.cyan); Object.DestroyImmediate(barrier.GetComponent<Collider>());
            // Graybox barrier remains visibly translucent rather than hiding the book behind a solid cube.
            var mat=new Material(barrier.GetComponent<Renderer>().sharedMaterial); mat.name="Barrier";
            const string path="Assets/Settings/Barrier.asset"; var existing=UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
            if(existing==null){mat.SetFloat("_Surface",1); mat.SetFloat("_Blend",0); mat.SetFloat("_SrcBlend",5); mat.SetFloat("_DstBlend",10); mat.SetFloat("_ZWrite",0); mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); mat.renderQueue=3000; mat.color=new Color(.2f,.6f,1,.2f); UnityEditor.AssetDatabase.CreateAsset(mat,path); existing=mat;} else Object.DestroyImmediate(mat);
            barrier.GetComponent<Renderer>().sharedMaterial=existing; Set(inspect,"barrierVisual",barrier);
            var pickups=new[]{Pickup("Ingredient_Moonpetal",null,new Vector3(5,1.05f,-4.2f),0),Pickup("Ingredient_SalamanderScale",null,new Vector3(5.6f,1.4f,1.5f),1),Pickup("Ingredient_CrystalDust",null,new Vector3(-3.5f,.75f,-3.25f),2),Pickup("Ingredient_Toadstool",null,new Vector3(-5.4f,.09f,4.4f),3)};
            Set(p,"ingredientPickups",pickups);
            Shape("Window_SE",null,new Vector3(5,1.7f,-4.88f),new Vector3(1.5f,1.4f,.05f),PrimitiveType.Cube,new Color(.2f,.3f,.6f));
            Shape("Windowsill",null,new Vector3(5,.85f,-4.2f),new Vector3(1,.2f,.5f));
            Shape("PlantPot",null,new Vector3(5,.96f,-4.2f),new Vector3(.25f,.08f,.25f),PrimitiveType.Cylinder);
            Shape("Shelf",null,new Vector3(5.78f,.65f,1.5f),new Vector3(.25f,1.3f,1));
            Shape("Drawer",null,new Vector3(-3.5f,.35f,-3.4f),new Vector3(1.2f,.7f,.7f));
            WorldText("FlameLabel",null,"불꽃",new Vector3(5.6f,1.15f,1.23f),new Vector2(.6f,.2f),2);
            WorldText("StarLabel",null,"★",new Vector3(-3.5f,.5f,-3.77f),new Vector2(.3f,.3f),3);
            p.gameObject.SetActive(true); Save();
        }
    }
}
