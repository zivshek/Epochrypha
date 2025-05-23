using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class MoveToWaypointActionNode : BehaviorNode
    {

        float m_stopDistance;
        Vector3 m_targetPos;

        public MoveToWaypointActionNode(float stopDistance) : base("MoveToPos")
        {
            m_stopDistance = stopDistance;
        }

        public MoveToWaypointActionNode(MoveToWaypointActionNode copy) : base(copy)
        {
            m_stopDistance = copy.m_stopDistance;
        }

        public override BehaviorNode Clone()
        {
            return new MoveToWaypointActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            ClearNavmesh();

            m_targetPos = (Vector3)Agent.AIBlackboard.Board[BbKey.v3_InvalidPosition];
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            ClearNavmesh();

            int nextWaypoint = (int)Agent.AIBlackboard.Board[BbKey.i_NextWaypoint];

            if (nextWaypoint == -1)
                return false;

            m_targetPos = Agent.WayPoints[nextWaypoint].position;

            while (!SetDestination(m_targetPos))
            {
                if (++nextWaypoint == Agent.WayPoints.Count)
                    nextWaypoint = 0;
                m_targetPos = Agent.WayPoints[nextWaypoint].position;
            }
            Agent.ArrivedCurrentDestination = false;
            return true;
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
                    // if we succeed in arriving the current waypoint, increment the nextwaypoint on blackboard 
                    int nextWaypoint = (int)Agent.AIBlackboard.Board[BbKey.i_NextWaypoint];
                    if (++nextWaypoint == Agent.WayPoints.Count)
                        nextWaypoint = 0;

                    Agent.AIBlackboard.Board[BbKey.i_NextWaypoint] = nextWaypoint;
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

            bool result = Agent.NavMeshAgent.SetDestination(destination );

            return result;
        }
    }
}

