using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Com.AbsurdAngle.Epochrypha
{
    public class AimRandomDirectionActionNode : BehaviorNode
    {
        Quaternion m_targetRotation = Quaternion.identity;
        Sentry m_sentry;

        public AimRandomDirectionActionNode()
            : base("AimRandomDirectionActionNode")
        {
        }

        public AimRandomDirectionActionNode(AimRandomDirectionActionNode copy)
            : base(copy)
        {
        }

        public override BehaviorNode Clone()
        {
            return new AimRandomDirectionActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();

            // Makes sure the random rotation won't be going underground
            m_targetRotation = Quaternion.Euler(Random.Range(-180, 0), Random.Range(0, 360), 0);
            m_sentry = Agent.GetComponent<Sentry>();
 
            return true;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (!m_firstEntered)
                OnEnterNode();

            var selfRotation = m_sentry.Turret.transform.rotation * Quaternion.Euler(new Vector3(-90.0f, 0, 0));

            if (selfRotation != m_targetRotation)
            {
                selfRotation = Quaternion.RotateTowards(selfRotation,
                                                        m_targetRotation,
                                                        Agent.m_baseEnemyStats.m_BaseSpeed * Time.deltaTime);

                selfRotation *= Quaternion.Euler(new Vector3(90.0f, 0, 0));
                m_sentry.Turret.transform.rotation = selfRotation;
                return BehaviorStatus.Running;
            }

            Reset();
            return BehaviorStatus.Success;
        }
    }
}