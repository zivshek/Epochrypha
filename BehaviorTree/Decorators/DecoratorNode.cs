using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public abstract class DecoratorNode : BehaviorNode
    {
        protected BehaviorNode m_childNode;
        public DecoratorNode(string name, BehaviorNode childNode) : base(name)
        {
            m_childNode = childNode;
        }

        public DecoratorNode(DecoratorNode copy) : base(copy)
        {
            m_childNode = copy.m_childNode.Clone();
        }

        public override void Init(BehaviorTree owningTree, BehaviorNode parent, BaseEnemy agent)
        {
            base.Init(owningTree, parent, agent);

            m_childNode.Init(owningTree, this, agent);
        }

        public override void Reset()
        {
            base.Reset();

            m_childNode.Reset();
        }
    }
}

