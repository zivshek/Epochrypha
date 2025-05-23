using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class ConditionalNode : DecoratorNode
    {
        protected List<KeyValuePair<BbKey, bool>> m_conditions = new List<KeyValuePair<BbKey, bool>>();

        public ConditionalNode(BehaviorNode childNode, params KeyValuePair<BbKey, bool>[] conditions) : base("conditional", childNode)
        {
            foreach (var c in conditions)
            {
                m_conditions.Add(c);
            }
        }

        public override BehaviorNode Clone()
        {
            return new ConditionalNode(this);
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            // if any of the conditions isn't met, don't bother updating the child at all
            foreach(var c in m_conditions)
            {
                if (Agent.AIBlackboard.Board.ContainsKey(c.Key))
                {
                    if ((bool)Agent.AIBlackboard.Board[c.Key] != c.Value)
                    {
                        base.Reset();
                        m_childNode.Reset();
                        return BehaviorStatus.Failed;
                    }
                }
            }

            return m_childNode.UpdateBehavior(depth + 1);
        }
    }
}