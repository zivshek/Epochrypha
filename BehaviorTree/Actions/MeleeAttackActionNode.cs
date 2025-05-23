using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class MeleeAttackActionNode : BehaviorNode
    {
        public MeleeAttackActionNode() : base("MeleeAttackActionNode")
        {

        }

        public MeleeAttackActionNode(MeleeAttackActionNode copy) : base(copy)
        {

        }

        public override BehaviorNode Clone()
        {
            return new MeleeAttackActionNode(this);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            Agent.MeleeAttack();

            return BehaviorStatus.Success;
        }
    }
}
