using ZL.Control;
using ZL.Core;
using ZL.Menu;
using ZL.Movement;
using UnityEngine;

namespace ZL.Combat
{
    public class WeaponSpecialBehavior : MonoBehaviour
    {
        public enum AttackBehavior { None, Charge, Boomerang, Escudo, Cajado };

        [SerializeField] private AttackBehavior _attackBehavior;


        private Weapon _weapon;
        private Transform _weaponPoint;
        private bool _isSpecialAttackBehaviorActive = false;
        private bool _boomerangReturning = false;

        // Update is called once per frame
        void Update()
        {
            UpdateBehaviors();
        }

        private void UpdateBehaviors()
        {
            if (!_isSpecialAttackBehaviorActive) return;
            switch (_attackBehavior)
            {
                case WeaponSpecialBehavior.AttackBehavior.Boomerang:
                    BoomerangBehavior();
                    break;
                case WeaponSpecialBehavior.AttackBehavior.Cajado:
                    StaffBehavior();
                    break;
                case WeaponSpecialBehavior.AttackBehavior.Escudo:
                    ShieldBehavior();
                    break;
                case WeaponSpecialBehavior.AttackBehavior.None:
                    //Debug.Log("Sem comportamento especial");
                    break;
                default:
                    Debug.Log("Comportamento especial invalido");
                    break;
            }
        }

        public AttackBehavior GetAttackBehavior()
        {
            return _attackBehavior;
        }

        public void ActiveSpecialBehavior()
        {
            _isSpecialAttackBehaviorActive = true;
        }

        private void ShieldBehavior()
        {
            if (!_weapon.HasShield()) return;

            _weapon.LaunchShield();
            _isSpecialAttackBehaviorActive = false;
            FindObjectOfType<HUDManager>().StartSpecialAttackCooldownCount(_weapon.GetCooldown());
            FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().ResetTimeSinceLastAttack();
        }

        private void StaffBehavior()
        {
            if (!_weapon.HasBomb()) return;

            //StaffExplosion();
            _isSpecialAttackBehaviorActive = false;
            FindObjectOfType<HUDManager>().StartSpecialAttackCooldownCount(_weapon.GetCooldown());
            FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().ResetTimeSinceLastAttack();
        }

        public void StaffExplosion()
        {
            _weapon.LaunchBomb();
            FindObjectOfType<AudioController>().PlayPlayerFireRingSound();
        }

        private void BoomerangBehavior()
        {
            Transform weapon = _weaponPoint.GetComponentInChildren<WeaponSpecialBehavior>().gameObject.transform;
            _weaponPoint.gameObject.GetComponentInChildren<Collider2D>().enabled = true;
            ToggleWeaponAttackAnimation(1);
            if (FindObjectOfType<PlayerController>().gameObject.GetComponent<Mover>().IsLookingRight())
            {
                if (!_boomerangReturning)
                {
                    Vector2 target = new Vector2(_weaponPoint.transform.position.x + _weapon.GetWeaponRange(), _weaponPoint.transform.position.y);
                    weapon.position = Vector2.Lerp(weapon.position, target, Time.deltaTime * 5f);
                    float distance = Vector2.Distance(weapon.position, target);
                    if (distance < 0.1)
                    {
                        _boomerangReturning = true;
                        FindObjectOfType<HUDManager>().StartSpecialAttackCooldownCount(_weapon.GetCooldown());
                        FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().ResetTimeSinceLastAttack();
                    }
                }
                else
                {
                    Vector2 target = new Vector2(_weaponPoint.transform.position.x, _weaponPoint.transform.position.y);
                    weapon.position = Vector2.Lerp(weapon.position, target, Time.deltaTime * 5f);
                    float distance = Vector2.Distance(weapon.position, target);

                    if (distance < 0.1)
                    {
                        //Debug.Log("Boomerang Behavior Ended");
                        ToggleWeaponAttackAnimation(0);
                        _isSpecialAttackBehaviorActive = false;
                        _boomerangReturning = false;
                    }
                }
            }
            else
            {
                if (!_boomerangReturning)
                {
                    Vector2 target = new Vector2(_weaponPoint.transform.position.x - _weapon.GetWeaponRange(), _weaponPoint.transform.position.y);
                    weapon.position = Vector2.Lerp(weapon.position, target, Time.deltaTime * 5f);

                    float distance = Vector2.Distance(weapon.position, target);
                    if (distance < 0.1)
                    {
                        _boomerangReturning = true;
                        FindObjectOfType<HUDManager>().StartSpecialAttackCooldownCount(_weapon.GetCooldown());
                        FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().ResetTimeSinceLastAttack();
                    }
                }
                else
                {
                    Vector2 target = new Vector2(_weaponPoint.transform.position.x, _weaponPoint.transform.position.y);
                    weapon.position = Vector2.Lerp(weapon.position, target, Time.deltaTime * 5f);
                    float distance = Vector2.Distance(weapon.position, target);

                    if (distance < 0.1)
                    {
                        //Debug.Log("Boomerang Behavior Ended");
                        ToggleWeaponAttackAnimation(0);
                        _isSpecialAttackBehaviorActive = false;
                        _boomerangReturning = false;
                    }
                }
            }
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

        public void ToggleSpecialAttackBehaviorActive(bool toggle)
        {
            _isSpecialAttackBehaviorActive = toggle;
        }

        public bool IsSpecialAttackBehaviorActive()
        {
            return _isSpecialAttackBehaviorActive;
        }

        //private Weapon _weapon;
        public void SetWeapon(Weapon weapon)
        {
            _weapon = weapon;
        }

        //private Transform _weaponPoint;
        public void SetWeaponPoint(Transform weaponPoint)
        {
            _weaponPoint = weaponPoint;
        }

    }
}