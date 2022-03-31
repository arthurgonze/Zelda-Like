using UnityEngine;

namespace ZL.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _persistentObjectPrefab;
        private static bool _hasSpawned = false;

        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersistentObjects();
            _hasSpawned = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObject = Instantiate(_persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}