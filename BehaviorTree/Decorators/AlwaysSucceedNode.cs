using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class AlwaysSucceedNode : DecoratorNode
    {
        public AlwaysSucceedNode(BehaviorNode childNode) : base("alwaysSuccess" + childNode.Name, childNode)
        {
        }

        public override BehaviorNode Clone() 
        {
            return new AlwaysSucceedNode(this);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            BehaviorStatus result = m_childNode.UpdateBehavior(depth + 1);

            if (result == BehaviorStatus.Failed)
                return BehaviorStatus.Success;

            return result;
        }
    }
}