using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;

        Fighter fighter;
        Health health;
        GameObject player;
        Mover mover;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeToArriveAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;


        private void Start()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindGameObjectWithTag("Player");

            guardPosition = transform.position;
        }
        private void Update()
        {
            if (health.IsDead()) return;
            if (InAttackRangeOfRange() && fighter.CanAttack(player))
            {               
                AttackBehaviour();
            }

            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }

            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeToArriveAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeToArriveAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();               
            }
            if (timeToArriveAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition);
            }           
        }

        private bool AtWaypoint()
        {
            float distancToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distancToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }
      

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attcack(player);
        }

        private bool InAttackRangeOfRange()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
