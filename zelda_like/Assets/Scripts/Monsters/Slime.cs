using ZL.Combat;
using ZL.Control;
using ZL.Core;
using ZL.Movement;
using ZL.Saving;
using System.Collections;
using UnityEngine;

namespace ZL.Monsters
{
    public class Slime : MonoBehaviour, IAction, ISavable, ICombatant
    {
        [Header("-------- Time Variables --------")]
        [SerializeField] private float _timeBetweenDamageTaken = 1f;


        [Space(10)]
        [Header("-------- Weapon Variables --------")]
        [SerializeField] private Weapon _currentWeapon = null;
        [SerializeField] private Transform _weaponPoint = null;

        [Space(10)]
        [Header("-------- AttackBehavior Variables --------")]
        [SerializeField] private Material _flashMat;
        [SerializeField] private float _knockbackForce = 5f;

        // class hidden variables
        private float _timeSinceLastAttack = Mathf.Infinity;
        private float _timeSinceLastDamage = Mathf.Infinity;
        private float _damageToBeTaken = 0;
        private bool _canAIattack = true;

        // Cached References
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Mover _mover;
        private Health _target;
        private Health _health;
        private Material _defaultMat;
        private AudioController _audioController;

        // Use this for initialization
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _defaultMat = _spriteRenderer.material;
            _audioController = FindObjectOfType<AudioController>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTimers();
            AIMoveToAttack();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TakeDamage(collision);
        }

        private void UpdateTimers()
        {
            _timeSinceLastAttack += Time.deltaTime;
            _timeSinceLastDamage += Time.deltaTime;
        }

        private void AIMoveToAttack()
        {
            if (_target == null || !_canAIattack || _health.IsDead() || _timeSinceLastDamage < _timeBetweenDamageTaken) return;

            // if the target is at range and there is any special behavior active, the ai can move
            if (!GetIsInRange())
            {
                _mover.ToggleStopped(false);
                GetComponent<AIController>().ToggleUpdateTargets(true);
                _mover.AIMove(_target.transform.position);
            }
            else if (_timeSinceLastAttack > _currentWeapon.GetCooldown())
            {
                Debug.Log("AI is in range to Attack");
                _mover.ToggleStopped(true);
                Attack();
            }
        }

        private void Attack()
        {
            if (!_animator.GetBool("attack"))
                _animator.SetBool("attack", true);

            _timeSinceLastAttack = 0;
            float damage = (this._currentWeapon.GetWeaponDamage());
            _target.GetComponent<Fighter>().DamageBehavior(_weaponPoint.GetComponentInChildren<Collider2D>(), _knockbackForce, damage);
        }

        public bool EquipWeapon(Weapon weapon)
        {
            if (weapon == null || _currentWeapon == weapon) return false;
            _currentWeapon = weapon;
            weapon.Spawn(_weaponPoint);

            return true;
        }


        private void TakeDamage(Collider2D collision)
        {
            if (!(this.tag == "Enemy" && collision.gameObject.tag == "PlayerWeapon")) return;

            Fighter player_fighter = FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>();
            float damage = (player_fighter.GetWeaponDamage() + player_fighter.GetDamageBonus());
            DamageBehavior(collision, player_fighter.GetKnockbackForce(), damage);
        }

        public void DamageBehavior(Collider2D collision, float knockbackForce, float damage)
        {
            if (_health.IsDead()) return;

            if (_timeSinceLastDamage <= _timeBetweenDamageTaken) return;
            _damageToBeTaken += damage;

            _animator.SetTrigger("hurt");

            KnockbackBehavior(collision, knockbackForce);
            _timeSinceLastDamage = 0;
            StartCoroutine("FlashSprite");
        }

        private void KnockbackBehavior(Collider2D collision, float knockbackForce)
        {
            Vector3 knockbackVector = this.transform.position - collision.gameObject.transform.position;
            knockbackVector.Normalize();
            Vector3 resultingVector = knockbackVector * knockbackForce;
            _mover.CancelMove();
            this.GetComponent<Rigidbody2D>().AddForce(resultingVector, ForceMode2D.Impulse);
        }

        private IEnumerator FlashSprite()
        {

            while (_timeSinceLastDamage <= _timeBetweenDamageTaken)
            {
                _spriteRenderer.material = _flashMat;
                yield return new WaitForSeconds(0.1f);
                _spriteRenderer.material = _defaultMat;
                yield return new WaitForSeconds(0.1f);
            }
            _spriteRenderer.material = _defaultMat;
        }

        public void LoseHealth()
        {
            _audioController.PlayMonsterHurtSound();
            _health.TakeDamage(_damageToBeTaken);
            _damageToBeTaken = 0;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
                return false;

            Health targetHealth = combatTarget.GetComponent<Health>();
            return targetHealth != null && !targetHealth.IsDead();
        }

        public Health GetTarget()
        {
            return _target;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) < _currentWeapon.GetWeaponRange();
        }

        public void Cancel()
        {
            _animator.SetBool("moving", false);
            _animator.SetBool("attack", false);
        }

        public object CaptureState()
        {
            Debug.Log("Salvar nao implementado em Slime");
            return null;
        }

        public void RestoreState(object state)
        {
            Debug.Log("Load nao implementado em Slime");
        }

        public void SetTarget(Health target)
        {
            this._target = target;
        }

        public void UpdateTargetLastKnownPosition(Vector3 targetPos)
        {
            //Debug.Log("Why slime is needing player last known position?");
        }

        public void ToggleAIAttack(bool toggle)
        {
            _canAIattack = toggle;
        }

        public void UpdateWeaponPointPosition(Vector2 newPos)
        {
            _weaponPoint.localPosition = newPos;
        }

        public void SetKnockbackForce(float force)
        {
            _knockbackForce = force;
        }

        void OnDrawGizmos()
        {
            // Weapon range
            if (_currentWeapon != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, _currentWeapon.GetWeaponRange());
            }
        }
    }
}