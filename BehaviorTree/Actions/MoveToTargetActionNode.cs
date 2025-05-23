
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class MoveToTargetActionNode : BehaviorNode
    {
        float m_stopDistance;
        Vector3 m_targetPos;

        public MoveToTargetActionNode(float stopDistance)
            : base("MoveToObject")
        {
            m_stopDistance = stopDistance;
        }

        public MoveToTargetActionNode(MoveToTargetActionNode copy)
            : base(copy)
        {
            m_stopDistance = copy.m_stopDistance;
        }

        public override BehaviorNode Clone()
        {
            return new MoveToTargetActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            ClearNavmesh();
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            var target = (GameObject)Agent.AIBlackboard.Board[BbKey.go_Target];

            if (target == null)
                return false;

            m_targetPos = target.transform.position;
            Agent.ArrivedCurrentDestination = false;
            return SetDestination(m_targetPos);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (!m_firstEntered)
                {
                    if (!OnEnterNode())
                    {
                        Reset();
                        return BehaviorStatus.Failed;
                    }
                }

                if (!Agent.NavMeshAgent.pathPending && Agent.NavMeshAgent.remainingDistance < m_stopDistance)
                {
                    Reset();
                    Agent.ArrivedCurrentDestination = true;
                    return BehaviorStatus.Success;
                }

                return BehaviorStatus.Running;
            }
            else
            {
                if (Agent.ArrivedCurrentDestination)
                    return BehaviorStatus.Success;
                return BehaviorStatus.Running;
            }
        }

        bool SetDestination(Vector3 destination)
        {
            //Set destination on the nav mesh agent
            Agent.NavMeshAgent.stoppingDistance = m_stopDistance;

            bool result = Agent.NavMeshAgent.SetDestination(destination + Agent.m_baseEnemyData.m_playerPivotOffset);
            
            return result;
        }
    }
}
