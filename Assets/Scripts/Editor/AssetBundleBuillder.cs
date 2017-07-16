using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime.Lua;

namespace Game.Editor
{
    /// <summary>
    /// 资源打包类。
    /// </summary>
    /// <remarks>封装 <see cref="UnityGameFramework.Editor.AssetBundleTools.BuildAssetBundle"/>，在打包前后给 Lua 脚本改名，以便它们能被识别为 <see cref="TextAsset"/>。</remarks>
    internal static class AssetBundleBuilder
    {
        [MenuItem("Game/Build AssetBundles")]
        private static void Run()
        {
            var luaPaths = AssetDatabase.FindAssets("l:" + EditorConst.LuaScriptAssetLabel).ToList().ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            var pathMap = ChangeFileNames(luaPaths);

            try
            {
                var buildAssetBundle = Assembly.GetExecutingAssembly().GetType("UnityGameFramework.Editor.AssetBundleTools.BuildAssetBundle")
                    .GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static);
                buildAssetBundle.Invoke(null, null);
            }
            finally
            {
                RevertFileNames(pathMap);
            }
        }

        private static bool AssetIsLuaScript(string assetPath)
        {
            var obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            var labels = new HashSet<string>(AssetDatabase.GetLabels(obj));
            return labels.Contains(EditorConst.LuaScriptAssetLabel);
        }

        private static void RevertFileNames(IDictionary<string, string> pathMap)
        {
            foreach (var kv in pathMap)
            {
                File.Move(kv.Key, kv.Value);
            }

            AssetDatabase.Refresh();
        }

        private static IDictionary<string, string> ChangeFileNames(IEnumerable<string> luaPaths)
        {
            var pathMap = new Dictionary<string, string>();
            foreach (string path in luaPaths)
            {
                var newPath = path + LuaComponent.LuaAssetExtInBundle;
                File.Move(path, newPath);
                pathMap.Add(newPath, path);
                File.Move(path + ".meta", newPath + ".meta");
                pathMap.Add(newPath + ".meta", path + ".meta");
            }

            AssetDatabase.Refresh();
            return pathMap;
        }
    }
}
