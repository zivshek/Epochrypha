using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class ForcedSequenceNode : CompositeNode
    {
        int m_currentNodeIndex;

        public ForcedSequenceNode(string name)
         : base(name)
        {
            Reset();
        }

        public ForcedSequenceNode(string name, params BehaviorNode[] children)
            : base(name)
        {
            Reset();

            AddChildren(children);
        }

        public ForcedSequenceNode(ForcedSequenceNode copy)
            : base(copy)
        {
            m_currentNodeIndex = copy.m_currentNodeIndex;
        }

        public override BehaviorNode Clone()
        {
            return new ForcedSequenceNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            m_currentNodeIndex = 0;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            while (m_currentNodeIndex < m_children.Count)
            {
                BehaviorStatus result = m_children[m_currentNodeIndex].UpdateBehavior(depth + 1);

                ++m_currentNodeIndex;
            }

            Reset();

            return BehaviorStatus.Success;
        }
    }
}
