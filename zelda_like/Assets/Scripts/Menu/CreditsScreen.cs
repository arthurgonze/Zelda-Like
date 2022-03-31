using UnityEngine;

namespace ZL.Menu
{
    public class CreditsScreen : MonoBehaviour
    {
        [SerializeField] RectTransform _credits;

        [SerializeField] float _speed = 60;
        [SerializeField] float _maxVerticalPos = 2200;

        [SerializeField] private bool _moveUp = false;
        private Vector2 _initialPos;

        private void Awake()
        {
            _initialPos = _credits.anchoredPosition;
        }

        // Update is called once per frame
        void Update()
        {
            if (!_moveUp) return;
            _credits.anchoredPosition += Vector2.up * _speed * Time.unscaledDeltaTime;
            //Debug.Log("Credits rolling");
            if (_credits.anchoredPosition.y > _maxVerticalPos || Input.GetButtonUp("Cancel"))
                EndCredits();
        }

        private void EndCredits()
        {
            ToggleMoveUp(false);
            FindObjectOfType<MenuManager>().ToggleCreditsScreen();
            ResetPos();
        }

        private void ResetPos()
        {
            _credits.anchoredPosition = _initialPos;
        }

        public void ToggleMoveUp(bool toggle)
        {
            _moveUp = toggle;
        }

    }
}