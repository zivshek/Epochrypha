using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class CastSpellTimedActionNode : TimedActionNode
    {
        List_Spells m_spell;

        bool m_hasCast = false;
        public CastSpellTimedActionNode(List_Spells spell, float timeToComplete) 
            :base ("CastSpellAction", timeToComplete)
        {
            m_spell = spell;
        }
        
        public CastSpellTimedActionNode(CastSpellTimedActionNode copy)
            : base(copy)
        {
            m_spell = copy.m_spell;
        }

        public override BehaviorNode Clone()
        {
            return new CastSpellTimedActionNode(this);
        }

        public override bool OnEnterNode()
        {
            base.OnEnterNode();
            m_hasCast = false;

            if (Agent.NavMeshAgent != null)
                ClearNavmesh();

            if (PhotonNetwork.isMasterClient)
            {
                m_hasCast = Agent.CastSpell(m_spell);
            }

            return true;
        }

        public override BehaviorStatus UpdateBehavior(int depth)
        {
            if (!m_firstEntered)
                OnEnterNode();

            var target = (GameObject)Agent.AIBlackboard.Board[BbKey.go_Target];

            // Making sure the Sentry won't be looking at the player instantly when it's casting a spell
            if (!(bool)Agent.AIBlackboard.Board[BbKey.b_Static])
                Agent.transform.LookAt(target.transform.position + Agent.m_baseEnemyData.m_playerPivotOffset);

            return base.UpdateBehavior(depth);
        }

        public override void OnCompleteNode()
        {
            base.OnCompleteNode();
            m_hasCast = false;
        }
    }
}