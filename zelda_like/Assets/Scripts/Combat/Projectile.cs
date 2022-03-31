using ZL.Core;
using UnityEngine;

namespace ZL.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _projectileSpeed = 1f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] private float _targetPositionOffset = 0.1f;
        [SerializeField] private float _lifetime = 1f;
        [SerializeField] private float _knockbackForce = 300f;

        private float _damage = 0;
        [SerializeField] private Health _target = null;
        private Vector3 _initialTargetPosition;
        private float _timeSinceLaunch = Mathf.Infinity;

        // Update is called once per frame
        void Update()
        {
            _timeSinceLaunch += Time.deltaTime;
            if (_target == null) return;
            if (IsInTargetsPosition() || (_timeSinceLaunch > _lifetime))
                Destroy(gameObject);
            ProjectileBehavior();
        }

        private bool IsInTargetsPosition()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _targetPositionOffset;
        }

        public void SetTarget(Health target, float damage)
        {
            this._target = target;
            this._damage = damage;
            _initialTargetPosition = _target.transform.position;//GetAimLocation();
            _timeSinceLaunch = 0;
        }

        private void ProjectileBehavior()
        {
            //if (_isHoming)
            //{
            //    transform.LookAt(GetAimLocation());
            //}
            //else
            //{
            //    transform.LookAt(_initialTargetPosition);
            //}
            Vector3 dir = Vector3.zero;
            if (transform.rotation.z == 0)
            {
                dir = _initialTargetPosition - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            transform.Translate(Vector2.right * Time.deltaTime * _projectileSpeed);
        }

        private Vector3 GetAimLocation()
        {
            Collider2D targetCollider = _target.GetComponent<Collider2D>();
            if (targetCollider != null && targetCollider.enabled)
            {
                return _target.transform.position;// + (Vector3.up * (targetCollider.height / 2));
            }

            return _target.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //Debug.Log("Tiro detectou trigger");
            //if (other.gameObject == _target.gameObject || (other.CompareTag("Player") && this.CompareTag("EnemyWeapon")))
            if (other.gameObject == _target.gameObject)
            {
                //Debug.Log("Tiro Colidiu com Alvo");
                //_target.TakeDamage(_damage);
                _target.GetComponent<Fighter>().DamageBehavior(this.GetComponent<Collider2D>(), _knockbackForce, _damage);
                Destroy(gameObject);
            }
        }

    }
}
