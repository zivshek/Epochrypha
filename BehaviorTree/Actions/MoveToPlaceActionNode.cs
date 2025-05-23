
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class MoveToPlaceActionNode : BehaviorNode
    {
        float m_stopDistance;
        BbKey m_targetPosBbKey;

        public MoveToPlaceActionNode(BbKey destination, float stopDistance)
            : base("MoveToPlaceActionNode")
        {
            m_stopDistance = stopDistance;
            //Debug.Assert(Agent.AIBlackboard.Board[destination] is Vector3);
            m_targetPosBbKey = destination;
        }

        public MoveToPlaceActionNode(MoveToPlaceActionNode copy)
            : base(copy)
        {
            m_stopDistance = copy.m_stopDistance;
            m_targetPosBbKey = copy.m_targetPosBbKey;
        }

        public override BehaviorNode Clone()
        {
            return new MoveToPlaceActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            ClearNavmesh();
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            Vector3 targetPos = (Vector3)Agent.AIBlackboard.Board[m_targetPosBbKey];

            if (targetPos == (Vector3)Agent.AIBlackboard.Board[BbKey.v3_InvalidPosition])
                return false;
            Agent.ArrivedCurrentDestination = false;
            return SetDestination(targetPos);
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
            Agent.NavMeshAgent.stoppingDistance = m_stopDistance;
            bool result = Agent.NavMeshAgent.SetDestination(destination);
            return result;
        }
    }
}
