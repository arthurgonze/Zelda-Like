using ZL.Combat;
using ZL.Core;
using ZL.Movement;
using ZL.Vision;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Control
{
    public class AIController : MonoBehaviour
    {
        //[SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private float _dwellTime = 2f;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] private float _waypointTolerance = 1f;
        [SerializeField] private Weapon _defaultWeapon = null;
        //[Range(0, 1)] [SerializeField] private float _patrolSpeedFraction = 0.2f;
        private int _currentWaypointIndex = 0;
        private List<Transform> _targets = new List<Transform>();
        private bool _updateTargets = true;
        private bool _blockControl = false;
        private bool _dontMove = false;
        private bool _isFixedTarget = false;

        // Cached reference
        private Fighter _fighter;
        private GameObject _player;
        private Health _health;
        private Mover _mover;
        private FOV _enemyVision;
        private ICombatant _combatant;

        // Patrol Behavior
        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceEnteringWaypoint = Mathf.Infinity;


        private void Awake()
        {
            _fighter = this.GetComponent<Fighter>();
            _health = this.GetComponent<Health>();
            _mover = this.GetComponent<Mover>();
            _enemyVision = this.GetComponent<FOV>();
            _combatant = this.GetComponent<ICombatant>();
        }

        private void Start()
        {
            _player = GameObject.FindWithTag("Player");
            _guardPosition = this.transform.position;
            _combatant.EquipWeapon(_defaultWeapon);
        }

        private void Update()
        {
            if (_health.IsDead() || _blockControl) return;

            if (_updateTargets)
                _targets = _enemyVision.GetVisibleTargets();

            AICoreBehavior();
            UpdateTimers();
        }

        // BEHAVIORS
        private void AICoreBehavior()
        {
            Transform[] targets = _targets.ToArray();

            if (targets.Length > 0 && _combatant.CanAttack(targets[0].gameObject))
                SetTarget(targets[0].gameObject);
            else if (_dontMove) return;
            else if (_timeSinceLastSawPlayer < _suspicionTime)
                SuspicionBehaviour();
            else
                PatrolBehaviour();
        }

        private void SuspicionBehaviour()
        {
            //Debug.Log("Suspicion Behavior");
            ClearTarget();
            //GetComponent<ActionScheduler>().CancelCurrentAction();
            if (!_mover.IsStopped())
                _mover.ToggleStopped(true);
        }

        private void PatrolBehaviour()
        {
            //Debug.Log("Patrol Behavior");
            ClearTarget();
            Vector3 nextPosition = _guardPosition;
            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    CycleWaypoint();

                    _timeSinceEnteringWaypoint = 0;
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (_patrolPath == null && Vector2.Distance(this.transform.position, _guardPosition) < _waypointTolerance)
            {
                if (!_mover.IsStopped())
                    _mover.ToggleStopped(true);
                return;
            }

            if (_timeSinceEnteringWaypoint > _dwellTime)
            {
                if (_mover.IsStopped())
                    _mover.ToggleStopped(false);
                _mover.AIMove(nextPosition);
            }
            else if (!_mover.IsStopped())
                _mover.ToggleStopped(true);
        }

        // WAYPOINT CYCLING UTILS
        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector2.Distance(this.transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        // UTILS
        private void ClearTarget()
        {
            _targets.Clear();
            _mover.ClearDestinations();
            _combatant.SetTarget(null);
        }

        private void SetTarget(GameObject target)
        {
            //Debug.Log("Set Target 00");
            _timeSinceLastSawPlayer = 0;
            if (_updateTargets)
            {
                //Debug.Log("Set Target 01");
                _combatant.SetTarget(target.GetComponent<Health>());
                _combatant.UpdateTargetLastKnownPosition(target.transform.position);
            }
        }

        public void SetTarget(Health target)
        {
            //Debug.Log("Set Target 02");
            _targets.Clear();
            _targets.Add(target.transform);
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceEnteringWaypoint += Time.deltaTime;
        }

        private float DistanceToPlayer()
        {
            float distance = Vector2.Distance(this.transform.position, _player.transform.position);
            return distance;
        }

        private void FocusOnTarget(Vector2 targetPos)
        {
            Vector2 playerPos = targetPos - (Vector2)transform.position;
            float angle = Mathf.Atan2(playerPos.y, playerPos.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), 10000 * Time.deltaTime);
        }

        public void ClearTargets()
        {
            _targets.Clear();
        }

        public void SetSuspicionTime(float time)
        {
            _suspicionTime = time;
        }
        public void ToggleUpdateTargets(bool toggle)
        {
            _updateTargets = toggle;
        }

        public void ToggleBlockControl(bool toggle)
        {
            _blockControl = toggle;
        }

        public void SetGuardPosition(Vector3 newPos)
        {
            _guardPosition = newPos;
        }

        public void ToggleDontMove(bool toggle)
        {
            _dontMove = toggle;
        }

        public void ToggleFixedTarget(bool toggle)
        {
            _isFixedTarget = toggle;
        }
    }
}