using ZL.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZL.Menu
{
    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] _weaponImages;

        // cached references
        private TMP_Text _goldText;
        private Image _playerHealthImage;
        private Image _manaImage;
        private Image _specialWeaponImage;
        private Image _basicAttackCooldownImage;
        private Image _specialAttackCooldownImage;
        private MenuManager _menuManager;

        private float _basicAttackCooldown;
        private float _specialAttackCooldown;
        void Awake()
        {
            _menuManager = FindObjectOfType<MenuManager>();
            //if (!_menuManager.IsInGameScene()) return;
            //_goldText = GameObject.FindGameObjectWithTag("GoldTextHUD")?.GetComponent<TMP_Text>();
            //_playerHealthImage = GameObject.FindGameObjectWithTag("HealthHUD")?.GetComponent<Image>();
            //_manaImage = GameObject.FindGameObjectWithTag("ManaHUD")?.GetComponent<Image>();
            //_specialWeaponImage = GameObject.FindGameObjectWithTag("SpecialWeaponImageHUD")?.GetComponent<Image>();
            //_basicAttackCooldownImage = GameObject.FindGameObjectWithTag("BasicAttackCooldownHUD")?.GetComponent<Image>();
            //_specialAttackCooldownImage = GameObject.FindGameObjectWithTag("SpecialAttackCooldownHUD")?.GetComponent<Image>();
        }

        //void Start()
        //{
        //    if (_menuManager.IsInGameScene() && !CheckReferences())
        //        UpdateReferences();
        //    if (!CheckReferences() || !_menuManager.IsInGameScene()) return;

        //    _basicAttackCooldownImage.fillAmount = 0;
        //    _specialAttackCooldownImage.fillAmount = 0;
        //}

        private bool UpdateReferences()
        {
            foreach (HUDCategories hud in FindObjectsOfType<HUDCategories>())
            {
                if (hud.GetHUDCategory() == HUDCategories.HUDCategory.GOLD_TEXT)
                    _goldText = hud.gameObject.GetComponent<TMP_Text>();
                if (hud.GetHUDCategory() == HUDCategories.HUDCategory.PLAYER_HEALTH_IMAGE)
                    _playerHealthImage = hud.gameObject.GetComponent<Image>();
                if (hud.GetHUDCategory() == HUDCategories.HUDCategory.PLAYER_MANA_IMAGE)
                    _manaImage = hud.gameObject.GetComponent<Image>();
                if (hud.GetHUDCategory() == HUDCategories.HUDCategory.SPECIAL_WEAPON_IMAGE)
                    _specialWeaponImage = hud.gameObject.GetComponent<Image>();
                if (hud.GetHUDCategory() == HUDCategories.HUDCategory.BASIC_ATTACK_COOLDOWN_IMAGE)
                    _basicAttackCooldownImage = hud.gameObject.GetComponent<Image>();
                if (hud.GetHUDCategory() == HUDCategories.HUDCategory.SPECIAL_ATTACK_COOLDOWN_IMAGE)
                    _specialAttackCooldownImage = hud.gameObject.GetComponent<Image>();
            }
            //_goldText = GameObject.FindGameObjectWithTag("GoldTextHUD")?.GetComponent<TMP_Text>();
            //_playerHealthImage = GameObject.FindGameObjectWithTag("HealthHUD")?.GetComponent<Image>();
            //_manaImage = GameObject.FindGameObjectWithTag("ManaHUD")?.GetComponent<Image>();
            //_specialWeaponImage = GameObject.FindGameObjectWithTag("SpecialWeaponImageHUD")?.GetComponent<Image>();
            //_basicAttackCooldownImage = GameObject.FindGameObjectWithTag("BasicAttackCooldownHUD")?.GetComponent<Image>();
            //_specialAttackCooldownImage = GameObject.FindGameObjectWithTag("SpecialAttackCooldownHUD")?.GetComponent<Image>();
            if (_basicAttackCooldownImage)
                _basicAttackCooldownImage.fillAmount = 0;
            if (_specialAttackCooldownImage)
                _specialAttackCooldownImage.fillAmount = 0;
            if (_goldText)
                _goldText.text = FindObjectOfType<PlayerController>().gameObject.GetComponent<Inventory.Inventory>().GetGoldCoins().ToString();

            return CheckReferences();
        }

        void LateUpdate()
        {
            // TODO cinematic condition
            if (_menuManager == null)
            {
                _menuManager = FindObjectOfType<MenuManager>();
                return;
            }

            if (_menuManager.IsInGameScene() && !CheckReferences())
            {
                bool referencesUpdated = UpdateReferences();
                //Debug.Log("Hud References updated: " + referencesUpdated);
            }
            if (_menuManager == null || !_menuManager.IsInGameScene() || !CheckReferences()) return;

            Cooldowns();
        }

        private bool CheckReferences()
        {
            bool condition =
                _goldText != null &&
                _playerHealthImage != null &&
                _manaImage != null &&
                _specialWeaponImage != null &&
                _basicAttackCooldownImage != null &&
                _specialAttackCooldownImage != null;
            return condition;
        }

        private void Cooldowns()
        {
            if (_basicAttackCooldownImage.fillAmount > 0)
            {
                _basicAttackCooldownImage.fillAmount -= Time.deltaTime / _basicAttackCooldown;
            }

            if (_specialAttackCooldownImage.fillAmount > 0)
            {
                _specialAttackCooldownImage.fillAmount -= Time.deltaTime / _specialAttackCooldown;
            }
        }

        public void SetGoldText(int gold)
        {
            _goldText.text = gold.ToString();
        }

        public void SetHealthValue(float value)
        {
            _playerHealthImage.fillAmount = value;
        }

        public void SetManaValue(float value)
        {
            _manaImage.fillAmount = value;
        }

        public void SetSpecialWeaponImage(int index)
        {
            _specialWeaponImage.sprite = _weaponImages[index];
        }

        public void StartBasicAttackCooldownCount(float cooldownTime)
        {
            if (_basicAttackCooldownImage.fillAmount == 0)
            {
                _basicAttackCooldown = cooldownTime;
                _basicAttackCooldownImage.fillAmount = 1;
            }
        }

        public void StartSpecialAttackCooldownCount(float cooldownTime)
        {
            if (_specialAttackCooldownImage.fillAmount == 0)
            {
                _specialAttackCooldown = cooldownTime;
                _specialAttackCooldownImage.fillAmount = 1;
            }
        }
    }
}