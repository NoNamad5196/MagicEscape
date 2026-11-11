using MagicEscape.Core;
using UnityEngine;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        public static void S8()
        {
            S7(); var p=SceneComponent<SealPuzzle>(); p.transform.position=new Vector3(-3,0,3.7f);
            Shape("Pedestal",p.transform,new Vector3(0,.5f,0),new Vector3(1.8f,1,.5f));
            var inscription=Component<SealInscription>("Inscription",p.transform); inscription.transform.localPosition=new Vector3(0,1.5f,.2f); Set(inscription,"puzzle",p);
            var slab=Shape("InscriptionSlab",inscription.transform,Vector3.zero,new Vector3(1.2f,.5f,.08f));
            Set(p,"inscriptionGlow",Glow(slab));
            var reveal=new GameObject("RevealedText"); reveal.transform.SetParent(inscription.transform,false);
            WorldText("InscriptionText",reveal.transform,GameText.InscriptionRevealed,new Vector3(0,.05f,-.055f),new Vector2(1.15f,.32f),1.65f);
            WorldText("Quad_WaterRune",reveal.transform,"물",new Vector3(-.4f,-.16f,-.055f),new Vector2(.3f,.15f),1.5f);
            WorldText("Quad_FireRune",reveal.transform,"불",new Vector3(.4f,-.16f,-.055f),new Vector2(.3f,.15f),1.5f); Set(p,"revealedInscription",reveal); reveal.SetActive(false);
            for(int i=0;i<2;i++)
            {
                var socket=Component<CrystalSocket>(i==0?"Socket_Left":"Socket_Right",p.transform); socket.transform.localPosition=new Vector3(i==0?-.6f:.6f,1.05f,-.05f); Set(socket,"side",i==0?SocketSide.Left:SocketSide.Right);
                var cup=Shape("SocketCup",socket.transform,Vector3.zero,new Vector3(.32f,.07f,.32f),PrimitiveType.Cylinder); Set(socket,"glow",Glow(cup));
                var crystal=Shape("PlacedCrystal",socket.transform,new Vector3(0,.16f,0),Vector3.one*.2f,PrimitiveType.Sphere,i==0?Color.blue:Color.red); Object.DestroyImmediate(crystal.GetComponent<Collider>()); crystal.SetActive(false); Set(socket,"placedCrystalVisual",crystal); Set(p,i==0?"leftSocket":"rightSocket",socket);
            }
            var core=Shape("SealCore",p.transform,new Vector3(0,1.15f,-.1f),Vector3.one*.25f,PrimitiveType.Sphere,new Color(.7f,.5f,1)); var glow=Glow(core); var interact=core.AddComponent<SealCore>(); Set(interact,"puzzle",p); Set(p,"coreGlow",glow);
            var light=Component<Light>("CoreLight",core.transform); light.type=LightType.Point; light.range=2; Set(glow,"glowLight",light);
            Set(p,"revealVfx",Particles("VFX_Reveal",p.transform,new Vector3(0,1.6f,0),new Color(.7f,.5f,1))); Set(p,"releaseVfx",Particles("VFX_Release",p.transform,new Vector3(0,1.6f,0),Color.yellow));
            var door=SceneComponent<DoorController>(); var hinge=door.transform.Find("DoorHinge"); Shape("DoorPanel",hinge,new Vector3(1.5f,1.3f,0),new Vector3(3,2.6f,.1f));
            Shape("DoorFrame_Left",door.transform,new Vector3(-1.575f,1.35f,0),new Vector3(.15f,2.7f,.25f)); Shape("DoorFrame_Right",door.transform,new Vector3(1.575f,1.35f,0),new Vector3(.15f,2.7f,.25f));
            var exitGo=new GameObject("ExitTrigger",typeof(BoxCollider)); exitGo.transform.SetParent(door.transform,false); var exit=exitGo.AddComponent<ExitTrigger>(); exit.transform.localPosition=new Vector3(0,1.25f,1);
            var box=(BoxCollider)exit.GetComponent<Collider>(); box.isTrigger=true; box.size=new Vector3(3,2.5f,1);
            var rb=exit.gameObject.AddComponent<Rigidbody>(); rb.isKinematic=true; rb.useGravity=false;
            Set(door,"exitTrigger",exit); Set(door,"openVfx",Particles("VFX_DoorOpen",door.transform,new Vector3(0,1.5f,0),Color.yellow)); exit.gameObject.SetActive(false);
            p.gameObject.SetActive(true); Save();
        }
    }
}



