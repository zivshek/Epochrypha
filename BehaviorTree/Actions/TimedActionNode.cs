using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class TimedActionNode : BehaviorNode
    {
        float m_TimeToCompleteAction;
        float m_CurrentTime;

        public TimedActionNode(string nodeName, float timeToCompleteAction)
            : base(nodeName)
        {
            m_TimeToCompleteAction = timeToCompleteAction;
            m_CurrentTime = 0.0f;
        }

        public TimedActionNode(TimedActionNode copy)
            : base(copy)
        {
            m_TimeToCompleteAction = copy.m_TimeToCompleteAction;
            m_CurrentTime = copy.m_CurrentTime;
        }

        public override BehaviorNode Clone()
        {
            return new TimedActionNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            m_CurrentTime = 0.0f;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (m_CurrentTime == 0.0f)
            {
                OnEnterNode();
            }

            m_CurrentTime += Time.deltaTime;

            if (m_CurrentTime >= m_TimeToCompleteAction)
            {
                OnCompleteNode();

                Reset();

                return BehaviorStatus.Success;
            }
            else
            {
                return BehaviorStatus.Running;
            }
        }
    }
}