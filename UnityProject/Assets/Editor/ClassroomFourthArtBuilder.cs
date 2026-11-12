using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using Object=UnityEngine.Object;

namespace MagicEscape.EditorTools
{
    public static partial class ClassroomBuilder
    {
        const string DArt="Assets/Art/S10d/";
        const string DEvidence="../docs/validation/s10d/";
        static string GameCorpus() => string.Concat(typeof(MagicEscape.Core.GameText).GetFields(BindingFlags.Public|BindingFlags.Static).Where(f=>f.FieldType==typeof(string)).Select(f=>(string)f.GetValue(null)));
        static TMP_FontAsset OldFont(string name,string corpus)
        {
            var path="Assets/Fonts/"+name+"-SDF.asset";
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(font==null)
            {
                font=TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/"+name+".ttf"),64,9,GlyphRenderMode.SDFAA,2048,2048,AtlasPopulationMode.Dynamic,true);
                AssetDatabase.CreateAsset(font,path);AssetDatabase.AddObjectToAsset(font.material,font);
                foreach(var atlas in font.atlasTextures)AssetDatabase.AddObjectToAsset(atlas,font);
            }
            font.atlasPopulationMode=AtlasPopulationMode.Dynamic;
            var fontSettings=new SerializedObject(font);fontSettings.FindProperty("m_ClearDynamicDataOnBuild").boolValue=false;fontSettings.ApplyModifiedPropertiesWithoutUndo();
            var chars=new string(corpus.Where(c=>!char.IsControl(c)).Distinct().OrderBy(c=>c).ToArray());
            font.TryAddCharacters(chars,out string missing);
            var absent=chars.Where(c=>!font.HasCharacter(c,false,false)).ToArray();
            File.WriteAllText(DEvidence+name+"-glyph-audit.txt","Required unique characters="+chars.Length+"\nMissing="+absent.Length+"\n"+new string(absent));
            if(absent.Length>0)throw new InvalidOperationException(name+" missing glyphs: "+new string(absent));
            foreach(var atlas in font.atlasTextures)if(!AssetDatabase.Contains(atlas))AssetDatabase.AddObjectToAsset(atlas,font);
            font.atlasPopulationMode=AtlasPopulationMode.Static;
            EditorUtility.SetDirty(font);return font;
        }
        static Sprite DSprite(string file)
        {
            string path=DArt+file+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Sprite;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;
            importer.spriteBorder=file.Contains("Slot")?new Vector4(15,15,15,15):new Vector4(12,12,12,12);importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        static void OldTypographyUI()
        {
            var texts=Resources.FindObjectsOfTypeAll<TMP_Text>().Where(t=>t.gameObject.scene.IsValid()).ToArray();
            string source=string.Concat(Directory.GetFiles("Assets/Scripts","*.cs",SearchOption.AllDirectories).Select(File.ReadAllText));
            string corpus=GameCorpus()+string.Concat(texts.Select(t=>t.text))+new string(source.Where(c=>c>=0xAC00&&c<=0xD7A3).Distinct().ToArray())+new string(Enumerable.Range(32,95).Select(c=>(char)c).ToArray());
            var font=OldFont("GowunBatang-Regular",corpus);
            var pen=OldFont("NanumPenScript-Regular",MagicEscape.Core.GameText.Chalkboard);
            foreach(var t in texts){t.font=font;t.color=new Color(.84f,.78f,.62f);t.fontStyle=FontStyles.Normal;}
            var board=Find("ChalkboardLabel").GetComponent<TMP_Text>();board.font=pen;board.fontSize=1.85f;board.color=new Color(.72f,.77f,.64f);board.transform.Rotate(0,0,-1.1f,Space.Self);
            var chalk=new Material(pen.material){name="S10d Chalk dust"};chalk.SetFloat("_FaceDilate",-.08f);chalk.SetFloat("_OutlineSoftness",.1f);chalk.EnableKeyword("UNDERLAY_ON");chalk.SetColor("_UnderlayColor",new Color(.55f,.65f,.52f,.22f));chalk.SetFloat("_UnderlaySoftness",.35f);
            var chalkPath="Assets/Settings/S10d_Chalk.mat";var saved=AssetDatabase.LoadAssetAtPath<Material>(chalkPath);if(saved==null){AssetDatabase.CreateAsset(chalk,chalkPath);saved=chalk;}else{EditorUtility.CopySerialized(chalk,saved);Object.DestroyImmediate(chalk);}board.fontSharedMaterial=saved;
            var inscription=texts.First(t=>t.name=="InscriptionText");inscription.fontSize=1.08f;inscription.text=MagicEscape.Core.GameText.InscriptionRevealed.Replace(", ",",\n");inscription.color=new Color(.76f,.67f,.43f);
            // Narrow dark cut immediately behind the luminous stroke reads as incised stone.
            var cut=new Material(font.material){name="S10d Incised lettering"};cut.EnableKeyword("UNDERLAY_ON");cut.SetColor("_UnderlayColor",new Color(.025f,.018f,.009f,1));cut.SetFloat("_UnderlayOffsetX",.8f);cut.SetFloat("_UnderlayOffsetY",-.8f);cut.SetFloat("_UnderlayDilate",.22f);cut.SetFloat("_UnderlaySoftness",.08f);cut.SetFloat("_FaceDilate",-.025f);
            string cutPath="Assets/Settings/S10d_Inscription.mat";var old=AssetDatabase.LoadAssetAtPath<Material>(cutPath);if(old==null){AssetDatabase.CreateAsset(cut,cutPath);old=cut;}else{EditorUtility.CopySerialized(cut,old);Object.DestroyImmediate(cut);}inscription.fontSharedMaterial=old;
            var panel=DSprite("ClothPanel");var slot=DSprite("WoodSlot");var selection=DSprite("SelectedSlot");var dot=DSprite("Crosshair");
            foreach(var im in Resources.FindObjectsOfTypeAll<Image>().Where(i=>i.gameObject.scene.IsValid()))
            {
                if(im.name=="Icon")continue;
                if(im.name=="Crosshair"){im.sprite=dot;im.color=Color.white;im.rectTransform.sizeDelta=new Vector2(38,38);continue;}
                im.sprite=im.name=="Frame"?selection:im.name.StartsWith("Slot_")?slot:panel;im.type=Image.Type.Sliced;im.color=Color.white;
                if(im.name=="Frame") {im.raycastTarget=false;}
            }
            var prompt=texts.First(t=>t.name=="PromptText");prompt.fontSize=27;
            var credits=texts.First(t=>t.name=="ArtCredits");credits.text=credits.text.Replace("Noto Sans KR","Gowun Batang 쨌 Nanum Pen Script");credits.fontSize=17;
        }
        static Material DSurface(string name,Color color,float emission=0)
        {
            var mat=ArtMaterial("S10d_"+name,color,emission);mat.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(DArt+"Patina.png");mat.SetFloat("_Smoothness",.1f);EditorUtility.SetDirty(mat);return mat;
        }
        static void OldMaterials(Transform decor)
        {
            var stone=DSurface("WornStone",new Color(.43f,.39f,.31f),.01f);var brass=DSurface("TarnishedBrass",new Color(.5f,.34f,.13f),.01f);brass.SetFloat("_Metallic",.62f);brass.SetFloat("_Smoothness",.28f);
            // Match model atlas, masonry and timber chroma/roughness without changing their UVs.
            foreach(var id in AssetDatabase.FindAssets("t:Material",new[]{"Assets/Settings"}))
            {
                var m=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(id));
                if(m==null||m.shader.name!="Universal Render Pipeline/Lit")continue;
                if(m.name.Contains("KayKit")||m.name.Contains("Quat_")){m.color=new Color(.58f,.52f,.42f);m.SetFloat("_Smoothness",.08f);}
                if(m.name.Contains("Masonry"))m.color=new Color(.53f,.49f,.41f);
                if(m.name.Contains("Timber"))m.color=new Color(.57f,.48f,.36f);
                EditorUtility.SetDirty(m);
            }
            foreach(var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                if(r.GetComponent<TMP_Text>()!=null||!r.enabled)continue;
                var m=r.sharedMaterial;
                if(m!=null&&m.shader.name=="Universal Render Pipeline/Lit"&&m.mainTexture==null&&!r.name.Contains("SealLight")&&!r.name.Contains("Bulb")&&!r.name.Contains("Glass"))
                {m.mainTexture=stone.mainTexture;m.SetFloat("_Smoothness",.12f);EditorUtility.SetDirty(m);}
                if(r.name=="SocketCup"){r.sharedMaterial=stone;stone.SetTexture("_EmissionMap",AssetDatabase.LoadAssetAtPath<Texture2D>(DArt+"BronzeEmission.png"));EditorUtility.SetDirty(stone);}
            }
            foreach(var socket in Object.FindObjectsByType<CrystalSocket>(FindObjectsSortMode.None))
            {
                var p=socket.transform.position;
                // Layered brass bands conceal the original bright blue cylinder, with no added collider.
                foreach(float y in new[]{-.045f,.045f})CVisual("SocketBand",socket.transform,p+Vector3.up*y,new Vector3(.35f,.012f,.35f),brass,PrimitiveType.Cylinder);
            }
            foreach(string key in new[]{"Sapphire","Ruby"})
            {
                var mat=AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Art_S10c_"+key+".mat");
                mat.shader=Shader.Find("MagicEscape/OldCrystal");mat.SetColor("_BaseColor",key=="Sapphire"?new Color(.025f,.13f,.53f):new Color(.55f,.012f,.035f));mat.SetColor("_EmissionColor",key=="Sapphire"?new Color(.015f,.1f,.36f):new Color(.36f,.012f,.025f));mat.SetTexture("_BaseMap",stone.mainTexture);EditorUtility.SetDirty(mat);
            }
            // The floating pink oval was one of the three primitive SealLight spheres.
            foreach(var r in Find("ExitDoor").GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("SealLight_")))
            {
                r.enabled=false;var p=r.transform.position;
                CVisual("SealLampCasing",r.transform,p,new Vector3(.26f,.30f,.09f),brass);
                CVisual("SealLampLens",r.transform,p-new Vector3(0,0,.065f),new Vector3(.09f,.15f,.025f),DSurface("SealAmber",new Color(.55f,.19f,.025f),1.2f));
            }
            // The bottom orange mass was a book group on the centre student desk, too close to spawn.
            foreach(var t in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.parent!=null&&t.parent.name=="Art_SecondPass"&&Mathf.Abs(t.position.x)<.7f&&t.position.z<-3.3f).ToArray())t.position+=new Vector3(2.9f,0,.42f);
        }
        static void OldLivedIn(Transform decor)
        {
            var random=new System.Random(5196);var candidates=Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t=>t.parent!=null&&t.parent.name=="Art_SecondPass"&&(t.name.Contains("Fit_table_small")||t.name.Contains("Fit_chair"))).ToArray();
            foreach(var t in candidates){t.Rotate(0,(float)(random.NextDouble()*16-8),0,Space.World);if(t.name.Contains("chair"))t.position+=new Vector3(.16f,0,.24f);}
            var banner=Find("ArtB_C_AcademyBanner_-1").transform;banner.position+=new Vector3(-.42f,-.21f,0);banner.Rotate(0,0,-7,Space.Self);
            Find("ArtB_C_BannerRod_-1").transform.position+=new Vector3(-.42f,-.21f,0);Find("ArtB_C_BannerRod_-1").transform.Rotate(0,0,-7,Space.Self);
            // Bounded clutter lives only on desks/shelves or along the side walls. Centre exit lane stays empty.
            int count=0;
            for(int i=0;i<3;i++)
            {
                float x=i==0?-1.7f:i==1?3f:1.9f;float z=i==1?-3.28f:-3.7f;
                foreach(var entry in new[]{("Book_5",new Vector3(-.17f,0,.10f),new Vector3(.32f,.075f,.23f)),("Scroll_1",new Vector3(.28f,0,-.12f),new Vector3(.28f,.06f,.18f)),("Potion_2",new Vector3(.34f,0,.17f),new Vector3(.07f,.13f,.07f)),("Book_Stack_1",new Vector3(-.33f,0,-.12f),new Vector3(.2f,.17f,.16f))})
                {ModelAt("Quaternius",entry.Item1,decor,new Vector3(x,.74f,z)+entry.Item2,entry.Item3,random.Next(-18,19));count++;}
                // Wax pools and a fine feather / brass pen resting beside the open book.
                CVisual("WaxPool",decor,new Vector3(x+.1f,.748f,z-.17f),new Vector3(.10f,.006f,.07f),DSurface("OldWax",new Color(.48f,.38f,.2f)),PrimitiveType.Sphere);
                var feather=CVisual("QuillFeather",decor,new Vector3(x+.11f,.764f,z+.23f),new Vector3(.035f,.004f,.13f),DSurface("Quill",new Color(.64f,.57f,.4f)),PrimitiveType.Sphere);feather.transform.Rotate(0,32,0);
                var quill=CVisual("Quill",decor,new Vector3(x+.08f,.76f,z+.17f),new Vector3(.006f,.007f,.25f),DSurface("Quill",new Color(.64f,.57f,.4f)));quill.transform.Rotate(0,32,0);
            }
            for(int i=0;i<9;i++)
            {
                float x=(i<5?-5.38f:5.12f)+(float)(random.NextDouble()*.3-.15);float z=(i<5?-.2f+i*.59f:-2.9f+(i-5)*.62f)+(float)(random.NextDouble()*.3-.15);
                ModelAt("Quaternius","Book_Stack_1",decor,new Vector3(x,0,z),new Vector3(.34f,.23f+(i%3)*.1f,.3f),random.Next(-35,36));count++;
            }
            // Existing free Standard archive supplies actual wooden crates; never substitute tables.
            for(int i=0;i<3;i++){ModelAt("Quaternius","Crate_Wooden",decor,new Vector3(-4.5f+i*.7f,.01f,-3.9f),new Vector3(.42f,.32f,.36f),random.Next(-20,20));count++;}
            for(int i=0;i<12;i++)
            {
                float x=i<6?5.33f:-4.9f;float y=i<6?.6f+(i%3)*.63f:0;float z=i<6?3.3f+(i/3)*.32f:2.1f+(i-6)*.22f;
                ModelAt("Quaternius",i%2==0?"Scroll_2":"Book_7",decor,new Vector3(x,y,z),new Vector3(.25f,.1f,.22f),random.Next(-30,31));count++;
            }
            for(int i=0;i<3;i++){ModelAt("Quaternius","Candle_2",decor,new Vector3(i==0?-1.3f:i==1?3.25f:2.3f,.74f,i==1?-3.28f:-3.7f),new Vector3(.07f,.16f,.07f));count++;}
            var web=Surface("DWeb",DArt+"Cobweb.png",new Color(.6f,.56f,.45f));web.SetFloat("_AlphaClip",1);web.SetFloat("_Cutoff",.08f);web.EnableKeyword("_ALPHATEST_ON");web.renderQueue=2450;EditorUtility.SetDirty(web);
            foreach(var p in new[]{new Vector3(-5.77f,3.68f,3.8f),new Vector3(5.71f,3.72f,3.85f),new Vector3(-4.65f,3.7f,4.70f)})
            {var g=CVisual("Cobweb",decor,p,new Vector3(.72f,.72f,1),web,PrimitiveType.Quad);if(p.x< -5)g.transform.rotation=Quaternion.Euler(0,-90,0);else if(p.x>5)g.transform.rotation=Quaternion.Euler(0,90,0);}
            CLight("WestFloorBounce",new Vector3(-3.8f,.8f,-.4f),new Color(.56f,.43f,.26f),.75f,4.6f);
            var moon=Find("Art_WindowBeam").GetComponent<Light>();moon.intensity=7.5f;moon.range=8.5f;
            CLight("MoonFloorBounce",new Vector3(2.1f,1.25f,-1),Lunar,.65f,3.8f);
            var soot=Surface("DSoot",DArt+"Soot.png",Color.white);soot.SetFloat("_Surface",1);soot.SetFloat("_SrcBlend",5);soot.SetFloat("_DstBlend",10);soot.SetFloat("_ZWrite",0);soot.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");soot.renderQueue=3000;EditorUtility.SetDirty(soot);
            foreach(float x in new[]{-4.5f,4.5f})CVisual("WallSoot",decor,new Vector3(x,3.17f,4.73f),new Vector3(.66f,1.3f,1),soot,PrimitiveType.Quad);
            File.WriteAllText(DEvidence+"dressing-audit.txt","Added CC0 model clusters="+count+" plus 9 wax/pen/feather props, 3 cobwebs, 2 soot glazes. All mesh-only, no collision. Clear centre lane preserved.\n");
        }
        public static void S10d()
        {
            Directory.CreateDirectory(DEvidence);S10c();var before=ColliderManifest();
            var scripts=Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.GetType().Namespace=="MagicEscape").ToDictionary(c=>c,c=>EditorJsonUtility.ToJson(c));
            var decor=new GameObject("Art_FourthPass").transform;OldTypographyUI();OldMaterials(decor);OldLivedIn(decor);
            foreach(var id in AssetDatabase.FindAssets("t:Material",new[]{"Assets/Settings"})) {var m=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(id));if(m!=null&&(m.name.Contains("KayKit")||m.name.Contains("Quat_"))){m.color=new Color(.58f,.52f,.42f);m.SetFloat("_Smoothness",.08f);EditorUtility.SetDirty(m);}}
            if(!before.SequenceEqual(ColliderManifest()))throw new InvalidOperationException("S10d changed collider serialization/world transforms/layers");
            if(scripts.Any(p=>p.Value!=EditorJsonUtility.ToJson(p.Key)))throw new InvalidOperationException("S10d changed gameplay configuration/references");
            File.WriteAllText(DEvidence+"builder-audit.txt","S10d rebuilt from S10c. Existing colliders, world transforms/scales/layers and gameplay MonoBehaviour configuration/references unchanged. No added colliders. GameText/Core unchanged. Font glyph coverage is checked before save.\n");
            File.WriteAllLines(DEvidence+"scene-manifest.txt",Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(t=>Hierarchy(t)+"|"+t.position.ToString("F4")+"|"+string.Join(",",t.GetComponents<Component>().Select(c=>c.GetType().Name))).OrderBy(x=>x));
            Save();
        }
    }
}
