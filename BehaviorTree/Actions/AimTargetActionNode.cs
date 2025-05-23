using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Com.AbsurdAngle.Epochrypha
{
    public class AimTargetActionNode : BehaviorNode
    {
        Quaternion m_targetRotation = Quaternion.identity;
        Sentry m_sentry;

        public AimTargetActionNode()
            : base("AimTargetActionNode")
        {
        }

        public AimTargetActionNode(AimTargetActionNode copy)
            : base(copy)
        {
        }

        public override BehaviorNode Clone()
        {
            return new AimTargetActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            m_sentry = Agent.GetComponent<Sentry>();

            return true;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (!m_firstEntered)
                OnEnterNode();

            Agent.AIBlackboard.Board[BbKey.b_Alerted] = true;

            var target = (GameObject)Agent.AIBlackboard.Board[BbKey.go_Target];

            // Make sure the farther away the player is, the higher we are gonna aim
            var distance = Vector3.Distance(target.transform.position, Agent.transform.position);
            var sight = (float)Agent.AIBlackboard.Board[BbKey.f_Sight];




            // we basically map the distance into a range from [-1, 10]
            var offsetY = MathUtils.Map(distance, 0, sight, -1, 10.0f);

            //Distance range is from 1-SightDist 
            //At the moment sight distance is 25, so if the player is 35 unity away we go to our MAX rotation which will be 50 degrees
            //if player is within 1 unit we go to our MIN of 15 degrees
            float percentDist = distance / sight;
            float angleX = Mathf.Lerp(15.0f, 50.0f, percentDist);
           // Debug.Log("Angle x: " + angleX);


            var targetDir = target.transform.position - Agent.transform.position;
            targetDir.Normalize();
            m_targetRotation = Quaternion.LookRotation(targetDir);
            m_targetRotation.eulerAngles = new Vector3(90 - angleX, m_targetRotation.eulerAngles.y, m_targetRotation.eulerAngles.z);

            // Since the turret's orientation is Vector3.up instead of Vector3.forward
            // I'll have to rotate it on x -90 degrees before doing any calculation
            var selfRotation = m_sentry.Turret.transform.rotation * Quaternion.Euler(Vector3.zero);

            float diff = Vector3.Distance(selfRotation.eulerAngles, m_targetRotation.eulerAngles);

            if (selfRotation != m_targetRotation && diff > 2.5f)
            {
                selfRotation = Quaternion.RotateTowards(selfRotation, 
                                                        m_targetRotation, 
                                                        Agent.m_baseEnemyStats.m_BaseSpeed * Time.deltaTime);

                // Once the calculations are done, we need to rotate it back, so it looks correct
                selfRotation *= Quaternion.Euler(Vector3.zero);
                m_sentry.Turret.transform.rotation = selfRotation;
                return BehaviorStatus.Running;
            }

            // Do not need to reset, cause we don't need to get the Sentry script ever again.
            // Reset();
            return BehaviorStatus.Success;
        }
    }
}