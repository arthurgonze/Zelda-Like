using UnityEngine;

namespace ZL.Menu
{
    public class ScreenCategories : MonoBehaviour
    {
        public enum ScreenCategory
        {
            UNASSIGNED,
            SPLASH,
            CREDITS,
            GAMEOVER
        };

        [SerializeField] private ScreenCategory _category = ScreenCategory.UNASSIGNED;

        public void SetScreenCategory(ScreenCategory newCategory)
        {
            _category = newCategory;
        }

        public ScreenCategory GetScreenCategory()
        {
            return _category;
        }

    }
}