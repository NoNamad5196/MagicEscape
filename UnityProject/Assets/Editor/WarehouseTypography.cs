using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Object=UnityEngine.Object;
namespace MagicEscape.EditorTools
{
    // Conservative superset: includes dynamically composed prompts, GameText and builder UI.
    // Tests read the source again, so a newly introduced syllable cannot silently become tofu.
    public static class WarehouseFontCorpus
    {
        public static bool IsKorean(char c) => (c >= '\uac00' && c <= '\ud7a3') || (c >= '\u3130' && c <= '\u318f') || (c >= '\u1100' && c <= '\u11ff');
        public static string Required()
        {
            string source=string.Concat(new[]{"Assets/Scripts","Assets/Editor"}.SelectMany(p=>Directory.GetFiles(p,"*.cs",SearchOption.AllDirectories)).Select(File.ReadAllText));
            string constants=string.Concat(typeof(MagicEscape.Core.GameText).GetFields(System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static).Where(f=>f.FieldType==typeof(string)).Select(f=>(string)f.GetValue(null)));
            string authored=constants+string.Concat(Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(t=>t.text));
            return new string((source.Where(IsKorean).Concat(authored.Where(c=>!char.IsControl(c))).Concat(Enumerable.Range(32,95).Select(c=>(char)c))).Distinct().OrderBy(c=>c).ToArray());
        }
    }
    public static partial class ClassroomBuilder
    {
        static void ConfigureWarehouseTypography()
        {
            const string path="Assets/Art/Warehouse/GowunBatang-Warehouse-SDF.asset";
            var font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if(font==null)
            {
                font=TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/GowunBatang-Regular.ttf"),64,9,GlyphRenderMode.SDFAA,4096,4096,AtlasPopulationMode.Dynamic,true);
                AssetDatabase.CreateAsset(font,path);AssetDatabase.AddObjectToAsset(font.material,font);
            }
            font.atlasPopulationMode=AtlasPopulationMode.Dynamic;
            string corpus=WarehouseFontCorpus.Required();
            font.TryAddCharacters(corpus,out string missing);
            string absent=new string(corpus.Where(c=>!font.HasCharacter(c,false,false)).ToArray());
            if(absent.Length>0)throw new InvalidOperationException("Warehouse UI glyphs missing: "+absent);
            foreach(var atlas in font.atlasTextures)if(!AssetDatabase.Contains(atlas))AssetDatabase.AddObjectToAsset(atlas,font);
            var settings=new SerializedObject(font);settings.FindProperty("m_ClearDynamicDataOnBuild").boolValue=false;settings.ApplyModifiedPropertiesWithoutUndo();
            font.atlasPopulationMode=AtlasPopulationMode.Static;EditorUtility.SetDirty(font);
            foreach(var text in Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                text.font=font;text.fontSharedMaterial=font.material;
            }
        }
    }
}
