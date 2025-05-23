using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha {
    public class SequenceNode : CompositeNode
    {
        int m_currentNodeIndex;

        public SequenceNode(string name)
         : base(name)
        {
            Reset();
        }

        public SequenceNode(string name, params BehaviorNode[] children)
            : base(name)
        {
            Reset();

            AddChildren(children);
        }

        public SequenceNode(SequenceNode copy)
            : base(copy)
        {
            m_currentNodeIndex = copy.m_currentNodeIndex;
        }

        public override BehaviorNode Clone()
        {
            return new SequenceNode(this);
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

                switch (result)
                {
                    case BehaviorStatus.Running:
                        return BehaviorStatus.Running;

                    case BehaviorStatus.Failed:
                        Reset();
                        return BehaviorStatus.Failed;

                    case BehaviorStatus.Success:
                        ++m_currentNodeIndex;
                        break;

                    default:
                        DebugUtils.LogError("Invalid Behavior result: {0}", result);
                        break;
                }
            }

            Reset();

            return BehaviorStatus.Success;
        }
    }
}
