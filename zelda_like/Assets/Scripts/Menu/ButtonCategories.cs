using ZL.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZL.Menu
{
    public class ButtonCategories : MonoBehaviour, ISelectHandler
    {
        public enum ButtonCategory
        {
            UNASSIGNED,
            MAIN_DEFAULT,
            OPTIONS_DEFAULT,
            CLOSED_OPTIONS,
            CLOSED_CREDITS
        };

        [SerializeField] private ButtonCategory _category = ButtonCategory.UNASSIGNED;

        public void SetButtonCategory(ButtonCategory newCategory)
        {
            _category = newCategory;
        }

        public ButtonCategory GetButtonCategory()
        {
            return _category;
        }

        public void OnSelect(BaseEventData eventData)
        {
            FindObjectOfType<AudioController>().PlayHoverSound();
        }
    }
}