using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
namespace MagicEscape.EditorTools
{
 public static partial class ClassroomBuilder
 {
        // S10f is a historical generator with writes to Classroom, fonts, shared materials and audits.
        // Warehouse reuses its construction, not its serialized side effects.
        sealed class HistoricalAssetScope : IDisposable
        {
            readonly Dictionary<string,byte[]> bytes=new Dictionary<string,byte[]>();
            public HistoricalAssetScope()
            {
                foreach(var dir in new[]{"Assets/Settings","Assets/Fonts","Assets/Prefabs","Assets/Data","Assets/Art/S10c/Icons","../docs/validation/s10","../docs/validation/s10b","../docs/validation/s10c","../docs/validation/s10d","../docs/validation/s10e","../docs/validation/s10f"})
                    if(Directory.Exists(dir))foreach(var path in Directory.GetFiles(dir,"*",SearchOption.AllDirectories))
                        if(!path.Contains("WarehouseLighting"))bytes[path]=File.ReadAllBytes(path);
                bytes[ScenePath]=File.ReadAllBytes(ScenePath);
            }
            public void Dispose()
            {
                foreach(var entry in bytes)
                    if(!File.Exists(entry.Key)||!File.ReadAllBytes(entry.Key).SequenceEqual(entry.Value))File.WriteAllBytes(entry.Key,entry.Value);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }
        }
 }
}
