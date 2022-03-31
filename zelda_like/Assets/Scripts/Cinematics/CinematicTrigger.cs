using ZL.Saving;
using UnityEngine;

namespace ZL.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISavable
    {
        // Start is called before the first frame update
        [SerializeField] private Cinematic _cinematic;
        [SerializeField] private bool _played = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && !_played)
            {
                _cinematic.Play();
                _played = true;
            }
        }

        public object CaptureState()
        {
            return _played;
        }

        public void RestoreState(object state)
        {
            _played = (bool)state;
        }
    }
}
