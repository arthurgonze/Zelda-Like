using ZL.Core;
using ZL.Menu;
using ZL.Movement;
using ZL.Saving;
using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace ZL.Combat
{
    [RequireComponent(typeof(Health), typeof(Mana))]
    public class Fighter : MonoBehaviour, IAction, ISavable
    {
        [Header("-------- Time Variables --------")]
        [SerializeField] private float _timeBetweenDamageTaken = 1f;

        [Space(10)]
        [Header("-------- Weapon Variables --------")]
        [SerializeField] private Weapon _equippedWeapon = null;
        [SerializeField] private Transform _weaponPoint = null;

        [Space(10)]
        [Header("-------- Attack Variables --------")]
        [SerializeField] private Material _flashMat;
        [SerializeField] private float _bonusDamage = 0f; // From upgrade altars, serializefield is active to debug purposes only
        [SerializeField] private float _knockbackForce = 5f;

        // class hidden variables
        private float _timeSinceLastAttack = Mathf.Infinity;
        private float _timeSinceLastDamage = Mathf.Infinity;
        private float _timeSinceLastSpecialAttack = Mathf.Infinity;
        private float _damageToBeTaken = 0;
        private Material _defaultMat;
        private double _timeCalc = 0;
        private float _specialWeaponCooldown = 0;

        // Cached References
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private SpriteRenderer _spriteRenderer;
        private AudioController _audioController;
        private Mover _mover;
        private Health _health;
        private Mana _mana;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _mover = GetComponent<Mover>();
            _mana = GetComponent<Mana>();
            _health = GetComponent<Health>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _defaultMat = _spriteRenderer.material;
            _audioController = FindObjectOfType<AudioController>();
        }

        void LateUpdate()
        {
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastAttack += Time.deltaTime;
            _timeSinceLastSpecialAttack += Time.deltaTime;
            _timeSinceLastDamage += Time.deltaTime;
        }

        //************************************************ 
        //******************** ATTACK ********************
        //************************************************
        //public bool PlayerBasicAttack(Weapon weapon, double time)
        public bool PlayerBasicAttack(Weapon weapon)
        {
            if (_timeSinceLastAttack < _equippedWeapon.GetCooldown()) return false;
            //_mover.ToggleStopped(true);
            _actionScheduler.StartAction(this);

            //Debug.Log("Toggle Weapon Collider: True, 1");
            //ToggleWeaponCollider(true);

            //Debug.Log("Toggle Attack Bool: True, PlayerBasicAttack()");
            // Weapon must be by player side when attacking
            if (_weaponPoint.GetComponent<WeaponPoint>().DistanceToPlayer() > 0.2f)
                _weaponPoint.GetComponent<WeaponPoint>().TeleportToTarget();// teleport to player side

            _animator.Play("player_atk");
            ToggleAttackBool(1);
            
            //double basicAttackEnd = (Time.realtimeSinceStartup - time) * 1000;
            //Debug.Log("Basic Attack Animation Called, lag: " + basicAttackEnd.ToString("F5") + " ms");
            //ToggleWeaponAttackAnimation(1);
            //_audioController.PlayPlayerAttackSound();


            FindObjectOfType<HUDManager>().StartBasicAttackCooldownCount(_equippedWeapon.GetCooldown());
            _timeSinceLastAttack = 0;
            //double basicAttackEnd = (Time.realtimeSinceStartup - time)*1000;
            //Debug.Log("Basic Attack end, lag: " + basicAttackEnd.ToString("F5") + " ms");
            return true;
        }

        public bool PlayerSpecialAttack(Weapon weapon)
        {
            if (_timeSinceLastSpecialAttack < _specialWeaponCooldown || weapon.GetWeaponManaCost() > _mana.GetMana()) return false;
            //_mover.ToggleStopped(true);
            _specialWeaponCooldown = _equippedWeapon.GetCooldown();

            _actionScheduler.StartAction(this);
            _mana.ConsumeMana(weapon.GetWeaponManaCost());

            // check for special attack behavior
            if (_weaponPoint.GetComponent<WeaponPoint>().DistanceToPlayer() > 0.2f)
                _weaponPoint.GetComponent<WeaponPoint>().TeleportToTarget();// teleport to player side

            _weaponPoint.GetComponentInChildren<WeaponSpecialBehavior>().ToggleSpecialAttackBehaviorActive(true);
            _animator.Play("special_atk");
            _animator.SetTrigger("special_attack");
            //_audioController.PlayPlayerSkillSound();
            ToggleWeaponAttackAnimation(1);
            _timeSinceLastSpecialAttack = 0;
            return true;
        }

        public void Cancel()
        {
            _mover.Cancel();
            _weaponPoint.gameObject.GetComponentInChildren<Collider2D>().enabled = false;
        }

        //***************************************************
        //******************** BEHAVIORS ********************
        //***************************************************
        public void DamageBehavior(Collider2D collision, float knockbackForce, float damage)
        {
            if (_health.IsDead())
                Cancel();
            if (_timeSinceLastDamage <= _timeBetweenDamageTaken) return;
            _animator.SetBool("moving", false);

            _animator.SetTrigger("hurt");
            KnockbackBehavior(collision, knockbackForce);
            ResetDamageCooldown();
            StartCoroutine("FlashSprite");
            _damageToBeTaken += damage;
            if (_weaponPoint.GetComponentInChildren<WeaponSpecialBehavior>().IsSpecialAttackBehaviorActive())
                ResetSpecialAttack();
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
            //if (_timeSinceLastDamage > _timeBetweenDamageTaken)
            //{
            _spriteRenderer.material = _defaultMat;
            yield break;
            //}
        }

        private void ResetMaterial()
        {
            _spriteRenderer.material = _defaultMat;
        }

        private void ResetSpecialAttack()
        {
            ToggleAttackBool(0);

            _mover.ToggleStopped(false);
            _timeSinceLastSpecialAttack = 0;
            //_weaponPoint.GetComponentInChildren<WeaponSpecialBehavior>().ToggleSpecialAttackBehaviorActive(false);
        }

        private void KnockbackBehavior(Collider2D collision, float knockbackForce)
        {
            Vector3 knockbackVector = this.transform.position - collision.gameObject.transform.position;
            knockbackVector.Normalize();

            Vector3 resultingVector = knockbackVector * knockbackForce;
            this.GetComponent<Rigidbody2D>().AddForce(resultingVector, ForceMode2D.Impulse);
        }

        //************************************************
        //******************** UTILS *********************
        //************************************************
        void OnDrawGizmos()
        {
            // Weapon range
            if (_equippedWeapon != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, _equippedWeapon.GetWeaponRange());
            }
        }

        //*************************************************
        //******************** SETTERS ********************
        //*************************************************
        public bool EquipWeapon(Weapon weapon)
        {
            if (weapon == null || _equippedWeapon == weapon) return false;
            _equippedWeapon = weapon;
            weapon.Spawn(_weaponPoint);

            if (CompareTag("Player"))
                ToggleWeaponCollider(false);

            return true;
        }

        public void AddDamageBonus(float bonus)
        {
            _bonusDamage += bonus;
        }

        public void ResetDamageCooldown()
        {
            _timeSinceLastDamage = 0;
        }

        public void ResetTimeSinceLastAttack()
        {
            _timeSinceLastAttack = 0;
        }

        public void LoseHealth()
        {
            _audioController.PlayPlayerHurtSound();
            _health.TakeDamage(_damageToBeTaken);
            _damageToBeTaken = 0;
        }

        public void SetKnockbackForce(float value)
        {
            _knockbackForce = value;
        }

        //*************************************************
        //******************** GETTERS ********************
        //*************************************************

        public float GetWeaponDamage()
        {
            return _equippedWeapon.GetWeaponDamage();
        }

        public float GetDamageBonus()
        {
            return _bonusDamage;
        }

        public Weapon GetCurrentWeapon()
        {
            return _equippedWeapon;
        }

        public bool IsAttacking()
        {
            return _timeSinceLastAttack < _equippedWeapon.GetCooldown();
        }

        public float GetKnockbackForce()
        {
            return _knockbackForce;
        }

        //**********************************************************
        //******************** TOGGLE FUNCTIONS ********************
        //**********************************************************
        public void ToggleAttackBool(int toggle)
        {
            if (toggle == 1)
            {
                _timeCalc = Time.realtimeSinceStartup;
                Debug.Log("Attack animation begin");
                _animator.SetBool("attack", true);
            }
            else
                _animator.SetBool("attack", false);
        }

        public void AttackEnd()
        {
            _timeCalc = (Time.realtimeSinceStartup - _timeCalc) * 1000;
            Debug.Log("Attack animation end, animation duration: " + _timeCalc.ToString("F5") + "ms");
            _timeCalc = 0;
        }

        public bool ToggleWeaponCollider(bool toggle)
        {
            //Debug.Log("Toggle Weapon Collider: " + toggle);
            _weaponPoint.gameObject.GetComponentInChildren<Collider2D>().enabled = toggle;
            return true;
        }

        public void ToggleWeaponAttackAnimation(int toggle)
        {
            if (toggle == 1)
            {
                Animator weaponAnimator = _weaponPoint.gameObject.GetComponentInChildren<Animator>();
                if (!weaponAnimator.GetBool("attack"))
                    weaponAnimator.SetBool("attack", true);
            }
            else
            {
                Animator weaponAnimator = _weaponPoint.gameObject.GetComponentInChildren<Animator>();
                weaponAnimator.SetBool("attack", false);
            }
        }

        public void PlayBasicAttackSound()
        {
            _audioController.PlayPlayerAttackSound();
        }

        public void PlaySpecialAttackSound()
        {
            _audioController.PlayPlayerSkillSound();
        }

        //*******************************************************
        //******************** SAVING SYSTEM ********************
        //*******************************************************
        public object CaptureState()
        {
            return _bonusDamage;
        }

        public void RestoreState(object state)
        {
            this._bonusDamage = (float)state;
        }
    }
}
