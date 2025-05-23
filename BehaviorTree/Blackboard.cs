using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.AbsurdAngle.Epochrypha
{
    [System.Serializable]
    public enum BbKey
    {
        // prefixes mean types, eg. v3 means vector3, go means gameobject
        // conditions
        b_Enabled,
        b_Static,
        b_CanSeePlayer,
        b_Alerted,
        b_CanMeleeAttack,
        b_Attacked,
        b_ShouldFlee,
        b_AttackSpellAvailable,
        b_DefensiveSpellAvailable,
        b_TargetInSpellRange,
        b_TooClose,
        b_SelectedIntermediatePoint,

        // data
        f_Sight,
        v3_InvalidPosition,
        v3_IntermediatePosition,
        v3_PlayerLastPosition,
        v3_CorrectionTargetPosition,
        go_Target,
        i_NextWaypoint,
        i_UpdateCount
    }

    public static class Pairing
    {
        public static KeyValuePair<BbKey, bool> Of(BbKey key, bool value)
        {
            return new KeyValuePair<BbKey, bool>(key, value);
        }
    }

    [System.Serializable]
    public class Blackboard
    {
        public Dictionary<BbKey, object> Board;
        public bool m_initialized = false;

        //public EnemyState CurrentState { get; private set; }

        private BaseEnemy m_Agent;
        private Vector3 m_invalidPos = new Vector3(-10000.0f, -10000.0f, -10000.0f);

        private int m_lastUpdateCount = 0;

        public Blackboard(BaseEnemy agent)
        {
            Init(agent);
        }

        // Use this for initialization
        public void Init(BaseEnemy agent)
        {
            m_Agent = agent;
            Board = new Dictionary<BbKey, object>();
            // initialize some initial values on the AI blackboard
            Board.Add(BbKey.b_Enabled, true);
            Board.Add(BbKey.b_Static, false);
            Board.Add(BbKey.b_CanSeePlayer, false);
            Board.Add(BbKey.b_Alerted, false);
            Board.Add(BbKey.b_CanMeleeAttack, true);
            Board.Add(BbKey.b_Attacked, false);
            Board.Add(BbKey.b_ShouldFlee, false);
            Board.Add(BbKey.b_AttackSpellAvailable, false);
            Board.Add(BbKey.b_DefensiveSpellAvailable, false);
            Board.Add(BbKey.b_TargetInSpellRange, false);
            Board.Add(BbKey.b_TooClose, false);
            Board.Add(BbKey.b_SelectedIntermediatePoint, false);
            Board.Add(BbKey.f_Sight, m_Agent.m_baseEnemyData.m_sightDistance);
            Board.Add(BbKey.v3_InvalidPosition, m_invalidPos);
            Board.Add(BbKey.v3_IntermediatePosition, m_invalidPos);
            Board.Add(BbKey.v3_PlayerLastPosition, m_invalidPos);
            Board.Add(BbKey.v3_CorrectionTargetPosition, m_invalidPos);
            Board.Add(BbKey.go_Target, null);
            // if there is a waypoint, start from a random index, otherwise, -1, invalid index
            Board.Add(BbKey.i_NextWaypoint, m_Agent.WayPoints.Count > 0 ? Mathf.RoundToInt(Random.Range(0, m_Agent.WayPoints.Count - 1)) : -1);
            Board.Add(BbKey.i_UpdateCount, 0);
            m_initialized = true;
        }

        // Update is called once per frame
        public void ConditionChecks()
        {
            if (!CheckEnability())
                return;

            CheckCanSeePlayer();
            CheckFleeCondition();
        }

        bool CheckEnability()
        {
            if(Board[BbKey.b_Enabled] != null)
                Board[BbKey.b_Enabled] = !m_Agent.m_isDead;
            return !m_Agent.m_isDead;
        }

        bool CheckFleeCondition()
        {
            // for now we'll check health every frame, later it will only be checked whenever enemy's health changes
            if (m_Agent.m_baseEnemyStats.m_CurrentHealth < (m_Agent.m_baseEnemyStats.m_MaxHealthAttribute * 0.2f))
            {
                Board[BbKey.b_ShouldFlee] = true;
                return true;
            }
            Board[BbKey.b_ShouldFlee] = false;
            return false;
        }

        bool CheckCanSeePlayer()
        {
            GameObject targetObject = AIUtils.FindClosestObjectInRadius(
                m_Agent.transform.position,
                (float)Board[BbKey.f_Sight],
                (obj) => obj.tag == "Player"
                );

            if (targetObject != null)
            {
                // check if anything's blocking the sight
                Vector3 posDiff = targetObject.transform.position - m_Agent.transform.position;
                float distance = posDiff.magnitude;
                if (!Physics.Raycast(m_Agent.transform.position, posDiff / distance, distance, LayerMask.GetMask("Floor", "Wall", "Ceiling")))
                {
                    Board[BbKey.go_Target] = targetObject;
                    Board[BbKey.b_CanSeePlayer] = true;
                    Board[BbKey.b_Alerted] = true;
                    Board[BbKey.v3_PlayerLastPosition] = targetObject.transform.position;
                    if (distance < 5.0f)
                    {
                        Board[BbKey.b_TooClose] = true;
                    } else
                    {
                        Board[BbKey.b_TooClose] = false;
                    }
                    SetIntermediatePosition(10.0f);
                    return true;
                }
            }
            Board[BbKey.v3_IntermediatePosition] = m_invalidPos;
            Board[BbKey.b_SelectedIntermediatePoint] = false;
            Board[BbKey.b_CanSeePlayer] = false;
            Board[BbKey.go_Target] = null;
            return false;
        }

        public bool CheckPlayerInRange(float range)
        {
            var target = (GameObject)Board[BbKey.go_Target];
            if (target == null) return false;

            var distance = Vector3.Distance(target.transform.position, m_Agent.transform.position);
            if (distance <= range)
            {
                return true;
            }
            return false;
        }

        public void CheckSpells(List_Spells attack, List_Spells defensive = List_Spells.INVALID)
        {
            Spell spell = Manager_Game.GetInstance().GetSpellData().GetSpellFromList(m_Agent.photonView.viewID, attack);
            // check spell availability
            if (spell == null)
                return;
            if (spell.TIMER_Cooldown >= spell.ADJUSTED_CooldownTime ) 
                Board[BbKey.b_AttackSpellAvailable] = true;
            else
                Board[BbKey.b_AttackSpellAvailable] = false;

            // check if distance is within range
            
            if (spell != null)
            {
                if (CheckPlayerInRange(spell.ADJUSTED_SpellRange))
                    Board[BbKey.b_TargetInSpellRange] = true;
                else
                    Board[BbKey.b_TargetInSpellRange] = false;
            }
            else
                Board[BbKey.b_TargetInSpellRange] = false;

            if (defensive == List_Spells.INVALID)
                return;

            Spell dSpell = Manager_Game.GetInstance().GetSpellData().GetSpellFromList(m_Agent.photonView.viewID, defensive);

            if (dSpell.TIMER_Cooldown >= dSpell.ADJUSTED_CooldownTime)
                Board[BbKey.b_DefensiveSpellAvailable] = true;
            else
                Board[BbKey.b_DefensiveSpellAvailable] = false;
        }

        public void SetIntermediatePosition(float range)
        {
            if ((bool)Board[BbKey.b_SelectedIntermediatePoint])
                return;

            GameObject target = (GameObject)Board[BbKey.go_Target];

            Vector3 randomDirection = Random.insideUnitSphere * range;
            randomDirection += target.transform.position;
            UnityEngine.AI.NavMeshHit navHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, range, UnityEngine.AI.NavMesh.AllAreas))
            {
                Board[BbKey.v3_IntermediatePosition] = navHit.position;
                Board[BbKey.b_SelectedIntermediatePoint] = true;
            }
        }

        public void SetPatrolPosition(float range)
        {

        }

    }
}
