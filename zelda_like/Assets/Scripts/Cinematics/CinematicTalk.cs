using ZL.Core;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Cinematics
{
    [CreateAssetMenu(fileName = "Cinematics", menuName = "Cinematics/Talk Action", order = 1)]
    public class CinematicTalk : CinematicAction
    {
        [SerializeField] [ReorderableList] List<string> _dialogo = new List<string>();

        private TextBalloon _textBalloon;
        public override void Play()
        {
            TogglePlaying(true);
            ToggleEnded(false);
            FindActor();

            _textBalloon = _actor.GetComponentInChildren<TextBalloon>();

            if (_textBalloon.GetDialogo().Count == 0)
                _textBalloon.SetDialogo(_dialogo);

            _textBalloon.PlayText();

            if (_textBalloon.Ended())
            {
                End();
            }
        }

        public override void End()
        {
            _textBalloon.ResetText();
            _textBalloon.ToggleEnded(false);
            TogglePlaying(false);
            ToggleEnded(true);
        }
    }
}