using NaughtyAttributes;
using UnityEngine;

namespace ZL.Cinematics
{
    public abstract class CinematicAction : ScriptableObject, ICinematicAction
    {
        [SerializeField] internal string _actorName;
        [SerializeField] [Tag] internal string _actorTag;

        internal bool _isPlaying = false;
        internal bool _ended = false;
        internal GameObject _actor = null;

        public abstract void Play();

        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public abstract void End();

        public bool Ended()
        {
            return _ended;
        }

        public void ToggleEnded(bool toggle)
        {
            _ended = toggle;
        }

        public void TogglePlaying(bool toggle)
        {
            _isPlaying = toggle;
        }

        internal void FindActor()
        {
            foreach (GameObject actor in GameObject.FindGameObjectsWithTag(_actorTag))
                if (actor.name == _actorName)
                    _actor = actor;
        }
    }
}