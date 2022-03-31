using ZL.SceneManagement;
using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Portal Transition", order = 1)]
    public class CinematicSceneTransition : CinematicAction
    {
        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();
            _actor.GetComponent<Portal>().MakeTransition();
            End();
        }



        public override void End()
        {
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}