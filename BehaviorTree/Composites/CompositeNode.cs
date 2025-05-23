using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public abstract class CompositeNode : BehaviorNode
    {
        protected List<BehaviorNode> m_children;

        protected GameObject m_target;

        public const int InvalidChildIndex = -1;

        public CompositeNode(string name) 
            : base(name)
        {
            m_children = new List<BehaviorNode>();
        }

        public CompositeNode(CompositeNode copy)
            : base(copy)
        {
            m_children = new List<BehaviorNode>();

            foreach (BehaviorNode child in copy.m_children)
            {
                BehaviorNode clonedChild = child.Clone();

                m_children.Add(clonedChild);
            }
        }

        public override void Init(BehaviorTree owningTree, BehaviorNode parent, BaseEnemy agent)
        {
            base.Init(owningTree, parent, agent);

            foreach (BehaviorNode child in m_children)
            {
                child.Init(owningTree, this, agent);
            }
        }

        public void ClearChildren()
        {
            m_children.Clear();
        }

        public void AddChildren(params BehaviorNode[] childrenToAdd)
        {
            foreach (BehaviorNode child in childrenToAdd)
            {
                child.Init(OwningTree, this, Agent);

                m_children.Add(child);
            }
        }
    }
}
