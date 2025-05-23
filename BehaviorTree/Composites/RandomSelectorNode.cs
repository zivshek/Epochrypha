using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.AbsurdAngle.Epochrypha
{
    public class RandomSelectorNode : CompositeNode
    {
        List<int> toVisit = new List<int>();
        int m_handledNodes;
        int m_currentNode;

        public RandomSelectorNode(string name)
        : base(name)
        {
            Reset();
        }

        public RandomSelectorNode(string name, params BehaviorNode[] children)
            : base(name)
        {
            AddChildren(children);
            Reset();
        }

        public RandomSelectorNode(RandomSelectorNode copy)
            : base(copy)
        {
            m_handledNodes = copy.m_handledNodes;
        }

        public override BehaviorNode Clone()
        {
            return new RandomSelectorNode(this);
        }

        public override void Reset()
        {
            base.Reset();

            m_handledNodes = 0;

            toVisit.Clear();

            for (int i = 0; i < m_children.Count; i++)
            {
                toVisit.Add(i);
            }

            m_firstEntered = false;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (!m_firstEntered)
            {
                OnEnterNode();
            }

            while (m_handledNodes < m_children.Count)
            {
                BehaviorStatus result = m_children[m_currentNode].UpdateBehavior(depth + 1);

                switch (result)
                {
                    case BehaviorStatus.Running:
                        return BehaviorStatus.Running;

                    case BehaviorStatus.Failed:
                        SelectARandomChild();
                        ++m_handledNodes;
                        break;

                    case BehaviorStatus.Success:
                        Reset();
                        return BehaviorStatus.Success;

                    default:
                        break;
                }
            }

            Reset();

            return BehaviorStatus.Failed;
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();
            SelectARandomChild();

            return true;
        }

        void SelectARandomChild()
        {
            var random = Random.Range(0, toVisit.Count);
            m_currentNode = toVisit[random];
            toVisit.RemoveAt(random);
        }
    }
}