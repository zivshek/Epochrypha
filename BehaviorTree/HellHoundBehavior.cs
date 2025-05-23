using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    public class HellHoundBehavior : MonoBehaviour, BehaviorCreator
    {
        public BehaviorNode CreateBehavior(BaseEnemy enemy)
        {
            var fleeConditional = new ConditionalNode(
                new MoveAlongOppositeDirTimedActionNode(3.0f, 3.0f),
                Pairing.Of(BbKey.b_ShouldFlee, true)
                );

            var handleConfrontingSequence = new SequenceNode(
                "handleConfrontingSequence",
                new AlwaysSucceedNode(
                    new ConditionalNode(
                        new MoveAlongOppositeDirTimedActionNode(3.0f, 5.0f),
                        Pairing.Of(BbKey.b_ShouldFlee, true)
                        )
                    ),
                new AlwaysSucceedNode(
                    new SequenceNode(
                        "handleAttacking",
                        new AlwaysSucceedNode(
                            new ConditionalNode(
                                new CastSpellTimedActionNode(List_Spells.SPELL_FLAMEPILLAR, 1.5f),
                                Pairing.Of(BbKey.b_ShouldFlee, false),
                                Pairing.Of(BbKey.b_AttackSpellAvailable, true),
                                Pairing.Of(BbKey.b_TargetInSpellRange, true)
                                )
                            ),
                        new SequenceNode(
                            "handleMelee",
                            // select a intermediate point if it didn't do that,
                            // but we need this to return success so that the rest of the nodes in this sequence can execute
                            new AlwaysSucceedNode(
                                new ConditionalNode(
                                    new SelectIntermediatePointActionNode(10.0f),
                                    Pairing.Of(BbKey.b_SelectedIntermediatePoint, false)
                                    )
                                ),
                            // move to that intermediate point, but if that doesn't exist, we still 
                            // return success so the rest of the nodes in this sequence can execute
                            new AlwaysSucceedNode(
                                new ConditionalNode(
                                    new MoveToPlaceActionNode(BbKey.v3_IntermediatePosition, 1.5f),
                                    Pairing.Of(BbKey.b_SelectedIntermediatePoint, true)
                                    )
                                ),
                            // move to player
                            new ConditionalNode(
                                new MoveToTargetActionNode(3.0f),
                                Pairing.Of(BbKey.b_ShouldFlee, false)
                                ),
                            // attack player
                            new ConditionalNode(
                                new MeleeAttackActionNode(),
                                Pairing.Of(BbKey.b_CanMeleeAttack, true)
                                )
                            )
                        )
                    )
                );

            var handleSeeingPlayerSelector = new SelectorNode(
                "handleSeeingPlayerSelector",
                fleeConditional,
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
                        new MoveToPlayerLastPositionActionNode(3.0f),
                        new IdleTimedActionNode("idle", 4.0f)
                        ),
                    Pairing.Of(BbKey.b_Alerted, true)
                    ),
                new ConditionalNode(
                    new RandomSelectorNode(
                        "handleNotAlerted",
                        new SelectorNode(
                            "Patrol or Waypoint, that is a question",
                            new MoveToWaypointActionNode(3.0f),
                            new PatrolTimedActionNode(3.0f, 4.0f)
                            ),
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