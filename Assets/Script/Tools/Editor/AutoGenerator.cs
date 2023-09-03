using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TsingPigSDK
{

    public class AutoGenerator : Editor
    {
        private const string fileName = "Str_Def";

        [MenuItem("我的工具/Prefab名配置 #W")]
        public static void AddressableAutoGen()
        {
            Object selectedObject = Selection.activeObject;
            if (selectedObject != null && selectedObject is GameObject)
            {
                GameObject prefabObj = (GameObject)selectedObject;

                string objectName = prefabObj.name;

                string codeLine = $"    public const string {Split(objectName)}_DATA_PATH = \"{objectName}\";";

                string scriptPath = $"Assets/Script/Configs/{fileName}.cs";

                if (!File.Exists(scriptPath))
                {
                    using (StreamWriter writer = File.CreateText(scriptPath))
                    {
                        writer.WriteLine($"public static class {fileName}");
                        writer.WriteLine("{");
                        writer.WriteLine(codeLine);
                        writer.WriteLine("}");
                        Log.Info($"创建脚本: {scriptPath}");
                        AssetDatabase.Refresh();

                        return;
                    }
                }
                string[] contexts=File.ReadAllLines(scriptPath);

                if (contexts.Contains(codeLine))
                {
                    Log.Warning($"{scriptPath} 已经存在{codeLine}");
                    return;
                }

                contexts[contexts.Length - 1] = codeLine+"\n}";
                File.WriteAllLines(scriptPath, contexts);
                Log.Info($"增加代码:{codeLine} 到 {scriptPath}");
                AssetDatabase.Refresh();
            }
            else
            {
                Log.Warning("请选择一个预制体再生成代码");
            }
        }
        
        private static string Split(string name)
        {
           return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToUpper();
        }
    }
   
}
