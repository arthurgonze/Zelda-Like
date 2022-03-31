using ZL.Saving;
using System.Collections;
using UnityEngine;

namespace ZL.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float _timeToFadeIn = .5f;

        private const string DefaultSaveFile = "save_v0.0";

        // Cached reference
        private SavingSystem _savingSystem;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
        }

        private IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            // fade out
            fader.FadeOutImmediate();
            //Debug.Log("Fade Out Immediate Saving Wrapper");
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                //Debug.Log("Scene Initial Load");
                //yield return _savingSystem.LoadLastScene(DefaultSaveFile);
            }
            // fade in
            yield return fader.FadeIn(_timeToFadeIn);
            //Debug.Log("Fade In SaveWrapper");
        }

        private void Update()
        {

        }

        public void Load()
        {
            _savingSystem.Load(DefaultSaveFile);
            Debug.Log("Game Loaded");
        }

        public void Save()
        {
            _savingSystem.Save(DefaultSaveFile);
            Debug.Log("Game Saved");
        }
    }
}