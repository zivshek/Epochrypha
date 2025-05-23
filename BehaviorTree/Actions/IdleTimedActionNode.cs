using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.AbsurdAngle.Epochrypha
{
    public class IdleTimedActionNode : TimedActionNode
    {
        public IdleTimedActionNode(string name, float secs) : base(name, secs) { }

        public IdleTimedActionNode(IdleTimedActionNode copy) : base(copy)
        {
        }

        public override BehaviorNode Clone()
        {
            return new IdleTimedActionNode(this);
        }

    }
}