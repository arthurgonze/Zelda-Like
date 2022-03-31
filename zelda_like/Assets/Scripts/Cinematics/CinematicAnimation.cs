using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Animation Action", order = 1)]
    public class CinematicAnimation : CinematicAction
    {
        [SerializeField] private string _animationParameter;
        [SerializeField] private bool _isTrigger = true; // is disable is bool
        [SerializeField] private bool _isTrue = false;
        [SerializeField] private string _stateName; // is disable is bool


        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();

            if (_isTrigger)
                _actor.GetComponent<Animator>().SetTrigger(_animationParameter);
            else
                _actor.GetComponent<Animator>().SetBool(_animationParameter, _isTrue);


            if (_actor.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(_stateName) &&
                _actor.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                //Debug.Log("Cinematic animation end - 1");
                End();
            }
            else if (!(_actor.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(_stateName)))
            {
                //Debug.Log("Cinematic animation end - 2");
                End();
            }
        }

        public override void End()
        {
            //if (!_isTrigger)
            //    _actor.GetComponent<Animator>().SetBool(_animationParameter, _isTrue);
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}