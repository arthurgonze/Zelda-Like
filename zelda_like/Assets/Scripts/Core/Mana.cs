using ZL.Menu;
using ZL.Saving;
using UnityEngine;

namespace ZL.Core
{
    public class Mana : MonoBehaviour, ISavable
    {
        [SerializeField] private float _manaPoints = 100f;
        [SerializeField] private float _maxManaPoints = 100f;

        public void ConsumeMana(float value)
        {
            _manaPoints = Mathf.Max(_manaPoints - value, 0);
            UpdateManaUI();
        }

        public void RestoreMana(float value)
        {
            _manaPoints = Mathf.Max(_manaPoints + value, 100);
            UpdateManaUI();
        }

        private void UpdateManaUI()
        {
            if (!this.CompareTag("Player")) return;
            FindObjectOfType<HUDManager>().SetManaValue(_manaPoints / _maxManaPoints);
        }

        public float GetMana()
        {
            return _manaPoints;
        }

        public object CaptureState()
        {
            return this._manaPoints;
        }

        public void RestoreState(object state)
        {
            this._manaPoints = (float)state;
        }
    }
}
