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
            // A wall-mounted, unnumbered dictionary near the altar. Its dowel and pegs touch the wall.
            var table = new GameObject("ElementTable"); table.transform.position=new Vector3(-4.35f,1.6f,.65f);table.transform.rotation=Quaternion.Euler(0,-90,0);
            var sheet=EShape("ElementParchment",table.transform,table.transform.position,new Vector3(1.15f,.86f,1),EPicture("ElementTable",.22f),PrimitiveType.Quad);
            sheet.transform.rotation=table.transform.rotation;
            var hit=table.AddComponent<BoxCollider>();hit.size=new Vector3(1.15f,.86f,.06f);
            var inspect=table.AddComponent<InspectableObject>(); Set(inspect,"picture",AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"ElementTable.png"));Set(inspect,"promptVerb","원소표 살펴보기");
            table.AddComponent<XRSimpleInteractable>().interactionManager=manager;table.AddComponent<XriPress>();table.AddComponent<HandlingProfile>();
            var clothRoot=new GameObject("ElementTableCloth");clothRoot.transform.SetPositionAndRotation(new Vector3(-4.22f,2.06f,.65f),table.transform.rotation);
            var cloth=Box("HangingCloth",clothRoot.transform,new Vector3(0,-.46f,0),new Vector3(1.25f,.94f,.035f));
            var linen=ArtMaterial("ClearingCloth",new Color(.62f,.56f,.45f));
            linen.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"ClearingLinen.png");linen.SetFloat("_Smoothness",.03f);EditorUtility.SetDirty(linen);
            cloth.GetComponent<Renderer>().sharedMaterial=linen;
            cloth.GetComponent<Renderer>().enabled=false; // hit volume; fabric is a curved mesh, not rigid boards.
            var fabric=new GameObject("FoldedLinen");fabric.transform.SetParent(clothRoot.transform,false);
            fabric.AddComponent<MeshFilter>().sharedMesh=ClearingFabricMesh();fabric.AddComponent<MeshRenderer>().sharedMaterial=linen;
            linen.SetFloat("_Cull",0);
            var rod=Box("ClothDowel",table.transform,new Vector3(0,.48f,-.08f),new Vector3(1.35f,.045f,.25f));rod.GetComponent<Renderer>().sharedMaterial=wood;
            foreach(float side in new[]{-.48f,.48f})
            {
                var tie=Box("ParchmentTie",table.transform,new Vector3(side,.44f,-.01f),new Vector3(.025f,.12f,.025f));tie.GetComponent<Renderer>().sharedMaterial=wood;
            }
            var curtain=clothRoot.AddComponent<XriClearable>();curtain.kind=ClearingKind.Cloth;curtain.interactionManager=manager;curtain.closedPosition=clothRoot.transform.localPosition;curtain.openPosition=curtain.closedPosition;
            curtain.colliders.AddRange(clothRoot.GetComponentsInChildren<Collider>());clothRoot.AddComponent<HandlingProfile>().weight=HandlingWeight.Fixed;
            table.AddComponent<ClearingGate>().blocker=curtain;
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
            var scrap=new GameObject("BookStackClue");scrap.transform.position=new Vector3(-1.8f,.605f,-3.05f);scrap.transform.rotation=Quaternion.Euler(90,0,0);
            var sc=scrap.AddComponent<BoxCollider>();sc.size=new Vector3(.42f,.29f,.006f);
            var drawing=EShape("LooseManuscript",scrap.transform,scrap.transform.position,new Vector3(.42f,.29f,1),EPicture("ElementTable"),PrimitiveType.Quad);drawing.transform.rotation=scrap.transform.rotation;
            Set(scrap.AddComponent<InspectableObject>(),"picture",AssetDatabase.LoadAssetAtPath<Texture2D>(EArt+"ElementTable.png"));
            scrap.AddComponent<XRSimpleInteractable>().interactionManager=manager;scrap.AddComponent<XriPress>();scrap.AddComponent<ClearingGate>().books=loose;
            // Dynamic hierarchies must not be baked into the room's static batch (including visual children).
            foreach(var moving in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None).Select(x=>x.transform)
                .Concat(Object.FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(x=>x.transform)))
                foreach(var part in moving.GetComponentsInChildren<Transform>(true)) part.gameObject.isStatic=false;
            // Same material instance / vector motif as each matching altar stone, larger on the frame.
            foreach(var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r=>r.name.StartsWith("ArtB_CornerRune_")))
            {
                r.transform.localScale=new Vector3(.40f,.40f,1);r.transform.localPosition=new Vector3(-.32f,.22f,-.028f);
                r.sharedMaterial=FGlyph((RuneId)System.Enum.Parse(typeof(RuneId),r.name.Replace("ArtB_CornerRune_","")));
            }
        }
        static Mesh ClearingFabricMesh()
        {
            const string path="Assets/Settings/ClearingFabricMesh.asset";
            var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);bool fresh=mesh==null;
            if(fresh)mesh=new Mesh {name="Gathered woven linen"};else mesh.Clear();
            const int nx=48,ny=16;var vertices=new Vector3[(nx+1)*(ny+1)];var uv=new Vector2[vertices.Length];
            var triangles=new int[nx*ny*6];int index=0;
            for(int y=0;y<=ny;y++)for(int x=0;x<=nx;x++)
            {
                float u=(float)x/nx,v=(float)y/ny;
                float wave=Mathf.Sin(u*Mathf.PI*16);
                vertices[y*(nx+1)+x]=new Vector3((u-.5f)*1.25f,-v*(.93f+.018f*Mathf.Cos(u*17)), -.026f+wave*(.018f+.012f*v));
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
