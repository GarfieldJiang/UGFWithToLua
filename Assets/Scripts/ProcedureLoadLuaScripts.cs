using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityGameFramework.Runtime.Lua;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureLoadLuaScripts : ProcedureBase
    {
        private class LuaScriptInfo
        {
            public string AssetPathPrefix;
            public string FileName;
        }

        private static readonly LuaScriptInfo[] PreloadLuaScriptsInfos = new LuaScriptInfo[]
        {
            new LuaScriptInfo { AssetPathPrefix = "Assets/LuaScripts/", FileName = "test.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "tolua.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "misc/functions.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Mathf.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Vector3.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Quaternion.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Vector2.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Vector4.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Ray.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Bounds.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/RaycastHit.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Touch.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Touch.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/LayerMask.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Plane.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Time.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "UnityEngine/Color.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "list.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "event.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "typeof.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "slot.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "System/Timer.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "System/coroutine.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "System/ValueType.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "System/Reflection/BindingFlags.lua" },
            new LuaScriptInfo { AssetPathPrefix = "Assets/ToLua/ToLua/Lua/", FileName = "misc/utf8.lua" },
        };

        private HashSet<string> m_LoadFlags = new HashSet<string>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            var luaComp = GameEntry.GetComponent<LuaComponent>();

            if (Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            {
                return;
            }

            for (int i = 0; i < PreloadLuaScriptsInfos.Length; ++i)
            {
                var info = PreloadLuaScriptsInfos[i];
                m_LoadFlags.Add(info.FileName);
                luaComp.LoadFile(info.AssetPathPrefix + info.FileName, info.FileName, OnLoadLuaScriptSuccess, OnLoadLuaScriptFailure);
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (m_LoadFlags.Count <= 0)
            {
                ChangeState<ProcedureExecLuaScripts>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            m_LoadFlags.Clear();
            base.OnLeave(procedureOwner, isShutdown);
        }

        private void OnLoadLuaScriptSuccess(string fileName)
        {
            Log.Info("Load lua script '{0}' success.", fileName);
            m_LoadFlags.Remove(fileName);
        }

        private void OnLoadLuaScriptFailure(string fileName, LoadResourceStatus status, string errorMessage)
        {
            Log.Warning("Load lua script '{0}' failure. Status is '{1}'. Error message is '{2}'.", fileName, status, errorMessage);
        }
    }
}
