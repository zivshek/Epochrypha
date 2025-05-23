using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class MoveAlongOppositeDirTimedActionNode : TimedActionNode
    {

        float m_stopDistance;

        GameObject m_TargetPlayer = null;
        Vector3 m_TargetDirection;

        public MoveAlongOppositeDirTimedActionNode(float stopDistance, float time) : base("MoveAlongOppositeDirTimedActionNode", time)
        {
            m_stopDistance = stopDistance;
        }

        public MoveAlongOppositeDirTimedActionNode(MoveAlongOppositeDirTimedActionNode copy) : base(copy)
        {
            m_stopDistance = copy.m_stopDistance;
        }

        public override BehaviorNode Clone()
        {
            return new MoveAlongOppositeDirTimedActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            ClearNavmesh();

            m_TargetPlayer = null;
        }

        public override bool OnEnterNode()
        {
            Reset();
            base.OnEnterNode();

            m_TargetPlayer = (GameObject)Agent.AIBlackboard.Board[BbKey.go_Target];

            m_TargetDirection = (Agent.transform.position - m_TargetPlayer.transform.position).normalized;

            Agent.NavMeshAgent.SetDestination(m_TargetPlayer.transform.position + m_TargetDirection * (Agent.m_baseEnemyData.m_sightDistance / Random.Range(2.0f, 3.0f)));
            Agent.ArrivedCurrentDestination = false;
            return true;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (!m_firstEntered)
                    OnEnterNode();

                if (!Agent.NavMeshAgent.pathPending && Agent.NavMeshAgent.remainingDistance < m_stopDistance)
                {
                    Reset();
                    Agent.ArrivedCurrentDestination = true;
                    return BehaviorStatus.Success;
                }

                return base.UpdateBehavior(depth);
            }
            else
            {
                if (Agent.ArrivedCurrentDestination)
                    return BehaviorStatus.Success;
                return BehaviorStatus.Running;
            }
        }
    }
}
