using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public abstract class BehaviorNode
    { 
        public string Name { get; private set; }
        public BehaviorTree OwningTree { get; private set; }
        public BehaviorNode Parent { get; private set; }
        public BaseEnemy Agent { get; private set; }

        protected bool m_firstEntered = false;

        public virtual bool OnEnterNode()
        {
            m_firstEntered = true;
            return true;
        }
        public abstract BehaviorStatus UpdateBehavior(int depth);
        public virtual void OnCompleteNode() { }

        public BehaviorNode(string name)
        {
            Name = name;
        }

        public BehaviorNode(BehaviorNode copy)
        {
            Name = copy.Name;
        }

        public abstract BehaviorNode Clone();

        public virtual void Init(BehaviorTree owningTree, BehaviorNode parent, BaseEnemy agent)
        {
            OwningTree = owningTree;
            Parent = parent;
            Agent = agent;
        }

        public virtual void Reset()
        {
            m_firstEntered = false;
        }
        
        public void ClearNavmesh()
        {
            //Agent.NavMeshAgent.isStopped = true;
            //Agent.NavMeshAgent.ResetPath();
        }
    }
}
