using System.Linq;
using MagicEscape.Core;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static Material WarehouseMotif(RuneId id)
        {
            var path="Assets/Art/Warehouse/Pictogram_"+id+".png";
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            var mat=ArtMaterial("WarehousePictogram_"+id,new Color(.64f,.55f,.37f));
            mat.shader=Shader.Find("MagicEscape/AlchemyEngraving");
            mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            mat.SetColor("_BaseColor",new Color(.64f,.55f,.37f));mat.SetColor("_EmissionColor",Color.black);mat.EnableKeyword("_EMISSION");EditorUtility.SetDirty(mat);return mat;
        }
        static void ConfigureScaleWorkbench(Material wood)
        {
            // Keep the original top at 1.20m and pickup home at (3.65,1.30,.05).
            // Four discrete legs replace the solid slab-like side supports.
            var root=new GameObject("WarehouseIngredientWorkbench").transform;
            root.SetParent(Find("WarehouseStorage").transform,true);
            var top=Box("WarehouseIngredientShelf",root,new Vector3(3.83f,1.15f,.05f),new Vector3(.90f,.10f,.68f));
            top.GetComponent<Renderer>().sharedMaterial=wood;
            for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)
            {
                var leg=Box("WarehouseShelfLeg",root,new Vector3(3.83f+x*.36f,.55f,.05f+z*.25f),new Vector3(.10f,1.10f,.10f));
                leg.GetComponent<Renderer>().sharedMaterial=wood;
            }
            foreach(int z in new[]{-1,1})WorldVisual("WorkbenchApron",root,new Vector3(3.83f,1.04f,.05f+z*.25f),new Vector3(.82f,.17f,.07f),wood);
            var seam=ArtMaterial("WarehouseSeam",new Color(.045f,.026f,.015f));
            for(int n=-1;n<=1;n++)WorldVisual("WorkbenchPlankJoint",root,new Vector3(3.83f,1.2005f,.05f+n*.16f),new Vector3(.89f,.001f,.008f),seam);
            // Small desaturated fixed dressing, no pickup, inspectable, rune or clue component.
            var clay=ArtMaterial("WarehouseIngredientCeramic",new Color(.27f,.25f,.19f));
            var glass=ArtMaterial("WarehouseIngredientGlass",new Color(.20f,.26f,.24f));
            for(int n=0;n<2;n++)
            {
                float z=-.16f+n*.38f;
                WorldVisual("WorkbenchDecorJar",root,new Vector3(4.08f,1.265f,z),new Vector3(.10f,.065f,.10f),glass,PrimitiveType.Cylinder);
                WorldVisual("WorkbenchDecorJarNeck",root,new Vector3(4.08f,1.345f,z),new Vector3(.055f,.025f,.055f),glass,PrimitiveType.Cylinder);
                WorldVisual("WorkbenchDecorCork",root,new Vector3(4.08f,1.375f,z),new Vector3(.06f,.008f,.06f),wood,PrimitiveType.Cylinder);
            }
            // Low mortar with a recessed dark bowl, below the hero bottle silhouette.
            WorldVisual("WorkbenchMortar",root,new Vector3(3.71f,1.235f,.26f),new Vector3(.13f,.035f,.13f),clay,PrimitiveType.Cylinder);
            WorldVisual("WorkbenchMortarBowl",root,new Vector3(3.71f,1.2705f,.26f),new Vector3(.095f,.001f,.095f),seam,PrimitiveType.Cylinder);
            WorldVisual("WorkbenchPestle",root,new Vector3(3.75f,1.282f,.26f),new Vector3(.14f,.025f,.03f),clay);
        }
        static void ConfigureWarehouseStage7()
        {
            // Substitute all historical alchemical mask dependencies, including lamps/inscription.
            foreach(var renderer in Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                var materials=renderer.sharedMaterials;
                for(int i=0;i<materials.Length;i++)
                {
                    var m=materials[i];if(m==null)continue;
                    foreach(RuneId id in System.Enum.GetValues(typeof(RuneId)))
                    {
                        if(m.GetTexturePropertyNames().Any(p=>AssetDatabase.GetAssetPath(m.GetTexture(p))=="Assets/Art/S10b/"+id+".png"))
                        {materials[i]=WarehouseMotif(id);break;}
                    }
                }
                renderer.sharedMaterials=materials;
            }
            foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
            {
                var slab=stone.transform.Find("AlchemicalSlab");slab.SetParent(null,true);slab.localScale=new Vector3(.41f,.055f,.49f);slab.SetParent(stone.transform,true);
                var label=stone.transform.Find("RuneName_"+stone.Rune);label.SetParent(null,true);label.localScale=new Vector3(.38f,.15f,1);label.SetParent(stone.transform,true);
            }
            foreach(string roman in new[]{"I","II","III","IV"})
            {
                var label=Find("PaintingLabel_"+roman).GetComponent<TMPro.TMP_Text>();label.text="";
                var frame=Find("ArtB_PictureFrame_"+roman).transform;
                var icon=frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("ArtB_CornerRune_"));
                icon.localPosition=new Vector3(-.30f,-.51f,-.103f);icon.localScale=new Vector3(.19f,.19f,1);
                var name=frame.GetComponentsInChildren<Transform>().Single(t=>t.name.StartsWith("RuneName_"));
                name.localPosition=new Vector3(.14f,-.51f,-.103f);name.localScale=new Vector3(.48f,.16f,1);
                frame.Find("ArtB_RomanPlaque").localScale=new Vector3(1.02f,.23f,.07f);
            }
            var seedPicture=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Warehouse/Story_1.png");
            Set(Find("Painting_I").GetComponent<InspectableObject>(),"picture",seedPicture);
            var seedMaterial=ArtMaterial("WarehouseSeedStory",Color.white);seedMaterial.mainTexture=seedPicture;
            EditorUtility.SetDirty(seedMaterial);
            Find("ArtB_PictureFrame_I").transform.Find("ArtB_StoryIllustration").GetComponent<Renderer>().sharedMaterial=seedMaterial;
            var recipe=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Warehouse/PictureRecipe.png");
            Set(Find("RecipeBook").GetComponent<InspectableObject>(),"picture",recipe);
            var recipeMat=ArtMaterial("WarehouseRecipe",Color.white);recipeMat.mainTexture=recipe;
            recipeMat.SetTexture("_EmissionMap",recipe);recipeMat.SetColor("_EmissionColor",Color.white*.28f);EditorUtility.SetDirty(recipeMat);
            Find("OpenPictureRecipe").GetComponent<Renderer>().sharedMaterial=recipeMat;
            var wood=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            // Supported raised chest: the gate and hinge travel together, respawn home is authored here.
            Find("IngredientChest").transform.position+=Vector3.up*.60f;
            // A steep open lid clears the wall plaque instead of forming a horizontal screen in front of III.
            Find("ChestLidHinge").GetComponent<XriClearable>().openEuler=new Vector3(100,0,0);
            var baseBox=Box("WarehouseChestPlinth",Find("WarehouseStorage").transform,new Vector3(-3.25f,.30f,-2.3f),new Vector3(1.12f,.60f,.78f));baseBox.GetComponent<Renderer>().sharedMaterial=wood;
            MoveItem(ItemIds.CrystalDust,new Vector3(-3.25f,.96f,-2.3f));
            MoveItem(ItemIds.SalamanderScale,new Vector3(3.65f,1.30f,.05f));
            ConfigureScaleWorkbench(wood);
            // Damp barrel top by the east work aisle, not a hidden floor corner.
            MoveItem(ItemIds.Toadstool,new Vector3(3.5f,1.10f,-1.25f));
            var barrel=WorldVisual("WarehouseDampBarrel",Find("WarehouseStorage").transform,new Vector3(3.55f,.48f,-1.25f),new Vector3(.70f,.48f,.70f),wood,PrimitiveType.Cylinder);
            barrel.AddComponent<BoxCollider>().size=new Vector3(1,2,1);
            for(int side=-1;side<=1;side+=2)WorldVisual("WarehouseBarrelHoop",barrel.transform,new Vector3(3.55f,.48f+side*.32f,-1.25f),new Vector3(.72f,.035f,.72f),ArtMaterial("WarehouseIron",new Color(.13f,.105f,.075f)),PrimitiveType.Cylinder);
            MoveItem(ItemIds.CrystalSapphire,new Vector3(-.8f,1.12f,-.3f));
            // Reward gems need a readable body, not just almost-transparent edges.
            foreach(var item in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None)
                .Where(i=>i.ItemId==ItemIds.CrystalRuby||i.ItemId==ItemIds.CrystalSapphire))
                foreach(var r in item.GetComponentsInChildren<Renderer>(true))
                {
                    var original=r.sharedMaterial;
                    if(original==null||original.shader.name!="MagicEscape/ElementCrystal")continue;
                    string path="Assets/Settings/Art_WarehouseReward_"+item.ItemId+".mat";
                    var material=AssetDatabase.LoadAssetAtPath<Material>(path);
                    if(material==null){material=new Material(original);AssetDatabase.CreateAsset(material,path);}
                    material.CopyPropertiesFromMaterial(original);material.SetFloat("_Opacity",.65f);
                    r.sharedMaterial=material;EditorUtility.SetDirty(material);
                }
            // The P2 stand is immediately beside the cauldron; original ledger rewards unchanged.
            var dark=SceneComponent<WarehouseDarkening>();
            var guides=dark.localLights.ToList();
            foreach(var item in Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                var light=new GameObject("WarehouseItemGuide_"+item.ItemId).AddComponent<Light>();
                light.transform.position=item.transform.position+new Vector3(-.18f,.30f,-.12f);
                light.type=LightType.Point;light.range=1.25f;light.intensity=.55f;light.color=new Color(.78f,.81f,.72f);light.shadows=LightShadows.None;
                var beacon=light.gameObject.AddComponent<WarehouseItemBeacon>();beacon.pickup=item;beacon.source=light;
                beacon.reward=item.ItemId==ItemIds.CrystalSapphire||item.ItemId==ItemIds.CrystalRuby||item.ItemId==ItemIds.PotionRevelation;
                guides.Add(light);
            }
            dark.localLights=guides.ToArray();
            Find("Guide_Scale").transform.position=new Vector3(3.65f,1.58f,.05f);
            Find("Guide_Chest").transform.position=new Vector3(-3.25f,1.32f,-2.3f);
            AssetDatabase.DeleteAsset("Assets/Art/Warehouse/ChalkBlanks.png");
            AssetDatabase.DeleteAsset("Assets/Settings/Art_S10e_WarehouseChalkBlanks.mat");
        }
    }
}
