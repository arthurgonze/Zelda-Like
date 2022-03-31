using ZL.Core;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Interactives
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] TextBalloon _textBalloon;
        [SerializeField] private List<string> _dialogo;
        private bool _interacting = false;

        // Use this for initialization
        void Start()
        {
            _textBalloon = GetComponentInChildren<TextBalloon>();
        }


        public void Close()
        {
            _interacting = false;
            _textBalloon.ToggleEnded(false);
        }

        public void Interact()
        {
            _interacting = true;
            _textBalloon.SetDialogo(_dialogo);
            _textBalloon.ToggleAutomatic(false);
            _textBalloon.PlayText();
        }

        private void NextLine()
        {
            _textBalloon.NextLine();
        }

        public void InteractWithNPC()
        {
            if (!_interacting)
                Interact();
            else if (_interacting && !_textBalloon.Ended())
                NextLine();
            else if (_interacting && _textBalloon.Ended())
                Close();
        }

        public void CloseInteractionWithNPC()
        {
            if (_interacting)
                Close();
        }
    }
}