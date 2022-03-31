using ZL.Movement;
using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Move Action", order = 1)]
    public class CinematicMove : CinematicAction
    {
        [SerializeField] private float _speed;
        [SerializeField] private Vector2 _destination = new Vector2();
        [SerializeField] private bool _doMovementAnimation = true;

        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();
            //Debug.Log("Moved to: " + _destination + ", current at: " + _actor.transform.position);
            if (_actor.GetComponent<Mover>().DistanceToDestination(_destination) < 0.05f)
                End();
            else
                _actor.GetComponent<Mover>().MoveToDestination(_destination, _speed, _doMovementAnimation);
        }



        public override void End()
        {
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}