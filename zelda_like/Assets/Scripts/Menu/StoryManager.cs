using ZL.Core;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZL.Menu
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _storyTextObject;
        [SerializeField] private Image _storyImage;
        [SerializeField] private float _zoomOutTime = 15f;
        [SerializeField] private float _textSpeed = 1;
        [SerializeField] [ReorderableList] private List<string> _storyTextLines;
        [SerializeField] private float _endPhraseWaitTime = 1f;
        [SerializeField] [Scene] private int _sceneToLoadNext;

        private AudioController _audioController;

        private string _printedMessage;
        private int _currentLine = 0;
        private float _currentLetter = 0;

        private float _zoomOutRatio;
        private bool _loadedNextScene = false;

        private float _countWaitTime = 0;

        public void Awake()
        {
            _audioController = FindObjectOfType<AudioController>();
        }


        // Use this for initialization
        void Start()
        {
            StartCoroutine("ZoomOut");
        }

        void Update()
        {
            if (_loadedNextScene) return;
            if (Input.GetButtonUp("Cancel"))
            {
                _loadedNextScene = true;
                FindObjectOfType<MenuManager>().LoadScene(_sceneToLoadNext);
            }

            _countWaitTime -= Time.deltaTime;
            _storyTextObject.text = _printedMessage;
            if (_currentLetter < _storyTextLines[_currentLine].Length)
            {
                if (Input.GetButtonUp("Submit"))
                {
                    _printedMessage = _storyTextLines[_currentLine];
                    _currentLetter = _storyTextLines[_currentLine].Length;
                    _countWaitTime = _endPhraseWaitTime;
                }
                else
                {
                    _currentLetter += Time.deltaTime * _textSpeed;
                    _printedMessage = _storyTextLines[_currentLine].Substring(0, Mathf.RoundToInt(_currentLetter));
                    _countWaitTime = _endPhraseWaitTime;
                    if (_printedMessage.Length % 2 == 0)
                        _audioController.PlayTalkSound();
                }
            }
            else if (_countWaitTime < 0)
            {
                //if (Input.GetButtonUp("Submit"))
                //{
                if (_currentLine == _storyTextLines.Count - 1)
                {
                    _loadedNextScene = true;
                    FindObjectOfType<MenuManager>().LoadScene(_sceneToLoadNext);
                    //.LoadNextScene();
                }
                else
                {
                    _printedMessage = string.Empty;
                    _currentLine++;
                    _currentLetter = 0;
                }
                //}
            }
        }

        public IEnumerator ZoomOut()
        {
            _storyImage.transform.localScale = new Vector3(1f, 1f, 1f);
            _zoomOutRatio = Time.deltaTime / _zoomOutTime;
            while (_storyImage.transform.localScale.x > 0f)
            {
                _storyImage.transform.localScale -= new Vector3(_zoomOutRatio, _zoomOutRatio, _zoomOutRatio);
                yield return null;
            }
        }
    }
}