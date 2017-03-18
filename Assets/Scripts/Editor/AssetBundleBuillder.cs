using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    /// <summary>
    /// 资源打包类。
    /// </summary>
    /// <remarks>封装 <see cref="UnityGameFramework.Editor.AssetBundleTools.BuildAssetBundle"/>，在打包前后给 Lua 脚本改名，以便它们能被识别为 <see cref="TextAsset"/>。</remarks>
    public static class AssetBundleBuilder
    {
        [MenuItem("Game/Build AssetBundles")]
        public static void Run()
        {
            var luaPaths = AssetDatabase.FindAssets("l:" + EditorConst.LuaScriptAssetLabel).ToList().ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            ChangeFileNames(luaPaths);

            var buildAssetBundle = Assembly.Load("UnityGameFramework.Editor").GetType("UnityGameFramework.Editor.AssetBundleTools.BuildAssetBundle")
                .GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                buildAssetBundle.Invoke(null, null);
            }
            finally
            {
                RevertFileNames(luaPaths);
            }
        }

        private static bool AssetIsLuaScript(string assetPath)
        {
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            var labels = new HashSet<string>(AssetDatabase.GetLabels(obj));
            return labels.Contains(EditorConst.LuaScriptAssetLabel);
        }

        private static void RevertFileNames(IEnumerable<string> luaPaths)
        {
            foreach (string path in luaPaths)
            {
                var filePath = Regex.Replace(path, @"^Assets", Application.dataPath);
                File.Move(filePath + ".bytes", filePath);
                File.Move(filePath + ".bytes.meta", filePath + ".meta");
                AssetDatabase.Refresh();
            }
        }

        private static void ChangeFileNames(IEnumerable<string> luaPaths)
        {
            foreach (string path in luaPaths)
            {
                var filePath = Regex.Replace(path, @"^Assets", Application.dataPath);
                Debug.LogFormat("Renaming asset: filePath is '{0}'.", filePath);
                File.Move(filePath, filePath + ".bytes");
                File.Move(filePath + ".meta", filePath + ".bytes.meta");
                AssetDatabase.Refresh();
            }
        }
    }
}
