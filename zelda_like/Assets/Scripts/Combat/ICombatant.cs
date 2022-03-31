using ZL.Core;
using UnityEngine;

namespace ZL.Combat
{
    public interface ICombatant
    {
        void SetTarget(Health target);
        void UpdateTargetLastKnownPosition(Vector3 targetPos);
        bool CanAttack(GameObject combatTarget);
        bool EquipWeapon(Weapon weapon);
        Health GetTarget();
    }
}