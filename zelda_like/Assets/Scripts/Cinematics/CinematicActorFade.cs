using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Actor Fade Action", order = 1)]
    public class CinematicActorFade : CinematicAction
    {
        [SerializeField] private bool _fadeOut;
        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();
            _actor.GetComponentInChildren<SpriteRenderer>().enabled = !_fadeOut;
            End();
        }

        public override void End()
        {
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}