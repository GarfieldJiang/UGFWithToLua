using LuaInterface;
using UnityEngine;

namespace UnityGameFramework.Runtime.Lua
{
    /// <summary>
    /// 自定义 Lua 资源加载器。
    /// </summary>
    public class CustomLuaLoader : LuaFileUtils
    {
        public delegate bool GetScript(string fileName, out byte[] buffer);

        private GetScript m_GetScript;

        public CustomLuaLoader(GetScript getScript) : base()
        {
            m_GetScript = getScript;
        }

        public override byte[] ReadFile(string fileName)
        {
            if (Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            {
                return base.ReadFile(fileName);
            }

            if (!fileName.EndsWith(".lua"))
            {
                fileName += ".lua";
            }

            byte[] buffer;
            if (!m_GetScript(fileName, out buffer))
            {
                throw new GameFramework.GameFrameworkException(string.Format("File '{0}' not loaded.", fileName));
            }

            return buffer;
        }
    }
}
