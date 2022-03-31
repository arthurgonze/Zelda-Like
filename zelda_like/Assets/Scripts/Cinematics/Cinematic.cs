using ZL.Control;
using ZL.SceneManagement;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Cinematics
{
    [Serializable]
    public class Cinematic : MonoBehaviour
    {
        [SerializeField] [ReorderableList] private List<CinematicAction> _actions = new List<CinematicAction>();

        [SerializeField] private int _currentAction = 0;

        [SerializeField] private bool _waitBetweenActions = false;
        [SerializeField] private float _waitTime = 0f;

        private bool _loadedNextScene = false;

        // cached reference
        private PlayerController _player;
        private CanvasGroup _topFader;
        private CanvasGroup _bottomFader;

        // Use this for initialization
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
            _topFader = GameObject.FindGameObjectWithTag("topFader")?.GetComponent<CanvasGroup>();
            _bottomFader = GameObject.FindGameObjectWithTag("bottomFader")?.GetComponent<CanvasGroup>();
        }

        void Start()
        {
            ResetActions();

            ToggleCinematicFaders(false);
        }

        void Update()
        {
            if (_loadedNextScene) return;
            if (Input.GetButtonUp("Cancel"))
            {
                _loadedNextScene = true;
                StopAllCoroutines();
                _actions[_actions.Count - 1].Play();
            }
        }

        private void ResetActions()
        {
            _currentAction = 0;
            foreach (CinematicAction action in _actions)
            {
                action.ToggleEnded(false);
                action.TogglePlaying(false);
            }
        }

        public void Play()
        {
            BlockControls();
            ToggleCinematicFaders(true);
            StartCoroutine("ExecuteActions");
        }

        private void ToggleCinematicFaders(bool toggle)
        {
            if (toggle && _topFader.alpha + _bottomFader.alpha <= 0.1)
            {
                _topFader.alpha = 1;
                _bottomFader.alpha = 1;
            }
            else if (!toggle && _topFader.alpha + _bottomFader.alpha > 1.9)
            {
                _topFader.alpha = 0;
                _bottomFader.alpha = 0;
            }
        }

        private void BlockControls()
        {
            _player.TogglePlayerControl(false);

            foreach (GameObject actor in GameObject.FindGameObjectsWithTag("Enemy"))
                actor.GetComponent<AIController>()?.ToggleBlockControl(true);
        }

        IEnumerator ExecuteActions()
        {

            while (_currentAction < _actions.Count)
            {
                if (_actions.Count == 0)
                    break;

                if (_waitBetweenActions) yield return new WaitForSeconds(_waitTime);
                //Debug.Log("Action: " + _currentAction);
                _actions[_currentAction].Play();
                if (_actions[_currentAction].Ended())
                    _currentAction++;
                yield return null;
            }
            if (_currentAction == _actions.Count)
                End();
        }

        public void End()
        {
            UnblockControls();
            ToggleCinematicFaders(false);
            FindObjectOfType<SavingWrapper>().Save();

        }

        private void UnblockControls()
        {
            _player.TogglePlayerControl(true);
            foreach (GameObject actor in GameObject.FindGameObjectsWithTag("Enemy"))
                actor.GetComponent<AIController>()?.ToggleBlockControl(false);
        }

        public void ToggleWaitBetweenActions(bool toggle)
        {
            _waitBetweenActions = toggle;
        }

        public void SetWaitBetweenActionsTime(float time)
        {
            _waitTime = time;
        }
    }
}