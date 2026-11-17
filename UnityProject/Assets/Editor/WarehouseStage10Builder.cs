using System.Linq;
using MagicEscape.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object=UnityEngine.Object;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static void ConfigureWarehouseStage10()
        {
            var storage=Find("WarehouseStorage").transform;
            var wood=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10b_Timber.mat");
            var manager=SceneComponent<XRInteractionManager>();
            // A supported stack in front of the bottle, leaving its illuminated neck visible.
            var books=NewLift("ScaleBookStack",storage,new Vector3(3.40f,1.335f,.04f),new Vector3(.24f,.27f,.48f),new Vector3(3.95f,1.335f,.04f));
            books.parking=new Vector3(3.96f,1.335f,.04f);
            books.parkingEuler=Vector3.zero;
            var pages=ArtMaterial("WarehouseBookPages",new Color(.68f,.60f,.43f));
            var bands=ArtMaterial("WarehouseBookBands",new Color(.37f,.28f,.15f));
            Color[] covers={new Color(.16f,.085f,.045f),new Color(.22f,.065f,.045f),new Color(.055f,.13f,.085f),new Color(.12f,.105f,.075f)};
            for(int i=0;i<4;i++)
            {
                var book=new GameObject("ScaleCoverBook_"+i).transform;book.SetParent(books.transform,false);
                book.localPosition=new Vector3((i%2==0?-.007f:.007f),-.135f+i*.0675f,0);
                book.localRotation=Quaternion.Euler(0,new[]{-3f,2f,-1f,4f}[i],0);
                float w=.22f-i*.008f,d=.44f-i*.012f;
                var cover=ArtMaterial("WarehouseBookCover"+i,covers[i]);
                BookPart("Pages",book,new Vector3(.005f,.03375f,0),new Vector3(w-.018f,.0455f,d-.024f),pages);
                foreach(float y in new[]{.0055f,.062f})BookPart("Cover",book,new Vector3(0,y,0),new Vector3(w,.011f,d),cover);
                BookPart("Spine",book,new Vector3(-w/2+.008f,.03375f,0),new Vector3(.016f,.0565f,d),cover);
                foreach(float z in new[]{-d*.32f,d*.32f})BookPart("RaisedSpineBand",book,new Vector3(-w/2-.001f,.03375f,z),new Vector3(.006f,.045f,.012f),bands);
                foreach(float y in new[]{.025f,.038f,.048f})BookPart("PageEdge",book,new Vector3(w/2-.003f,y,0),new Vector3(.001f,.001f,d-.03f),bands);
            }
            Item(ItemIds.SalamanderScale).gameObject.AddComponent<ClearingGate>().books=books;
            // Hollow staved barrel: no solid primitive top under the mushroom.
            Object.DestroyImmediate(Find("ArtB_WarehouseDampBarrel"));
            var barrel=new GameObject("WarehouseDampBarrel").transform;barrel.SetParent(storage,false);
            var iron=ArtMaterial("WarehouseIron",new Color(.13f,.105f,.075f));
            for(int i=0;i<32;i++)
            {
                float angle=i*Mathf.PI*2/32;
                var stave=Box("BarrelStave",barrel,new Vector3(3.55f+Mathf.Cos(angle)*.325f,.59f,-1.25f+Mathf.Sin(angle)*.325f),new Vector3(.05f,1.18f,.0645f));
                stave.transform.rotation=Quaternion.Euler(0,-angle*Mathf.Rad2Deg,0);stave.GetComponent<Renderer>().sharedMaterial=wood;
                foreach(float y in new[]{.16f,.97f,1.165f})
                {
                    var hoop=Box("BarrelHoop",barrel,new Vector3(3.55f+Mathf.Cos(angle)*.352f,y,-1.25f+Mathf.Sin(angle)*.352f),new Vector3(.012f,.035f,.071f));
                    hoop.transform.rotation=stave.transform.rotation;hoop.GetComponent<Renderer>().sharedMaterial=iron;
                    Object.DestroyImmediate(hoop.GetComponent<Collider>());
                }
            }
            var floor=WorldVisual("BarrelRaisedFloor",barrel,new Vector3(3.55f,1.015f,-1.25f),new Vector3(.60f,.015f,.60f),wood,PrimitiveType.Cylinder);
            // Mushroom home stays at 1.10m: shallow, reachable interior under the 1.18m rim.
            var hinge=new GameObject("ToadstoolBarrelLid");hinge.transform.SetParent(storage,false);
            hinge.transform.position=new Vector3(3.90f,1.21f,-1.25f);
            var lid=GameObject.CreatePrimitive(PrimitiveType.Cylinder);lid.name="DampBarrelCover";lid.transform.SetParent(hinge.transform,false);
            lid.transform.localPosition=new Vector3(-.35f,0,0);lid.transform.localScale=new Vector3(.70f,.02f,.70f);lid.GetComponent<Renderer>().sharedMaterial=wood;
            var grip=Box("BarrelCoverHandle",hinge.transform,new Vector3(-.35f,.045f,0),new Vector3(.15f,.05f,.045f));grip.GetComponent<Renderer>().sharedMaterial=iron;
            // Shallow scored joints do not turn the round lid into floating square planks.
            foreach(float z in new[]{-.20f,0,.20f})BookPart("LidPlankJoint",hinge.transform,new Vector3(-.35f,.0205f,z),new Vector3(z==0?.66f:.54f,.001f,.003f),iron);
            hinge.AddComponent<HandlingProfile>().weight=HandlingWeight.Fixed;
            var clear=hinge.AddComponent<XriClearable>();clear.kind=ClearingKind.Hinge;clear.interactionManager=manager;
            clear.closedPosition=hinge.transform.localPosition;clear.openEuler=new Vector3(0,0,-110);
            clear.colliders.AddRange(hinge.GetComponentsInChildren<Collider>());
            foreach(float z in new[]{-.16f,.16f})
            {
                WorldVisual("BarrelCoverHinge",storage,new Vector3(3.89f,1.19f,-1.25f+z),new Vector3(.07f,.07f,.08f),iron);
            }
            Item(ItemIds.Toadstool).gameObject.AddComponent<ClearingGate>().blocker=clear;
            // A packing roll lies over the dust jar inside the chest. Opening is not collecting:
            // both the existing lid and the lifted roll must clear before dust can be selected.
            var roll=NewLift("DustPackingRoll",storage,new Vector3(-3.25f,1.10f,-2.3f),new Vector3(.46f,.16f,.48f),new Vector3(-2.62f,1.04f,-2.3f));
            var linen=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_ClearingCloth.mat");
            for(int i=0;i<3;i++)
            {
                var fabric=Box("PackingLinen_"+i,roll.transform,new Vector3(0,-.055f+i*.055f,0),new Vector3(.46f,.05f,.46f-i*.035f));
                fabric.GetComponent<Renderer>().sharedMaterial=linen;Object.DestroyImmediate(fabric.GetComponent<Collider>());
            }
            var support=Box("DustPackingSideShelf",storage,new Vector3(-2.62f,.93f,-2.3f),new Vector3(.52f,.06f,.56f));support.GetComponent<Renderer>().sharedMaterial=wood;
            var leg=Box("DustPackingShelfLeg",storage,new Vector3(-2.62f,.45f,-2.3f),new Vector3(.13f,.90f,.13f));leg.GetComponent<Renderer>().sharedMaterial=wood;
            Item(ItemIds.CrystalDust).GetComponent<ClearingGate>().books=roll;
            foreach(var prop in Object.FindObjectsByType<XriClearable>(FindObjectsSortMode.None).Select(x=>x.gameObject)
                .Concat(Object.FindObjectsByType<XriLooseProp>(FindObjectsSortMode.None).Select(x=>x.gameObject)))
            {
                var feedback=prop.AddComponent<ClearingFeedback>();
                if(prop.name=="ScaleBookStack"){feedback.highlightMix=.10f;feedback.highlightEmission=.015f;}
                foreach(var t in prop.GetComponentsInChildren<Transform>(true))t.gameObject.isStatic=false;
            }
            foreach(var gate in Object.FindObjectsByType<ClearingGate>(FindObjectsSortMode.None))
            {
                var light=new GameObject("ClearingReveal_"+gate.name).AddComponent<Light>();
                light.transform.position=gate.transform.position+new Vector3(gate.transform.position.x<0?.32f:-.32f,.18f,0);
                light.type=LightType.Point;light.range=1.15f;light.intensity=.12f;light.color=new Color(.86f,.81f,.64f);light.shadows=LightShadows.None;
                var reveal=light.gameObject.AddComponent<ClearingReveal>();reveal.gate=gate;reveal.source=light;
            }
        }
        static void BookPart(string name,Transform parent,Vector3 position,Vector3 size,Material material)
        {
            var part=Box(name,parent,position,size);part.GetComponent<Renderer>().sharedMaterial=material;
            Object.DestroyImmediate(part.GetComponent<Collider>());
        }
        static ItemPickup Item(string id) => Object.FindObjectsByType<ItemPickup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(x=>x.ItemId==id);
        static XriLooseProp NewLift(string name,Transform parent,Vector3 home,Vector3 size,Vector3 parking)
        {
            var root=new GameObject(name);root.transform.SetParent(parent,false);root.transform.position=home;root.layer=9;
            root.AddComponent<BoxCollider>().size=size;
            var rb=root.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
            var grab=root.AddComponent<XRGrabInteractable>();grab.interactionManager=SceneComponent<XRInteractionManager>();
            grab.throwOnDetach=false;grab.movementType=XRBaseInteractable.MovementType.Instantaneous;
            root.AddComponent<HandlingProfile>().weight=HandlingWeight.Lift;
            var loose=root.AddComponent<XriLooseProp>();loose.home=home;loose.parking=parking;return loose;
        }
    }
}
