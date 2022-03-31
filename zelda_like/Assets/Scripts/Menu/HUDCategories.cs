using UnityEngine;

namespace ZL.Menu
{
    public class HUDCategories : MonoBehaviour
    {
        public enum HUDCategory
        {
            UNASSIGNED,
            GOLD_TEXT,
            PLAYER_HEALTH_IMAGE,
            PLAYER_MANA_IMAGE,
            SPECIAL_WEAPON_IMAGE,
            BASIC_ATTACK_COOLDOWN_IMAGE,
            SPECIAL_ATTACK_COOLDOWN_IMAGE
        };

        [SerializeField] private HUDCategory _category = HUDCategory.UNASSIGNED;

        public void SetHUDCategory(HUDCategory newCategory)
        {
            _category = newCategory;
        }

        public HUDCategory GetHUDCategory()
        {
            return _category;
        }
    }
}