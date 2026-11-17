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
            books.parkingEuler=new Vector3(0,90,0); // Fits between the fixed workbench jars.
            for(int i=0;i<4;i++)
            {
                var book=Box("ScaleCoverBook_"+i,books.transform,new Vector3(0,-.10f+i*.067f,(i%2)*.012f),new Vector3(.24f,.06f,.46f));
                book.GetComponent<Renderer>().sharedMaterial=wood; Object.DestroyImmediate(book.GetComponent<Collider>());
            }
            Item(ItemIds.SalamanderScale).gameObject.AddComponent<ClearingGate>().books=books;
            // The mushroom cap peeks through the aisle-side gap below a hinged barrel cover.
            var hinge=new GameObject("ToadstoolBarrelLid");hinge.transform.SetParent(storage,false);
            hinge.transform.position=new Vector3(3.86f,1.24f,-1.25f);
            var lid=Box("DampBarrelCover",hinge.transform,new Vector3(-.34f,0,0),new Vector3(.70f,.06f,.70f));lid.GetComponent<Renderer>().sharedMaterial=wood;
            var grip=Box("BarrelCoverHandle",hinge.transform,new Vector3(-.34f,.06f,0),new Vector3(.18f,.06f,.065f));grip.GetComponent<Renderer>().sharedMaterial=wood;
            hinge.AddComponent<HandlingProfile>().weight=HandlingWeight.Fixed;
            var clear=hinge.AddComponent<XriClearable>();clear.kind=ClearingKind.Hinge;clear.interactionManager=manager;
            clear.closedPosition=hinge.transform.localPosition;clear.openEuler=new Vector3(0,0,-105);
            clear.colliders.AddRange(hinge.GetComponentsInChildren<Collider>());
            for(int side=-1;side<=1;side+=2)
            {
                WorldVisual("BarrelCoverHingePost",storage,new Vector3(3.86f,1.10f,-1.25f+side*.27f),new Vector3(.055f,.28f,.055f),wood);
                WorldVisual("BarrelCoverHingePin",storage,new Vector3(3.86f,1.24f,-1.25f+side*.27f),Vector3.one*.075f,wood,PrimitiveType.Sphere);
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
                prop.AddComponent<ClearingFeedback>();
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
