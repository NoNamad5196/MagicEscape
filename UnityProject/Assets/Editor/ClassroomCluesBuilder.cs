using MagicEscape.Core;
using TMPro;
using UnityEngine;
namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        static TMP_Text WorldText(string name,Transform parent,string text,Vector3 pos,Vector2 size,float fontSize=3)
        {
            var t=Component<TextMeshPro>(name,parent); t.font=KoreanFont(); t.text=text; t.fontSize=fontSize; t.alignment=TextAlignmentOptions.Center; t.rectTransform.sizeDelta=size; t.transform.localPosition=pos; return t;
        }
        static InspectableObject Clue(string name,Vector3 pos,string title,string body)
        {
            var g=Box(name,null,pos,new Vector3(.1f,1.0f,1.2f)); g.layer=9;
            var c=g.AddComponent<InspectableObject>(); Set(c,"promptVerb",name=="Chalkboard"?"칠판 읽기":"액자 살펴보기"); Set(c,"title",title); Set(c,"body",body); return c;
        }
        public static void S5()
        {
            S4();
            string[] roman={"III","I","IV","II"}; string[] rune={"불","대지","빛","물"}; string[] scene={"햇볕의 열기","흙 속 씨앗","빛 속의 꽃","비"};
            string[] body={"뜨거운 햇볕이 새싹을 깨운다.","씨앗이 흙 속에서 잠든다.","빛 속에서 마침내 꽃이 핀다.","비가 내려 씨앗을 적신다."};
            for(int i=0;i<4;i++)
            {
                var c=Clue("Painting_"+roman[i],new Vector3(-5.88f,1.7f,-2.4f+i*1.6f),"씨앗의 일생 "+roman[i],body[i]);
                // Keep label outside the thin west-wall slab with unscaled text dimensions.
                var text=WorldText("PaintingLabel_"+roman[i],null,roman[i]+"  "+rune[i]+"\n"+scene[i],new Vector3(-5.79f,1.7f,-2.4f+i*1.6f),new Vector2(1.1f,.85f),2.8f); text.transform.rotation=Quaternion.Euler(0,-90,0);
            }
            Clue("Chalkboard",new Vector3(-5.88f,1.5f,-4),"칠판",GameText.Chalkboard);
            var board=WorldText("ChalkboardLabel",null,GameText.Chalkboard,new Vector3(-5.79f,1.5f,-4),new Vector2(1.2f,1),2.4f); board.transform.rotation=Quaternion.Euler(0,-90,0);
            Save();
        }
    }
}
