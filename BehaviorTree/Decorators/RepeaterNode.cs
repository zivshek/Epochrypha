using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class RepeaterNode : DecoratorNode
    {
        public RepeaterNode(BehaviorNode childNode)
            : base("RepeatUntilFail: " + childNode.Name, childNode)
        {
        }

        public override BehaviorNode Clone()
        {
            return new RepeaterNode(this);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            BehaviorStatus result = m_childNode.UpdateBehavior(depth + 1);

            switch (result)
            {
                case BehaviorStatus.Running:
                    return BehaviorStatus.Running;

                case BehaviorStatus.Failed:
                    return BehaviorStatus.Failed;

                case BehaviorStatus.Success:
                    return BehaviorStatus.Running;

                default:
                    DebugUtils.LogError("Invalid behaviour result: {0}", result);
                    break;
            }

            return BehaviorStatus.Failed;
        }
    }
}
