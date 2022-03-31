using UnityEngine;

namespace ZL.Menu
{
    public class MenuCategories : MonoBehaviour
    {
        public enum MenuCategory
        {
            UNASSIGNED,
            MAIN,
            PAUSE,
            OPTIONS
        };

        [SerializeField] private MenuCategory _category = MenuCategory.UNASSIGNED;

        public void SetMenuCategory(MenuCategory newCategory)
        {
            _category = newCategory;
        }

        public MenuCategory GetMenuCategory()
        {
            return _category;
        }
    }
}