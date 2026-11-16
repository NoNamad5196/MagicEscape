using System.Linq;
using MagicEscape.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object = UnityEngine.Object;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static void ConfigureClearing()
        {
            var manager = SceneComponent<XRInteractionManager>();
            var props = Find("WarehouseStorage").transform;
            var wood = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            // Independent supported sliding chests, with visible handles and scuff rails.
            // West storage rails exclude the altar (x > -2.5), south chest and north exit.
            foreach (string roman in new[] { "I", "IV" })
            {
                var painting = Find("Painting_"+roman);
                float z = painting.transform.position.z;
                var crate = StorageCrate(props,"ClearingCrate_"+roman,new Vector3(-3.83f,0,z),new Vector3(.85f,1.02f,.86f));
                crate.GetComponent<HandlingProfile>().weight = HandlingWeight.PushPull;
                var clear = crate.AddComponent<XriClearable>(); clear.interactionManager = manager; clear.kind = ClearingKind.Slide;
                clear.closedPosition = crate.transform.position;
                // Each crate moves in Z only, within its own bay. No stacked load can be stranded.
                clear.minPosition = clear.closedPosition - (roman == "IV" ? Vector3.forward : Vector3.zero);
                clear.maxPosition = clear.closedPosition + (roman == "I" ? Vector3.forward : Vector3.zero);
                clear.revealDistance = 1.04f;
                var handle = WorldVisual("ClearingHandle",crate.transform,crate.transform.position+new Vector3(.44f,0,0),new Vector3(.045f,.08f,.38f),ArtMaterial("WarehouseIron",new Color(.13f,.105f,.075f)));
                for (int side=-1;side<=1;side+=2)
                    WorldVisual("DragScuff_"+roman+side,props,new Vector3(-3.83f+side*.32f,.006f,z+(roman=="I"?.5f:-.5f)),new Vector3(.025f,.008f,1.9f),ArtMaterial("WarehouseSeam",new Color(.045f,.026f,.015f)));
                painting.AddComponent<ClearingGate>().blocker = clear;
            }
            // Existing central stacks would intersect these sliding bays: keep their seven-crate count,
            // floor support and original detail, but put the compact stacks against the south wall.
            for (int i=0;i<7;i++)
                Find("StackedCrate_"+i).transform.position = new Vector3(-3.8f+(i%3)*.68f,.24f+(i/3)*.48f,-3.08f);
            var chest = Find("IngredientChest").transform;
            Object.DestroyImmediate(chest.Find("ChestOpenLid").gameObject);
            var pivot = new GameObject("ChestLidHinge"); pivot.transform.SetParent(chest,false); pivot.transform.localPosition = new Vector3(0,.61f,.315f);
            var lid = Box("ChestLid",pivot.transform,new Vector3(0,0,-.35f),new Vector3(1.05f,.07f,.7f)); lid.GetComponent<Renderer>().sharedMaterial=wood;
            pivot.AddComponent<HandlingProfile>().weight=HandlingWeight.Fixed;
            var hinge = pivot.AddComponent<XriClearable>(); hinge.kind=ClearingKind.Hinge; hinge.interactionManager=manager;
            hinge.closedPosition=pivot.transform.localPosition; hinge.openEuler=new Vector3(165,0,0);
            hinge.colliders.Add(lid.GetComponent<Collider>());
            Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(p=>p.ItemId==ItemIds.CrystalDust).gameObject.AddComponent<ClearingGate>().blocker=hinge;
            // A fitted linen cover over III; the numbered painting itself remains wall-mounted.
            var paintingIII=Find("Painting_III");
            var frameIII=paintingIII.transform.Find("ArtB_PictureFrame_III");
            var clothRoot=new GameObject("PaintingIIICloth");
            clothRoot.transform.SetPositionAndRotation(new Vector3(-4.20f,2.19f,paintingIII.transform.position.z),frameIII.rotation);
            var cloth=Box("HangingCloth",clothRoot.transform,new Vector3(0,-.49f,0),new Vector3(1.12f,1.04f,.055f));
            var linen=ArtMaterial("ClearingCloth",new Color(.62f,.56f,.45f));
            linen.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"ClearingLinen.png");linen.SetFloat("_Smoothness",.03f);linen.SetFloat("_Cull",0);EditorUtility.SetDirty(linen);
            cloth.GetComponent<Renderer>().enabled=false;
            var fabric=new GameObject("FoldedLinen");fabric.transform.SetParent(clothRoot.transform,false);
            fabric.AddComponent<MeshFilter>().sharedMesh=ClearingFabricMesh();fabric.AddComponent<MeshRenderer>().sharedMaterial=linen;
            // Top return lies over the frame's upper edge, not on a separate dictionary dowel.
            var top=Box("ClothTopReturn",clothRoot.transform,new Vector3(0,0,.07f),new Vector3(1.12f,.025f,.21f));
            top.GetComponent<Renderer>().sharedMaterial=linen;Object.DestroyImmediate(top.GetComponent<Collider>());
            var curtain=clothRoot.AddComponent<XriClearable>();curtain.kind=ClearingKind.Cloth;curtain.interactionManager=manager;
            curtain.closedPosition=clothRoot.transform.localPosition;curtain.openPosition=curtain.closedPosition;
            curtain.colliders.AddRange(clothRoot.GetComponentsInChildren<Collider>());clothRoot.AddComponent<HandlingProfile>().weight=HandlingWeight.Fixed;
            paintingIII.AddComponent<ClearingGate>().blocker=curtain;
            // Books lift, but are not inventory items. Parking is on the next supported crate top.
            var books=new GameObject("MovableBookStack");books.transform.position=new Vector3(-1.8f,.73f,-3.05f);
            var bc=books.AddComponent<BoxCollider>();bc.size=new Vector3(.48f,.26f,.34f);
            for(int i=0;i<4;i++)
            {
                var b=Box("Book_"+i,books.transform,new Vector3((i%2)*.018f,-.095f+i*.062f,0),new Vector3(.46f,.055f,.32f));b.GetComponent<Renderer>().sharedMaterial=wood;
                Object.DestroyImmediate(b.GetComponent<Collider>());
            }
            var rb=books.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            var grab=books.AddComponent<XRGrabInteractable>();grab.interactionManager=manager;grab.throwOnDetach=false;grab.movementType=XRBaseInteractable.MovementType.Instantaneous;
            books.AddComponent<HandlingProfile>().weight=HandlingWeight.Lift;books.layer=9;
            var loose=books.AddComponent<XriLooseProp>();loose.home=books.transform.position;loose.parking=new Vector3(-1.05f,.88f,-3.05f);
            // A plain bookbinding board is dressing, never an inspectable clue.
            var board=Box("BookbindingBoard",props,new Vector3(-1.8f,.607f,-3.05f),new Vector3(.42f,.014f,.29f));
            board.GetComponent<Renderer>().sharedMaterial=wood;
            // Dynamic hierarchies must not be baked into the room's static batch (including visual children).
            foreach(var moving in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None).Select(x=>x.transform)
                .Concat(Object.FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(x=>x.transform)))
                foreach(var part in moving.GetComponentsInChildren<Transform>(true)) part.gameObject.isStatic=false;
            ConfigureRuneNames();
            // Same material instance / vector motif as each matching altar stone, larger on the frame.
            foreach(var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r=>r.name.StartsWith("ArtB_CornerRune_")))
            {
                r.transform.localScale=new Vector3(.40f,.40f,1);r.transform.localPosition=new Vector3(-.32f,.22f,-.028f);
                r.sharedMaterial=FGlyph((RuneId)System.Enum.Parse(typeof(RuneId),r.name.Replace("ArtB_CornerRune_","")));
            }
        }
        static void ConfigureRuneNames()
        {
            foreach(var stone in Object.FindObjectsByType<RuneStone>(FindObjectsSortMode.None))
            {
                var slab=stone.transform.Find("AlchemicalSlab");
                slab.SetParent(null,true);slab.localScale=new Vector3(.36f,.055f,.43f);slab.SetParent(stone.transform,true);
                var icon=stone.transform.Find("ArtB_RuneIcon_"+stone.Rune);
                icon.SetParent(null,true);
                icon.position=stone.transform.position+new Vector3(0,.039f,.055f);
                icon.localScale=new Vector3(.25f,.25f,1);icon.SetParent(stone.transform,true);
                var label=RuneName(stone.transform,stone.Rune,stone.transform.position+new Vector3(0,.040f,-.135f),new Vector3(.33f,.11f,1));
                label.transform.SetParent(null,true);label.transform.rotation=Quaternion.Euler(90,0,0);label.transform.localScale=new Vector3(.33f,.11f,1);label.transform.SetParent(stone.transform,true);
                Set(stone.GetComponent<GlowFeedback>(),"additionalRenderers",new Renderer[]{label.GetComponent<Renderer>()});
                stone.GetComponent<AlchemicalPulse>().additionalEngravings=new Renderer[]{label.GetComponent<Renderer>()};
            }
            foreach(var corner in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r=>r.name.StartsWith("ArtB_CornerRune_")))
            {
                var id=(RuneId)System.Enum.Parse(typeof(RuneId),corner.name.Replace("ArtB_CornerRune_",""));
                var label=RuneName(corner.transform.parent,id,Vector3.zero,new Vector3(.36f,.12f,1));
                label.transform.localPosition=new Vector3(-.32f,-.06f,-.030f);label.transform.localRotation=Quaternion.identity;label.transform.localScale=new Vector3(.36f,.12f,1);
            }
        }
        static GameObject RuneName(Transform parent,RuneId id,Vector3 position,Vector3 size)
        {
            var importer=(TextureImporter)AssetImporter.GetAtPath(EArt+"RuneName_"+id+".png");
            importer.mipmapEnabled=false;importer.filterMode=FilterMode.Bilinear;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.SaveAndReimport();
            var mat=ArtMaterial("RuneName_"+id,new Color(.64f,.55f,.37f));
            mat.shader=Shader.Find("MagicEscape/AlchemyEngraving");
            mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"RuneName_"+id+".png"));
            mat.SetColor("_BaseColor",new Color(.64f,.55f,.37f));mat.SetColor("_EmissionColor",Color.black);mat.EnableKeyword("_EMISSION");EditorUtility.SetDirty(mat);
            return EShape("RuneName_"+id,parent,position,size,mat,PrimitiveType.Quad);
        }
        static Mesh ClearingFabricMesh()
        {
            const string path="Assets/Settings/PaintingCoverMesh.asset";
            var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);bool fresh=mesh==null;
            if(fresh)mesh=new Mesh {name="Gathered woven linen"};else mesh.Clear();
            const int nx=48,ny=16;var vertices=new Vector3[(nx+1)*(ny+1)];var uv=new Vector2[vertices.Length];
            var triangles=new int[nx*ny*6];int index=0;
            for(int y=0;y<=ny;y++)for(int x=0;x<=nx;x++)
            {
                float u=(float)x/nx,v=(float)y/ny;
                float wave=Mathf.Sin(u*Mathf.PI*16);
                vertices[y*(nx+1)+x]=new Vector3((u-.5f)*1.12f,-v*(1.04f+.018f*Mathf.Cos(u*17)), -.026f+wave*(.018f+.012f*v));
                uv[y*(nx+1)+x]=new Vector2(u,1-v);
                if(x<nx&&y<ny)
                {
                    int a=y*(nx+1)+x;triangles[index++]=a;triangles[index++]=a+1;triangles[index++]=a+nx+1;
                    triangles[index++]=a+1;triangles[index++]=a+nx+2;triangles[index++]=a+nx+1;
                }
            }
            mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();
            if(fresh)AssetDatabase.CreateAsset(mesh,path);else EditorUtility.SetDirty(mesh);return mesh;
        }
    }
}
