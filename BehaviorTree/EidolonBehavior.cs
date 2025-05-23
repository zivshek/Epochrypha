using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class EidolonBehavior : MonoBehaviour, BehaviorCreator
    {
        public BehaviorNode CreateBehavior(BaseEnemy enemy)
        {
            var fleeConditional = new ConditionalNode(
                new MoveAlongOppositeDirTimedActionNode(2.0f, 3.0f),
                Pairing.Of(BbKey.b_ShouldFlee, true)
                );

            var tooCloseConditional = new ConditionalNode(
                new ConditionalNode(
                    new CastSpellTimedActionNode(List_Spells.SPELL_EIDOLONDASH, 2.0f),
                    Pairing.Of(BbKey.b_DefensiveSpellAvailable, true)
                    ),
                Pairing.Of(BbKey.b_TooClose, true)
                );

            var handleConfrontingSequence = new SequenceNode(
                "handleConfrontingSequence",
                new AlwaysSucceedNode(
                    new ConditionalNode(
                        new MoveAlongOppositeDirTimedActionNode(2.0f, 5.0f)
                        )
                    ),
                new AlwaysSucceedNode(
                    new ConditionalNode(
                        new CastSpellTimedActionNode(List_Spells.SPELL_FIREBALL, 1.5f),
                        Pairing.Of(BbKey.b_ShouldFlee, false)
                        )
                    ),

                new ConditionalNode(
                    new MoveToTargetActionNode(2.0f),
                    Pairing.Of(BbKey.b_TargetInSpellRange, false)
                    )
                );

            var handleSeeingPlayerSelector = new SelectorNode(
                "handleSeeingPlayerSelector",
                fleeConditional,
                tooCloseConditional,
                handleConfrontingSequence
                );

            var canSeePlayerConditional = new ConditionalNode(
                handleSeeingPlayerSelector,
                Pairing.Of(BbKey.b_CanSeePlayer, true)
                );

            var handleNotSeeingPlayerSelector = new SelectorNode(
                "handleNotSeeingPlayerSelector",
                new ConditionalNode(
                    new SequenceNode(
                        "handleAlerted",
                        new MoveToPlayerLastPositionActionNode(2.0f),
                        new IdleTimedActionNode("idle", 4.0f)
                        ),
                    Pairing.Of(BbKey.b_Alerted, true)
                    ),
                new ConditionalNode(
                    new RandomSelectorNode(
                        "handleNotAlerted",
                        new MoveToWaypointActionNode(2.0f),
                        new IdleTimedActionNode("idle", 3.0f)
                        ),
                    Pairing.Of(BbKey.b_Alerted, false)
                    )
                );


            var cantSeePlayerConditional = new ConditionalNode(
                handleNotSeeingPlayerSelector,
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