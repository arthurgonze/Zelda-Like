using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ZL.Core
{
    public class TextBalloon : MonoBehaviour
    {
        [SerializeField] TMP_Text _textBalloon;
        [SerializeField] private float _textSpeed = 1;
        [SerializeField] [ReorderableList] List<string> _dialogo = new List<string>();

        private AudioController _audioController;

        private string _printedMessage;
        private int _currentLine = 0;
        private float _currentLetter = 0;

        private bool _playText = false;
        private bool _isAutomatic = true;
        private bool _goToNextLine = false;
        private bool _ended = false;

        // Use this for initialization
        void Awake()
        {
            _textBalloon = GetComponentInChildren<TMP_Text>();
            _audioController = FindObjectOfType<AudioController>();
        }

        void Start()
        {
            _dialogo.Clear();
            this.GetComponent<Canvas>().enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_playText && !_ended)
            {
                _textBalloon.text = _printedMessage;

                if (_currentLetter < _dialogo[_currentLine].Length)
                {
                    _currentLetter += Time.deltaTime * _textSpeed;
                    _printedMessage = _dialogo[_currentLine].Substring(0, Mathf.RoundToInt(_currentLetter));
                    if (_printedMessage.Length % 5 == 0)
                        _audioController.PlayTalkSound();
                }
                else
                {
                    if ((!_isAutomatic && _goToNextLine) || (_isAutomatic))
                    {
                        _goToNextLine = false;
                        if (_currentLine == _dialogo.Count - 1)
                            End();
                        else
                        {
                            _printedMessage = string.Empty;
                            _currentLine++;
                            _currentLetter = 0;
                        }
                    }
                }
            }
        }

        public void NextLine()
        {
            if (_playText && !_ended && !(_currentLetter < _dialogo[_currentLine].Length) && !_isAutomatic)
                _goToNextLine = true;
        }

        public void ToggleAutomatic(bool toggle)
        {
            _isAutomatic = toggle;
        }

        public void SetDialogo(List<string> dialogo)
        {
            _dialogo.Clear();
            _dialogo.AddRange(dialogo);
        }

        public List<string> GetDialogo()
        {
            return _dialogo;
        }

        public void PlayText()
        {
            if (!_playText && !_ended)
                _playText = true;

            if (!_ended && !GetComponent<Canvas>().enabled)
                this.GetComponent<Canvas>().enabled = true;
        }

        private void End()
        {
            ResetText();
            Close();
            ToggleEnded(true);
        }

        public void Close()
        {
            this.GetComponent<Canvas>().enabled = false;
            _playText = false;
        }

        public void ResetText()
        {
            _dialogo.Clear();
            _currentLine = 0;
            _currentLetter = 0;
            _printedMessage = string.Empty;
        }

        public void ToggleEnded(bool toggle)
        {
            _ended = toggle;
        }

        public bool Ended()
        {
            return _ended;
        }
    }
}