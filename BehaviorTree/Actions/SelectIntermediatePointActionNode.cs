using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Com.AbsurdAngle.Epochrypha
{
    public class SelectIntermediatePointActionNode : BehaviorNode
    {
        float m_range;
        public SelectIntermediatePointActionNode(float range)
            : base("SelectIntermediatePointActionNode")
        {
            m_range = range;
        }

        public SelectIntermediatePointActionNode(SelectIntermediatePointActionNode copy)
            : base(copy)
        {
            m_range = copy.m_range;
        }

        public override BehaviorNode Clone()
        {
            return new SelectIntermediatePointActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (!PhotonNetwork.isMasterClient)
                return BehaviorStatus.Success;

            //at this point, there should be a target, but I'm gonna check it anyways
            GameObject target = (GameObject)Agent.AIBlackboard.Board[BbKey.go_Target];
            if (target == null)
                return BehaviorStatus.Failed;

            // select a random spot on the navmesh that's within the range around the player
            Vector3 randomDirection = Random.insideUnitSphere * m_range;
            randomDirection += target.transform.position;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, m_range, NavMesh.AllAreas))
            {
                Agent.AIBlackboard.Board[BbKey.v3_IntermediatePosition] = navHit.position;
                Agent.AIBlackboard.Board[BbKey.b_SelectedIntermediatePoint] = true;
                return BehaviorStatus.Success;
            }
            return BehaviorStatus.Success;

        }
    }
}