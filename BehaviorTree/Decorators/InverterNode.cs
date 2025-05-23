using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class InverterNode : DecoratorNode
    {

        public InverterNode(BehaviorNode childNode) : base("inverter" + childNode.Name, childNode)
        {
        }

        public override BehaviorNode Clone()
        {
            return new InverterNode(this);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            BehaviorStatus result = m_childNode.UpdateBehavior(depth + 1);

            if (result == BehaviorStatus.Failed)
                return BehaviorStatus.Success;
            else if (result == BehaviorStatus.Success)
                return BehaviorStatus.Failed;

            return result;
        }
    }
}
