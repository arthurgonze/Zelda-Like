using ZL.Menu;
using ZL.Quest;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


namespace ZL.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int _goldCoins = 0;
        [SerializeField] private bool _canDropItemsOnDeath = false;
        [SerializeField] [Range(0, 100)] private int _chanceOfDrop = 0;
        [SerializeField] private QuestItem _droppableQuestItem;
        [SerializeField] private GameObject _coinPrefab;

        private System.Random _rnd = new System.Random();
        private bool _initialCoinHUDUpdated = false;

        void LateUptade()
        {
            if (!this.CompareTag("Player")) return;
            if (!_initialCoinHUDUpdated)
            {
                _initialCoinHUDUpdated = true;
                FindObjectOfType<HUDManager>()?.SetGoldText(_goldCoins);
            }
        }

        public bool CheckIfAnyItemWillDrop()
        {
            if (!_canDropItemsOnDeath) return false;
            float chance = _rnd.Next(0, 100);
            if (chance <= _chanceOfDrop)
            {
                // instantiate the object if any
                if (_droppableQuestItem)
                {
                    QuestItem item = Instantiate(_droppableQuestItem, this.transform.position, Quaternion.identity);
                }

                // get reference to player and add some gold to their inventory
                float normalizedChance = chance / _chanceOfDrop;
                int droppedCoins = Mathf.RoundToInt(normalizedChance * _goldCoins);
                if (droppedCoins > 0)
                {
                    GameObject coin = Instantiate(_coinPrefab, this.transform.position, Quaternion.identity, this.transform);
                    StartCoroutine(LaunchCoin(coin));
                    coin.GetComponent<Coin>().SetGoldCoins(droppedCoins);
                }
            }

            return true;
        }

        private IEnumerator LaunchCoin(GameObject coin)
        {
            Vector2 originPoint = this.transform.position;
            Vector2 controlPointRandomOffset = new Vector2(Random.Range(-2, 2), Random.Range(1, 3));

            Vector2 controlPoint = new Vector2(originPoint.x + controlPointRandomOffset.x, originPoint.y + controlPointRandomOffset.y);
            Vector2 targetPoint = new Vector2(originPoint.x + (2 * controlPointRandomOffset.x), originPoint.y);

            for (float t = 0f; t <= 1; t += Time.deltaTime)
            {
                Vector3 m1 = Vector3.Lerp(originPoint, controlPoint, t);
                Vector3 m2 = Vector3.Lerp(controlPoint, targetPoint, t);
                coin.transform.position = Vector3.Lerp(m1, m2, t);
                yield return null;
            }
            coin.GetComponent<Coin>().ToggleSpawned(true);
        }

        public void AddCoins(int value)
        {
            this._goldCoins += value;
            if (!this.CompareTag("Player")) return;
            FindObjectOfType<HUDManager>().SetGoldText(_goldCoins);
        }

        public void SubtractGoldCoins(int value)
        {
            this._goldCoins -= value;
            if (!this.CompareTag("Player")) return;
            FindObjectOfType<HUDManager>().SetGoldText(_goldCoins);
        }

        public int GetGoldCoins()
        {
            return _goldCoins;
        }

        public void SetGoldCoins(int value)
        {
            _goldCoins = value;
        }

        public void ToggleCanDropItemsOnDeath(bool toggle)
        {
            _canDropItemsOnDeath = toggle;
        }
    }
}

