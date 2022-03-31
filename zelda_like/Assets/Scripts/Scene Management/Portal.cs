using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZL.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        private enum DestinationIdentifier
        {
            Alfa, Bravo, Charlie, Delta, Echo
        }

        //[SerializeField] private int _sceneToLoad;
        [SerializeField] [Scene] private string _sceneToLoad;
        [SerializeField] private float _fadeOutTime = 1f;
        [SerializeField] private float _fadeInTime = 0.5f;
        [SerializeField] private float _fadeWaitTime = 0.5f;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private DestinationIdentifier _destination;

        private bool _transitioning = false;
        private void OnTriggerStay2D(Collider2D other)
        {
            //if (SceneManager.GetSceneByName(_sceneToLoad).buildIndex <
            //    SceneManager.GetSceneByName("Sandbox_Main_Map").buildIndex) return;
            //if (other.tag == "Player" && Input.GetButtonDown("Submit") && !_transitioning)
            //    StartCoroutine(Transition());
        }

        public void UsePortal()
        {
            if (!_transitioning)
                StartCoroutine(Transition());
        }

        private IEnumerator Transition()
        {
            //if (_sceneToLoad < 0)
            //{
            //    Debug.LogError("Scene to load not set");
            //    yield break;
            //}
            _transitioning = true;
            DontDestroyOnLoad(this.gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            yield return fader.FadeOut(_fadeOutTime);
            //Debug.Log("Fade Out Portal");
            //MovePlayerToSpawnPoint();

            //savingWrapper.Save();
            yield return SceneManager.LoadSceneAsync(_sceneToLoad);
            //savingWrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);
            //savingWrapper.Save();

            yield return new WaitForSeconds(_fadeWaitTime);
            //fader = FindObjectOfType<Fader>();
            yield return fader.FadeIn(_fadeInTime);
            //Debug.Log("Fade In Portal");

            Destroy(this.gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = otherPortal._spawnPoint.position;
        }

        private void MovePlayerToSpawnPoint()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = _spawnPoint.position;
        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = FindObjectsOfType<Portal>();
            foreach (Portal portal in portals)
            {
                if (portal._destination == this._destination && portal != this)
                {
                    return portal;
                }
            }
            return null;
        }

        public void MakeTransition()
        {
            StartCoroutine(Transition());
        }
    }
}