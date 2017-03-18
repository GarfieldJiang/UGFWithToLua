using GameFramework.Resource;
using LuaInterface;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFramework.Runtime.Lua
{
    /// <summary>
    /// Lua 组件。将 ToLua 插件集成到 UnityGameFramework 中。本类的实现参考 ToLua 中的 <see cref="LuaClient"/> 类。
    /// </summary>
    public class LuaComponent : GameFrameworkComponent
    {
        private LuaState m_LuaState = null;
        private LuaLooper m_LuaLooper = null;

        private Dictionary<string, byte[]> m_CachedLuaScripts = new Dictionary<string, byte[]>();

        [SerializeField, Tooltip("Lua script search paths relative to 'Assets/' for editor use.")]
        private string[] m_EditorSearchPaths = null;

        public delegate void OnLoadScriptSuccess(string fileName);
        public delegate void OnLoadScriptFailure(string fileName, LoadResourceStatus status, string errorMessage);

        /// <summary>
        /// 启动 Lua 虚拟机。
        /// </summary>
        public void StartLuaVM()
        {
            m_LuaState.Start();
            StartLooper();
        }

        /// <summary>
        /// 清理 Lua 虚拟机。
        /// </summary>
        /// <remarks>重启游戏时调用。</remarks>
        public void ClearLuaVM()
        {
            Deinit();
        }

        /// <summary>
        /// 加载 Lua 脚本文件。
        /// </summary>
        /// <param name="assetPath">Lua 脚本的资源路径。</param>
        /// <param name="fileName">Lua 脚本文件名。</param>
        /// <param name="onSuccess">加载成功回调。</param>
        /// <param name="onFailure">加载失败回调。</param>
        public void LoadFile(string assetPath, string fileName, OnLoadScriptSuccess onSuccess, OnLoadScriptFailure onFailure = null)
        {
            if (m_CachedLuaScripts.ContainsKey(fileName) || Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            {
                if (onSuccess != null)
                {
                    onSuccess(fileName);
                }

                return;
            }

            // Load lua script from AssetBundle.
            var innerCallbacks = new LoadAssetCallbacks(
                loadAssetSuccessCallback: OnLoadAssetSuccess,
                loadAssetFailureCallback: OnLoadAssetFailure);
            var userData = new LoadLuaScriptUserData { FileName = fileName, OnSuccess = onSuccess, OnFailure = onFailure };

            assetPath += ".bytes";
            GameEntry.GetComponent<ResourceComponent>().LoadAsset(assetPath, innerCallbacks, userData);
        }

        /// <summary>
        /// 卸载 Lua 脚本文件。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        public void UnloadFile(string fileName)
        {
            if (Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            {
                m_CachedLuaScripts.Remove(fileName);
            }
        }

        /// <summary>
        /// 执行 Lua 脚本字符串。
        /// </summary>
        /// <param name="chunk">代码块。</param>
        /// <param name="chunkName">代码块名称。</param>
        /// <returns>返回值列表。</returns>
        public object[] DoString(string chunk, string chunkName = "")
        {
            if (string.IsNullOrEmpty(chunkName))
            {
                return m_LuaState.DoString(chunk);
            }

            return m_LuaState.DoString(chunk, chunkName);
        }

        /// <summary>
        /// 执行 Lua 脚本文件。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <returns>返回值列表。</returns>
        public object[] DoFile(string fileName)
        {
            return m_LuaState.DoFile(fileName);
        }

        #region MonoBehaviour

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            Deinit();
        }

        #endregion MonoBehaviour

        private void Init()
        {
            new CustomLuaLoader(GetScriptContent);
            m_LuaState = new LuaState();
            OpenLibs();
            m_LuaState.LuaSetTop(0);
            Bind();
            AddSearchPaths();
        }

        private void Deinit()
        {
            m_CachedLuaScripts.Clear();
            if (m_LuaLooper != null)
            {
                m_LuaLooper.Destroy();
                m_LuaLooper = null;
            }

            if (m_LuaState != null)
            {
                m_LuaState.Dispose();
                m_LuaState = null;
            }
        }

        private void OpenLibs()
        {
            m_LuaState.OpenLibs(LuaDLL.luaopen_pb);
            m_LuaState.OpenLibs(LuaDLL.luaopen_struct);
            m_LuaState.OpenLibs(LuaDLL.luaopen_lpeg);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            m_LuaState.OpenLibs(LuaDLL.luaopen_bit);
#endif
        }

        private void Bind()
        {
            LuaBinder.Bind(m_LuaState);
            LuaCoroutine.Register(m_LuaState, this);
        }

        private void StartLooper()
        {
            m_LuaLooper = gameObject.AddComponent<LuaLooper>();
            m_LuaLooper.luaState = m_LuaState;
        }

        private void AddSearchPaths()
        {
            if (Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            {
                for (int i = 0; i < m_EditorSearchPaths.Length; ++i)
                {
                    m_LuaState.AddSearchPath(Utility.Path.GetCombinePath(Application.dataPath, m_EditorSearchPaths[i]));
                }
            }
        }

        private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            var myUserData = userData as LoadLuaScriptUserData;
            if (myUserData == null) return;

            if (myUserData.OnFailure != null)
            {
                myUserData.OnFailure(myUserData.FileName, status, errorMessage);
            }
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            var myUserData = userData as LoadLuaScriptUserData;
            TextAsset textAsset = asset as TextAsset;
            if (textAsset == null)
            {
                throw new GameFramework.GameFrameworkException("The loaded asset should be a text asset.");
            }

            if (!m_CachedLuaScripts.ContainsKey(myUserData.FileName))
            {
                m_CachedLuaScripts.Add(myUserData.FileName, textAsset.bytes);
            }

            if (myUserData.OnSuccess != null)
            {
                myUserData.OnSuccess(myUserData.FileName);
            }
        }

        private bool GetScriptContent(string fileName, out byte[] buffer)
        {
            try
            {
                buffer = m_CachedLuaScripts[fileName];
                return true;
            }
            catch (KeyNotFoundException)
            {
                buffer = null;
                return false;
            }
        }

        private class LoadLuaScriptUserData
        {
            public string FileName;
            public OnLoadScriptSuccess OnSuccess;
            public OnLoadScriptFailure OnFailure;
        }
    }
}
