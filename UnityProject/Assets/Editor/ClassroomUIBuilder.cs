using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        public static void ImportTMP() { AssetDatabase.importPackageCompleted += _ => { AssetDatabase.SaveAssets(); EditorApplication.Exit(0); }; AssetDatabase.ImportPackage(Directory.GetFiles("Library/PackageCache","TMP Essential Resources.unitypackage",SearchOption.AllDirectories).First(),false); }
        static TMP_FontAsset KoreanFont()
        {
            const string path="Assets/Fonts/NotoSansKR-SDF.asset";
            if(AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset")==null)
            {
                var package=Directory.GetFiles("Library/PackageCache","TMP Essential Resources.unitypackage",SearchOption.AllDirectories).First();
                AssetDatabase.ImportPackage(package,false);
            }
            var f=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(f==null)
            {
                f=TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/NotoSansKR.ttf"),48,9,GlyphRenderMode.SDFAA,4096,4096,AtlasPopulationMode.Dynamic,true);
                AssetDatabase.CreateAsset(f,path);
                AssetDatabase.AddObjectToAsset(f.material,f);
                foreach(var tex in f.atlasTextures) AssetDatabase.AddObjectToAsset(tex,f);
                string corpus=string.Concat(Directory.GetFiles("Assets/Scripts","*.cs",SearchOption.AllDirectories).Select(File.ReadAllText));
                f.TryAddCharacters(corpus,out string missing);
                EditorUtility.SetDirty(f);
            }
            return f;
        }
        static RectTransform Rect(string name,Transform parent,Vector2 size,Vector2 pos)
        { var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>(); r.SetParent(parent,false); r.sizeDelta=size; r.anchoredPosition=pos; return r; }
        static TMP_Text Label(string name,Transform parent,string text,Vector2 size,Vector2 pos,float fontSize=30)
        { var t=Rect(name,parent,size,pos).gameObject.AddComponent<TextMeshProUGUI>(); t.font=KoreanFont(); t.text=text; t.fontSize=fontSize; t.alignment=TextAlignmentOptions.Center; t.raycastTarget=false; return t; }
        static GameObject Panel(string name,Transform parent,Vector2 size,Vector2 pos)
        { var r=Rect(name,parent,size,pos); var image=r.gameObject.AddComponent<Image>(); image.color=new Color(.035f,.05f,.12f,.94f); return r.gameObject; }
        static void Button(string name,Transform parent,string text,Vector2 pos,UnityAction action)
        { var r=Rect(name,parent,new Vector2(300,64),pos); var image=r.gameObject.AddComponent<Image>(); image.color=new Color(.15f,.25f,.4f); var b=r.gameObject.AddComponent<Button>(); b.targetGraphic=image; UnityEventTools.AddPersistentListener(b.onClick,action); Label("Label",r,text,new Vector2(280,60),Vector2.zero); }
        public static void S3()
        {
            S2(); KoreanFont();
            Set(Find("GameManager").GetComponent<GameManager>(),"skipTitle",false);
            var canvas=Component<Canvas>("UI_Canvas"); canvas.renderMode=RenderMode.ScreenSpaceOverlay;
            var scaler=canvas.gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution=new Vector2(1920,1080); scaler.matchWidthOrHeight=.5f;
            canvas.gameObject.AddComponent<GraphicRaycaster>(); var ui=canvas.gameObject.AddComponent<UIManager>();
            var title=Panel("TitlePanel",canvas.transform,new Vector2(1920,1080),Vector2.zero);
            Label("Title",title.transform,"MagicEscape",new Vector2(900,100),new Vector2(0,220),64);
            Label("Subtitle",title.transform,"마법학교 방탈출",new Vector2(900,100),new Vector2(0,100),40);
            Button("Button_Start",title.transform,"시작",new Vector2(0,-50),ui.OnClickStart); Button("Button_Quit",title.transform,"종료",new Vector2(0,-150),ui.OnClickQuit);
            var hud=Rect("HUDPanel",canvas.transform,new Vector2(1920,1080),Vector2.zero).gameObject;
            var cross=Rect("Crosshair",hud.transform,new Vector2(8,8),Vector2.zero).gameObject.AddComponent<Image>(); cross.raycastTarget=false;
            Set(ui,"promptText",Label("PromptText",hud.transform,"",new Vector2(1100,55),new Vector2(0,-60),28));
            var toast=Panel("ToastRoot",hud.transform,new Vector2(1500,100),new Vector2(0,420)); Set(ui,"toastRoot",toast); Set(ui,"toastText",Label("ToastText",toast.transform,"",new Vector2(1460,95),Vector2.zero));
            var pause=Panel("PausePanel",canvas.transform,new Vector2(1920,1080),Vector2.zero); Label("PauseTitle",pause.transform,"일시정지",new Vector2(600,80),new Vector2(0,220),48);
            Button("Button_Resume",pause.transform,"계속하기",new Vector2(0,50),ui.OnClickResume); Button("Button_Restart",pause.transform,"다시 시작",new Vector2(0,-50),ui.OnClickRestart); Button("Button_QuitPause",pause.transform,"종료",new Vector2(0,-150),ui.OnClickQuit);
            var clear=Panel("ClearPanel",canvas.transform,new Vector2(1920,1080),Vector2.zero); Label("ClearTitle",clear.transform,"탈출 성공!",new Vector2(900,90),new Vector2(0,260),48);
            Set(ui,"clearTimeText",Label("ClearTime",clear.transform,"",new Vector2(1000,70),new Vector2(0,150)));
            Set(ui,"clearMessageText",Label("ClearMessage",clear.transform,"",new Vector2(1400,100),new Vector2(0,30)));
            Button("Button_Restart2",clear.transform,"다시 시작",new Vector2(0,-100),ui.OnClickRestart); Button("Button_Quit2",clear.transform,"종료",new Vector2(0,-200),ui.OnClickQuit);
            var read=Panel("ReadPanel",canvas.transform,new Vector2(1500,700),Vector2.zero);
            Set(ui,"readTitleText",Label("ReadTitle",read.transform,"",new Vector2(1400,90),new Vector2(0,230),40)); Set(ui,"readBodyText",Label("ReadBody",read.transform,"",new Vector2(1350,350),Vector2.zero)); Label("CloseHelp",read.transform,"[E] / [ESC] 닫기",new Vector2(1200,60),new Vector2(0,-270),24);
            var objective=Panel("ObjectivePanel",canvas.transform,new Vector2(850,230),new Vector2(-500,340));
            Set(ui,"objectiveText",Label("ObjectiveText",objective.transform,"",new Vector2(800,100),new Vector2(0,50),28)); Set(ui,"hintText",Label("HintText",objective.transform,"",new Vector2(800,100),new Vector2(0,-55),24));
            foreach(var pair in new[]{("titlePanel",title),("hudPanel",hud),("pausePanel",pause),("clearPanel",clear),("readPanel",read),("objectivePanel",objective)}) Set(ui,pair.Item1,pair.Item2);
            var hotbar=Rect("Hotbar",hud.transform,new Vector2(650,72),new Vector2(0,-460)); var hot=hotbar.gameObject.AddComponent<HotbarUI>();
            var icons=new Image[8]; var frames=new GameObject[8];
            for(int i=0;i<8;i++)
            { var slot=Panel("Slot_"+(i+1),hotbar,new Vector2(64,64),new Vector2((i-3.5f)*80,0)); icons[i]=Rect("Icon",slot.transform,new Vector2(46,46),Vector2.zero).gameObject.AddComponent<Image>(); icons[i].raycastTarget=false;
              frames[i]=Rect("Frame",slot.transform,new Vector2(64,64),Vector2.zero).gameObject; var frame=frames[i].AddComponent<Image>(); frame.color=new Color(1,.8f,.3f,.3f); frame.raycastTarget=false; Label("Key",slot.transform,(i+1).ToString(),new Vector2(20,20),new Vector2(-22,22),16); }
            Set(hot,"slotIcons",icons); Set(hot,"selectionFrames",frames); Set(hot,"selectedNameText",Label("SelectedName",hud.transform,"",new Vector2(900,55),new Vector2(0,-390),26));
            foreach(var g in new[]{hud,pause,clear,read,objective}) g.SetActive(false);
            Save();
        }
    }
}

