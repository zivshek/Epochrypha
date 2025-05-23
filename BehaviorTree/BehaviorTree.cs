using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public enum BehaviorStatus
    {
        Invalid,
        Success,
        Failed,
        Running
    }

    public class BehaviorTree
    {
        BehaviorStatus m_behaviorStatus;
        BehaviorNode m_root;
        public BaseEnemy Owner { get; private set; }

        public BehaviorTree(BaseEnemy enemy)
        {
            Owner = enemy;
        }
        public BehaviorNode Root
        {
            get { return m_root; }

            set
            {
                m_root = value;

                if (m_root != null)
                {
                    m_root.Init(this, null, Owner);
                }
            }
        }

        public BehaviorStatus UpdateBehavior()
        {
            if (m_root == null)
            {
                return BehaviorStatus.Failed;
            }

            BehaviorStatus result = m_root.UpdateBehavior(0);

            return result;
        }
    }


}
