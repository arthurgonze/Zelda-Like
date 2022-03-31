using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Wait Action", order = 1)]
    public class CinematicWaitAction : CinematicAction
    {
        [SerializeField] private float _waitTime = 1f;
        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();

            _actor.GetComponent<Cinematic>().SetWaitBetweenActionsTime(_waitTime);
            _actor.GetComponent<Cinematic>().ToggleWaitBetweenActions(true);
            End();
        }

        public override void End()
        {
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}