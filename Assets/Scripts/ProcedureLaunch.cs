using GameFramework.Event;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureLaunch : ProcedureBase
    {
        private bool m_HasStartedInitRes = false;
        private bool m_InitResComplete = false;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_HasStartedInitRes = false;
            m_InitResComplete = false;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (Application.isEditor && GameEntry.GetComponent<BaseComponent>().EditorResourceMode)
            {
                ChangeState<ProcedureLoadLuaScripts>(procedureOwner);
                return;
            }

            if (!m_HasStartedInitRes)
            {
                m_HasStartedInitRes = true;
                GameEntry.GetComponent<EventComponent>().Subscribe(EventId.ResourceInitComplete, OnResourceInitComplete);
                GameEntry.GetComponent<ResourceComponent>().InitResources();
            }

            if (m_InitResComplete)
            {
                GameEntry.GetComponent<EventComponent>().Unsubscribe(EventId.ResourceInitComplete, OnResourceInitComplete);
                ChangeState<ProcedureLoadLuaScripts>(procedureOwner);
            }
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        private void OnResourceInitComplete(object sender, GameEventArgs e)
        {
            m_InitResComplete = true;
        }
    }
}
