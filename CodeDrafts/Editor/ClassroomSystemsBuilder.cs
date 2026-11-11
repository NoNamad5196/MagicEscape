using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static T Component<T>(string name, Transform parent = null) where T:Component
        { var g=new GameObject(name); g.transform.SetParent(parent,false); return g.AddComponent<T>(); }
        static void Set(object target,string field,object value)
        { target.GetType().GetField(field,BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value); EditorUtility.SetDirty((UnityEngine.Object)target); }
        static GameObject Find(string name) => GameObject.Find(name);
        static T Future<T>(string name) where T:Component
        { var g=new GameObject(name); g.SetActive(false); return g.AddComponent<T>(); }
        public static void S2()
        {
            S1();
            var inv=Component<InventoryManager>("InventoryManager");
            var audio=Component<AudioManager>("AudioManager");
            var bgm=audio.gameObject.AddComponent<AudioSource>(); bgm.playOnAwake=false; bgm.loop=true; bgm.volume=.4f;
            var sfx=audio.gameObject.AddComponent<AudioSource>(); sfx.playOnAwake=false;
            Set(audio,"bgmSource",bgm); Set(audio,"sfxSource",sfx);
            var es=Component<EventSystem>("EventSystem"); es.gameObject.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
            var p=Component<PlayerController>("Player"); p.tag="Player"; p.gameObject.layer=8; p.transform.position=new Vector3(0,0,-4);
            var cc=p.GetComponent<CharacterController>(); cc.height=1.8f; cc.radius=.3f; cc.center=new Vector3(0,.9f,0); cc.skinWidth=.05f;
            var cam=Component<Camera>("CameraPivot",p.transform); cam.transform.localPosition=new Vector3(0,1.6f,0); cam.tag="MainCamera";
            cam.fieldOfView=70; cam.nearClipPlane=.05f; cam.farClipPlane=50; cam.gameObject.AddComponent<AudioListener>(); cam.GetUniversalAdditionalCameraData().renderPostProcessing=true;
            Set(p,"cameraPivot",cam.transform);
            var interaction=p.gameObject.AddComponent<PlayerInteraction>(); Set(interaction,"viewCamera",cam); Set(interaction,"interactMask",(LayerMask)~(1<<8));
            var gm=Component<GameManager>("GameManager"); Set(gm,"player",p); Set(gm,"skipTitle",true);
            Set(gm,"runePuzzle",Future<RunePuzzle>("Altar")); Set(gm,"potionPuzzle",Future<PotionPuzzle>("Cauldron")); Set(gm,"sealPuzzle",Future<SealPuzzle>("SealDevice"));
            var door=Component<DoorController>("ExitDoor"); door.transform.position=new Vector3(0,0,5);
            var hinge=new GameObject("DoorHinge").transform; hinge.SetParent(door.transform,false); hinge.localPosition=new Vector3(-1.5f,0,0); Set(door,"hinge",hinge); Set(gm,"exitDoor",door);
            Save();
        }
    }
}
