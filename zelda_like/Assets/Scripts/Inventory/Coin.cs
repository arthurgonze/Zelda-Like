using ZL.Core;
using UnityEngine;

namespace ZL.Inventory
{
    public class Coin : MonoBehaviour
    {
        [SerializeField] private int _goldCoins = 0;

        private bool _spawned = false;

        public void Start()
        {
            FindObjectOfType<AudioController>().PlayCoinSound();
        }
        public void SetGoldCoins(int value)
        {
            _goldCoins = value;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !_spawned) return;
            other.GetComponent<Inventory>().AddCoins(_goldCoins);
            // TODO audio
            Destroy(this.gameObject);
        }

        public void ToggleSpawned(bool toggle)
        {
            _spawned = toggle;
        }
    }
}