using System.Collections;
using System.Collections.Generic;

namespace Com.AbsurdAngle.Epochrypha {
    public interface BehaviorCreator
    {
        BehaviorNode CreateBehavior(BaseEnemy enemy);
    }
}