using System.IO;
using UnityEditor;
using UnityEngine;

namespace TsingPigSDK
{

    public class AutoGenerator : Editor
    {
        [MenuItem("我的工具/AA写入Str_Def #W")]
        public static void AddressableAutoGen()
        {
            Object selectedObject = Selection.activeObject;
            if (selectedObject != null && selectedObject is GameObject)
            {
                GameObject prefabObj = (GameObject)selectedObject;
                string objectName = prefabObj.name;

                string codeLine = $"public const string {objectName}_DATA_PATH = \"{objectName}\";";

                string scriptPath = "Assets/Script/Config/Str_Def.cs";

                if (!File.Exists(scriptPath))
                {
                    using (StreamWriter writer = File.CreateText(scriptPath))
                    {
                        writer.WriteLine("public static class Str_Def");
                        writer.WriteLine("{");
                    }
                }

                using (StreamWriter writer = File.AppendText(scriptPath))
                {
                    writer.WriteLine($"    {codeLine}");
                }

                Log.Info($"Added code: {codeLine} to {scriptPath}");
            }
            else
            {
                Log.Warning("Select a GameObject to generate code.");
            }
        }

    }
}
