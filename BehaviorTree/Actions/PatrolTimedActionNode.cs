using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Com.AbsurdAngle.Epochrypha
{
    public class PatrolTimedActionNode : TimedActionNode
    {

        float m_stopDistance;
        Vector3 m_targetPos;

        public PatrolTimedActionNode(float stopDistance, float time) : base("PatrolTimedActionNode", time)
        {
            m_stopDistance = stopDistance;
        }

        public PatrolTimedActionNode(PatrolTimedActionNode copy) : base(copy)
        {
            m_stopDistance = copy.m_stopDistance;
        }

        public override BehaviorNode Clone()
        {
            return new PatrolTimedActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            ClearNavmesh();

            m_targetPos = (Vector3)Agent.AIBlackboard.Board[BbKey.v3_InvalidPosition];
            Agent.AIBlackboard.Board[BbKey.v3_CorrectionTargetPosition] = m_targetPos;
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            ClearNavmesh();

            // find a random position on the NavMesh within distance of 10
            Vector3 randomDirection = Random.insideUnitSphere * 50.0f;
            randomDirection += Agent.transform.position;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, 50.0f, NavMesh.AllAreas))
            {
                m_targetPos = navHit.position;
                Agent.AIBlackboard.Board[BbKey.v3_CorrectionTargetPosition] = m_targetPos;
                SetDestination(m_targetPos);
                Agent.ArrivedCurrentDestination = false;
                return true;
            }
            return false;

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

                if (Agent.NavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    Reset();
                    return BehaviorStatus.Failed;
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

            bool result = Agent.NavMeshAgent.SetDestination(destination);

            return result;
        }
    }
}

