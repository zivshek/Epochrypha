using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.AbsurdAngle.Epochrypha
{
    public class SelectorNode : CompositeNode {

        int m_currentNodeIndex;

        public SelectorNode(string name)
        : base(name)
        {
            Reset();
        }

        public SelectorNode(string name, params BehaviorNode[] children)
            : base(name)
        {
            Reset();

            AddChildren(children);
        }

        public SelectorNode(SelectorNode copy)
            : base(copy)
        {
            m_currentNodeIndex = copy.m_currentNodeIndex;
        }

        public override BehaviorNode Clone()
        {
            return new SelectorNode(this);
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
                        ++m_currentNodeIndex;
                        break;

                    case BehaviorStatus.Success:
                        Reset();
                        return BehaviorStatus.Success;

                    default:
                        DebugUtils.LogError("Invalid behaviour result: {0}", result);
                        break;
                }
            }

            Reset();

            return BehaviorStatus.Failed;
        }
    }
}