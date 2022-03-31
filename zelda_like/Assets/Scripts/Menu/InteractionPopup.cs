using UnityEngine;

namespace ZL.Menu
{
    public class InteractionPopup : MonoBehaviour
    {
        private Animator _animator;
        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Appear()
        {
            _animator.SetBool("popup", true);
        }

        public void Disappear()
        {
            _animator.SetBool("popup", false);
        }
    }
}