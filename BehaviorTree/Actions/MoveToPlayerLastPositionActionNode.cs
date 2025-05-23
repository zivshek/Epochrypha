using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class MoveToPlayerLastPositionActionNode : BehaviorNode
    {

        float m_stopDistance;
        bool m_MovementStarted;
        bool m_setDestinationSuccessful = false;

        public MoveToPlayerLastPositionActionNode(float stopDistance) : base("MoveToPos")
        {
            m_stopDistance = stopDistance;
        }

        public MoveToPlayerLastPositionActionNode(MoveToPlayerLastPositionActionNode copy) : base(copy)
        {
            m_stopDistance = copy.m_stopDistance;
        }

        public override BehaviorNode Clone()
        {
            return new MoveToPlayerLastPositionActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            ClearNavmesh();
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            ClearNavmesh();
            var targetPos = (Vector3)Agent.AIBlackboard.Board[BbKey.v3_PlayerLastPosition];
            
            SetDestination(targetPos);
            Agent.ArrivedCurrentDestination = false;
            return true;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (!m_firstEntered)
                    OnEnterNode();

                if (!m_setDestinationSuccessful || Agent.NavMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
                {
                    Reset();
                    return BehaviorStatus.Failed;
                }

                if (Agent.NavMeshAgent.remainingDistance < m_stopDistance)
                {
                    Reset();
                    Agent.AIBlackboard.Board[BbKey.b_Alerted] = false;
                    Agent.ArrivedCurrentDestination = true;
                    return BehaviorStatus.Success;
                }

                return BehaviorStatus.Running;
            }
            else
            {
                if (Agent.ArrivedCurrentDestination)
                {
                    Agent.AIBlackboard.Board[BbKey.b_Alerted] = false;
                    return BehaviorStatus.Success;
                }
                return BehaviorStatus.Running;
            }
        }

        bool SetDestination(Vector3 destination)
        {
            //Set destination on the nav mesh agent
            Agent.NavMeshAgent.stoppingDistance = m_stopDistance;
            
            m_setDestinationSuccessful = Agent.NavMeshAgent.SetDestination(destination + Agent.m_baseEnemyData.m_playerPivotOffset);
            
            return m_setDestinationSuccessful;
        }
    }
}

