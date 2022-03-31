using ZL.Combat;
using ZL.Control;
using ZL.Core;
using ZL.Movement;
using ZL.Saving;
using System.Collections;
using UnityEngine;

namespace ZL.Monsters
{
    public class Boar : MonoBehaviour, IAction, ISavable, ICombatant
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
        [SerializeField] private float _chargeSpeedMultiplier = 2f;
        [SerializeField] private float _minChargeDistanceToPlayer = 1f;
        [SerializeField] private float _cameraShakeMagnitude;
        [SerializeField] private float _cameraShakeDuration;


        // class hidden variables
        private float _timeSinceLastAttack = Mathf.Infinity;
        private float _timeSinceLastDamage = Mathf.Infinity;
        private bool _updateTarget = false;
        private float _damageToBeTaken = 0;
        private bool _endCharge = false;
        private bool _charge = false;
        private bool _playerWasHit = false;

        // Cached References
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Mover _mover;
        private Health _target;
        private Health _health;
        private Material _defaultMat;
        private Vector3 _targetLastKnownPosition;
        private Vector3 _targetPositionExtrapolation;
        private AudioController _audioController;

        private void Awake()
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
            BoarBehavior();
            if (_charge && !_endCharge)
                Attack();
            //else if (_endCharge)
            //    EndAttack();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TakeDamage(collision);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.tag == "Paredes")
            {
                Debug.Log("Colidiu com a Parede");
                if (_charge || _endCharge)
                {
                    Debug.Log("Colidiu com a Parede enquanto estava dando charge");
                    if (_charge)
                        _animator.SetBool("endAttack", true);

                    ShakeCamera();
                    if (!_endCharge)
                    {
                        _endCharge = true;
                        StartCoroutine("EndAttack");
                    }
                }
            }
        }

        private void UpdateTimers()
        {
            _timeSinceLastAttack += Time.deltaTime;
            _timeSinceLastDamage += Time.deltaTime;
        }

        private void BoarBehavior()
        {
            if (_target == null || _health.IsDead() || _animator.GetBool("charge") || _animator.GetBool("endAttack")) return;

            // if the target is at range and there is any special behavior active, the ai can move
            if (!GetIsInRange())
            {
                _mover.ToggleStopped(false);
                MoveToAttack();
            }
            else if (_timeSinceLastAttack > _currentWeapon.GetCooldown())
            {
                _mover.ToggleStopped(true);
                PrepareAttack();
            }
        }

        private void MoveToAttack()
        {
            ResetAttackBools();
            if (_animator.GetBool("attack"))
                _animator.SetBool("attack", false);
            if (_animator.GetBool("charge"))
                _animator.SetBool("charge", false);
            if (_animator.GetBool("endAttack"))
                _animator.SetBool("endAttack", false);
            GetComponent<AIController>().ToggleUpdateTargets(true);
            _mover.AIMove(_target.transform.position);
        }

        // prepare charge
        private void PrepareAttack()
        {
            ResetAttackBools();
            if (!_animator.GetBool("attack"))
                _animator.SetBool("attack", true);
            GetComponent<AIController>().ToggleUpdateTargets(false);
        }

        // charge
        private void Attack()
        {
            GetComponent<AIController>().ToggleUpdateTargets(false);
            float distanceToLastKnownExtrapolatedPosition = Vector2.Distance(this.transform.position, _targetPositionExtrapolation);
            float distanceToLastKnownPosition = Vector2.Distance(this.transform.position, _targetLastKnownPosition);
            float distanceToPlayerPosition = Mathf.Infinity;

            if (_target != null)
                distanceToPlayerPosition = Vector2.Distance(this.transform.position, _target.transform.position);

            // if the boar is on the player last know position he will end the attack but still move a little
            if (distanceToLastKnownPosition <= _minChargeDistanceToPlayer)
            {
                //Debug.Log("Hit the last known target position, transition to endCharge animation");
                _animator.SetBool("endAttack", true);
            }

            if (distanceToPlayerPosition <= _minChargeDistanceToPlayer)
            {
                //Debug.Log("Hit the target, transition to endCharge animation and end charge motion");

                _animator.SetBool("endAttack", true);

                _target.GetComponent<Fighter>().DamageBehavior(_weaponPoint.GetComponentInChildren<Collider2D>(),
                    _knockbackForce, _currentWeapon.GetWeaponDamage());
                if (!_endCharge)
                {
                    _endCharge = true;
                    StartCoroutine("EndAttack");
                }
            }

            // if the boar hit the player or is in the final position, end charge
            if (distanceToLastKnownExtrapolatedPosition <= _minChargeDistanceToPlayer)
            {
                //Debug.Log("Hit the target last known extrapolated position, transition to endCharge animation and end charge motion");

                _animator.SetBool("endAttack", true);

                if (!_endCharge)
                {
                    _endCharge = true;
                    EndAttackImmediate();
                }
            }

            _mover.Charge(_targetPositionExtrapolation, _chargeSpeedMultiplier);
        }

        // end charge / slide
        private void EndAttackImmediate()
        {
            GetComponent<AIController>().ToggleUpdateTargets(true);
            _mover.ToggleStopped(false);// boar can move again
            Cancel();
            _timeSinceLastAttack = 0;
        }

        private IEnumerator EndAttack()
        {
            GetComponent<AIController>().ToggleUpdateTargets(true);
            _mover.ToggleStopped(false);// boar can move again
            yield return new WaitForSeconds(0.5f);
            Cancel();
            _timeSinceLastAttack = 0;
            yield return null;
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
            Cancel();
        }

        private void KnockbackBehavior(Collider2D collision, float knockbackForce)
        {
            Vector3 knockbackVector = this.transform.position - collision.gameObject.transform.position;
            knockbackVector.Normalize();
            Vector3 resultingVector = knockbackVector * knockbackForce;
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
            if (combatTarget == null) return false;
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

        public void UpdateTargetLastKnownPosition(Vector3 targetPos)
        {
            _targetLastKnownPosition = targetPos;
            _targetPositionExtrapolation = _targetLastKnownPosition + 3 * Vector3.Normalize((_targetLastKnownPosition - this.transform.position));
        }

        public void SetTarget(Health target)
        {
            this._target = target;
        }

        public void ToggleChargeAnimation(int toggle)
        {
            if (toggle == 1)
                _animator.SetBool("charge", true);
            else
                _animator.SetBool("charge", false);
        }

        public void ToggleChargeMotion(int toggle)
        {
            if (toggle == 1)
                _charge = true;
            else
                _charge = false;
        }

        public void Cancel()
        {
            ResetAttackBools();
            ResetAnimationsBools();
        }

        public void ResetAnimationsBools()
        {
            _animator.SetBool("moving", false);
            _animator.SetBool("attack", false);
            _animator.SetBool("charge", false);
            _animator.SetBool("endAttack", false);
        }

        public void ResetAttackBools()
        {
            _charge = false;
            _endCharge = false;
        }

        private void ShakeCamera()
        {
            FindObjectOfType<CameraUtils>().ShakeCamera(_cameraShakeMagnitude, _cameraShakeDuration);
        }

        public object CaptureState()
        {
            Debug.Log("Salvar nao implementado em Boar");
            return null;
        }

        public void RestoreState(object state)
        {
            Debug.Log("Load nao implementado em Boar");
        }

        void OnDrawGizmos()
        {
            // _target Position and last known pos
            if (_target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(_targetLastKnownPosition, new Vector3(0.5f, 0.5f, 0.5f));

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(_targetPositionExtrapolation, new Vector3(0.5f, 0.5f, 0.5f));
            }

            // distance that player take damage
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _minChargeDistanceToPlayer);

            // Weapon range
            if (_currentWeapon != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, _currentWeapon.GetWeaponRange());
            }
        }
    }
}
