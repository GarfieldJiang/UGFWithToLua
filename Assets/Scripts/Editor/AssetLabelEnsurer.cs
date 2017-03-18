using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Game.Editor
{
    /// <summary>
    /// 用于确保 Asset 上 Label 的编辑器类。
    /// </summary>
    public static class AssetLabelEnsurer
    {

        [MenuItem("Game/Ensure Asset Labels")]
        public static void Run()
        {
            var luaPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".lua"));
            foreach (var path in luaPaths)
            {
                var obj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                var curLabels = AssetDatabase.GetLabels(obj);
                AssetDatabase.SetLabels(obj, new HashSet<string>(curLabels).Union(new string[] { EditorConst.LuaScriptAssetLabel }).ToArray());
            }
        }
    }
}
