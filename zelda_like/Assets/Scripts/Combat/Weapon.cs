using ZL.Core;
using UnityEngine;

namespace ZL.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public enum BreakObjects { None, Alpha, Bravo, Charlie, Delta, Echo };
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 10f;
        [SerializeField] private float _weaponManaCost = 0;
        [SerializeField] private float _cooldownTime = 1f;
        [SerializeField] private GameObject _equippedWeaponPrefab = null;
        [SerializeField] private BreakObjects _breakObjectsOfType = BreakObjects.None;
        [SerializeField] private Projectile _projectile = null;
        [SerializeField] private Bomb _bomb = null;
        [SerializeField] private Shield _shield = null;
        [SerializeField] private Animator _animatorOverride = null;

        private const string weaponName = "Weapon";

        public bool Spawn(Transform weaponPoint)
        {
            DestroyOldWeapon(weaponPoint);
            if (_equippedWeaponPrefab != null)
            {
                GameObject weapon = Instantiate(_equippedWeaponPrefab, weaponPoint);
                weapon.name = weaponName;
                WeaponSpecialBehavior weaponSpecialBehavior = weapon.GetComponent<WeaponSpecialBehavior>();
                if (weaponSpecialBehavior != null)
                {
                    weaponSpecialBehavior.SetWeapon(this);
                    weaponSpecialBehavior.SetWeaponPoint(weaponPoint);
                }
                return true;
            }
            return false;
        }

        private void DestroyOldWeapon(Transform weaponPoint)
        {
            Transform oldWeapon = weaponPoint.Find(weaponName);
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        public void LaunchProjectile(Transform launchPoint, Health target)
        {
            Projectile projectileInstance = Instantiate(_projectile, launchPoint.position, Quaternion.identity);
            projectileInstance.SetTarget(target, _weaponDamage);
        }

        public void LaunchBomb()
        {
            Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            Bomb bombInstance = Instantiate(_bomb, playerPosition, Quaternion.identity);
        }

        public void LaunchShield()
        {
            Transform playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            Vector2 weaponPoint = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).localPosition;
            float offset = 1f;

            Vector2 frontShieldSpawnPos = new Vector2(playerPos.localPosition.x + offset, playerPos.localPosition.y);
            Vector2 backShieldSpawnPos = new Vector2(playerPos.localPosition.x - offset, playerPos.localPosition.y);

            Shield frontShield = Instantiate(_shield, frontShieldSpawnPos, Quaternion.identity, playerPos);
            frontShield.SetRotationPoint(playerPos);
            Shield backShield = Instantiate(_shield, backShieldSpawnPos, Quaternion.identity, playerPos);
            backShield.GetComponentInChildren<SpriteRenderer>().flipX = true;
            backShield.SetRotationPoint(playerPos);
        }

        // CHECKERS
        public bool HasProjectile() { return _projectile != null; }
        public bool HasBomb() { return _bomb != null; }
        public bool HasShield() { return _shield != null; }

        // GETTERS
        public float GetWeaponRange() { return _weaponRange; }
        public float GetWeaponDamage() { return _weaponDamage; }
        public float GetWeaponManaCost() { return _weaponManaCost; }
        public BreakObjects GetBreakableObject() { return _breakObjectsOfType; }
        public float GetCooldown() { return _cooldownTime; }
    }
}
