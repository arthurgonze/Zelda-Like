using ZL.Movement;
using UnityEngine;

namespace ZL.Combat
{
    public class WeaponPoint : MonoBehaviour
    {
        [SerializeField] private float _followSpeed = 1f;
        [SerializeField] private float _xOffset = 0.4f;
        [SerializeField] private float _yOffset = 0.4f;
        [SerializeField] private GameObject _target;

        private Mover _targetMover;
        // Use this for initialization
        void Awake()
        {
            _targetMover = _target.GetComponent<Mover>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 targetPosition = GetTargetPosition();

            this.transform.position = Vector2.MoveTowards(this.transform.position,
                targetPosition,
                _followSpeed * Time.deltaTime);
        }

        private Vector3 GetTargetPosition()
        {
            Vector3 targetPosition = _target.transform.position;

            if (_targetMover.IsMovingUp())
            {
                targetPosition.y -= _yOffset;
                return targetPosition;
            }

            if (_targetMover.IsMovingDown())
            {
                targetPosition.y += _yOffset;
                return targetPosition;
            }

            if (_targetMover.IsMovingRight())
            {
                targetPosition.x -= _xOffset;
                return targetPosition;
            }

            targetPosition.x += _xOffset;
            return targetPosition;
        }

        public float DistanceToPlayer()
        {
            Vector3 targetPosition = GetTargetPosition();

            float distance = Vector2.Distance(this.transform.position, targetPosition);
            return distance;
        }

        public void TeleportToTarget()
        {
            Vector3 targetPosition = GetTargetPosition();
            this.transform.position = targetPosition;
        }
    }
}