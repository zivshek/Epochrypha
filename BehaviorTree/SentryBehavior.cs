using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class SentryBehavior : MonoBehaviour, BehaviorCreator
    {
        public BehaviorNode CreateBehavior(BaseEnemy enemy)
        {
            var canSeePlayerConditional = new ConditionalNode(
                    new SequenceNode(
                            "handleAttackingPlayer",
                            new AimTargetActionNode(),
                            new ConditionalNode(
                                new CastSpellTimedActionNode(List_Spells.SPELL_GRENADE, 5.0f),
                                Pairing.Of(BbKey.b_AttackSpellAvailable, true)
                            )
                        ),
                    Pairing.Of(BbKey.b_CanSeePlayer, true)
                );

            var cantSeePlayerConditional = new ConditionalNode(
                    new SequenceNode(
                        "cantSeePlayerSequence",
                        new AimRandomDirectionActionNode(),
                        new TimedActionNode("idle", 3.0f)
                        ),
                    Pairing.Of(BbKey.b_CanSeePlayer, false)
                );

            var rootSelector = new SelectorNode(
                    "rootSelector",
                    canSeePlayerConditional,
                    cantSeePlayerConditional
                );

            var rootConditional = new ConditionalNode(
                    rootSelector,
                    Pairing.Of(BbKey.b_Enabled, true)
                );

            return rootConditional;
        }
    }
}
