using GameFramework.Fsm;
using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;
using UnityGameFramework.Runtime.Lua;

namespace Game
{
    public class ProcedureExecLuaScripts : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            var luaComp = GameEntry.GetComponent<LuaComponent>();
            luaComp.StartLuaVM();
            luaComp.DoFile("test.lua");
        }
    }
}
