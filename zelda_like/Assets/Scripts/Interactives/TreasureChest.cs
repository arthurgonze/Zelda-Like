using ZL.Inventory;
using System.Collections;
using UnityEngine;

namespace ZL.Interactives
{
    public class TreasureChest : MonoBehaviour
    {
        [SerializeField] private Sprite _openedSprite;
        [SerializeField] private int _goldCoins = 0;
        [SerializeField] private GameObject _coinPrefab;


        private bool _opened = false;

        // cached references
        private SpriteRenderer _spriteRenderer;

        public void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void InteractWithTreasureChest()
        {
            if (_opened) return;
            _spriteRenderer.sprite = _openedSprite;
            GameObject coin = Instantiate(_coinPrefab, this.transform.position, Quaternion.identity, this.transform);
            StartCoroutine(LaunchCoin(coin));
            coin.GetComponent<Coin>().SetGoldCoins(_goldCoins);
            _opened = true;
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
    }
}