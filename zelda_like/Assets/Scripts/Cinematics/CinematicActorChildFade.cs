using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Actor Child Fade Action", order = 1)]
    public class CinematicActorChildFade : CinematicAction
    {
        //[SerializeField] private string _childName;
        [SerializeField] private bool _fadeOut;
        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();

            foreach (SpriteRenderer child in _actor.GetComponentsInChildren<SpriteRenderer>())
            {
                child.enabled = !_fadeOut;
            }

            //SpriteRenderer actorChildRenderer = _actor.transform.Find(_childName).GetComponent<SpriteRenderer>();
            //if (actorChildRenderer != null)
            //    actorChildRenderer.enabled = !_fadeOut;
            //SpriteRenderer actorGrandChildRenderer = _actor.transform.Find(_childName).GetComponentInChildren<SpriteRenderer>();
            //if (actorGrandChildRenderer != null)
            //    actorGrandChildRenderer.enabled = !_fadeOut;

            End();
        }

        public override void End()
        {
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}