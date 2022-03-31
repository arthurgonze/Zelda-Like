using ZL.Combat;
using ZL.Control;
using ZL.Core;
using ZL.Saving;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Interactives
{
    public class Blacksmith : MonoBehaviour, IInteractable, ISavable
    {
        [SerializeField] private float[] _damageUpgrade;
        [SerializeField] private int[] _upgradeCost;
        [SerializeField] [ReorderableList] private List<string> _unpayedText;
        [SerializeField] [ReorderableList] private List<string> _firstUpgradeText;
        [SerializeField] [ReorderableList] private List<string> _secondUpgradeText;
        [SerializeField] [ReorderableList] private List<string> _thirdUpgradeText;
        [SerializeField] [ReorderableList] private List<string> _fourthUpgradeText;
        [SerializeField] [ReorderableList] private List<string> _fifthUpgradeText;
        [SerializeField] [ReorderableList] private List<string> _fullyUpgradedText;

        private bool _interacting = false;
        private int _upgradeCount = 0;
        private bool[] _payed = { false, false, false, false, false };
        private bool[] _interaction = { false, false, false, false, false };

        TextBalloon _textBalloon;
        void Start()
        {
            _textBalloon = GetComponentInChildren<TextBalloon>();
        }

        public void Close()
        {
            _interacting = false;
            _textBalloon.ToggleEnded(false);
        }

        public void Interrupt()
        {
            _textBalloon.ResetText();
            _textBalloon.Close();
            _interacting = false;
            _textBalloon.ToggleEnded(false);
        }

        public void CloseInteractionWithBlacksmith()
        {
            Interrupt();
        }

        public void Interact()
        {
            _interacting = true;
            if (_interacting && _upgradeCount <= _payed.Length - 1 && !_payed[_upgradeCount] && !_interaction[_upgradeCount])
            {
                _textBalloon.SetDialogo(GetUpgradeText());
                _textBalloon.ToggleAutomatic(false);
                _textBalloon.PlayText();
            }
            else if (_interacting && _upgradeCount <= _payed.Length - 1 && !_payed[_upgradeCount] && _interaction[_upgradeCount])
            {
                string lastLine = _unpayedText[_unpayedText.Count - 1];
                lastLine += " " + _upgradeCost[_upgradeCount].ToString() + " pieces of gold.";
                _unpayedText[_unpayedText.Count - 1] = lastLine;
                _textBalloon.SetDialogo(_unpayedText);
                _textBalloon.ToggleAutomatic(false);
                _textBalloon.PlayText();
            }
            else if (_interacting && _upgradeCount == _payed.Length && _payed[_upgradeCount - 1])// if it is fully upgraded
            {
                _textBalloon.SetDialogo(_fullyUpgradedText);
                _textBalloon.ToggleAutomatic(false);
                _textBalloon.PlayText();
            }
        }
        private void NextLine()
        {
            _textBalloon.NextLine();
        }

        public void InteractWithBlacksmith()
        {
            if (Input.GetButtonDown("Submit") && !_interacting)
                Interact();
            if (Input.GetButtonDown("Submit") && _interacting && !_textBalloon.Ended())
                NextLine();
            if (Input.GetButtonDown("Submit") && _interacting && _textBalloon.Ended())
            {
                _interaction[_upgradeCount] = true;
                WeaponUpgrade();
                Close();
            }
            if (Input.GetButtonUp("Cancel") && _interacting)
                Interrupt();
        }

        private void WeaponUpgrade()
        {
            GameObject player = FindObjectOfType<PlayerController>().gameObject;
            if (player.GetComponent<Inventory.Inventory>().GetGoldCoins() >= _upgradeCost[_upgradeCount])
            {
                player.GetComponent<Inventory.Inventory>().SubtractGoldCoins(_upgradeCost[_upgradeCount]);
                player.GetComponent<Fighter>().AddDamageBonus(_damageUpgrade[_upgradeCount]);
                _payed[_upgradeCount] = true;
                _upgradeCount += 1;
            }
        }

        private List<string> GetUpgradeText()
        {
            switch (_upgradeCount)
            {
                case 0:
                    return _firstUpgradeText;
                    break;
                case 1:
                    return _secondUpgradeText;
                    break;
                case 2:
                    return _thirdUpgradeText;
                    break;
                case 3:
                    return _fourthUpgradeText;
                    break;
                case 4:
                    return _fifthUpgradeText;
                    break;
                default:
                    Debug.Log("Invalid Upgrade Weapon text");
                    return null;
                    break;
            }
        }

        public object CaptureState()
        {
            return _payed;
        }

        public void RestoreState(object state)
        {
            _payed = (bool[])state;
        }
    }
}