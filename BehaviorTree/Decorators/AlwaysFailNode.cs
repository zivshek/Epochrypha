using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class AlwaysFailNode : DecoratorNode
    {
        public AlwaysFailNode(BehaviorNode childNode) : base("alwaysFail" + childNode.Name, childNode)
        {
        }

        public override BehaviorNode Clone()
        {
            return new AlwaysFailNode(this);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            BehaviorStatus result = m_childNode.UpdateBehavior(depth + 1);

            if (result == BehaviorStatus.Success)
                return BehaviorStatus.Failed;

            return result;
        }

    }
}