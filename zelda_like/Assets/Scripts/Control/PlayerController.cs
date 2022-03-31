using ZL.Combat;
using ZL.Core;
using ZL.Interactives;
using ZL.Menu;
using ZL.Movement;
using ZL.Quests;
using ZL.SceneManagement;
using System.Collections;
using UnityEngine;

namespace ZL.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Weapon _defaultWeapon = null;
        [SerializeField] private Weapon[] _secondaryWeapons = new Weapon[3];
        [SerializeField] private bool isCinematicScene = false;

        private int _activeSecondaryWeapon = 0;
        private bool _cancelPlayerControl = false;
        private bool _canInteractWithNPC = false;
        private bool _canInteractWithInteractable = false;
        private bool _usingSpecialAttack = false;
        private bool _canInteractWithPortal = false;

        // Cached Referene
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;
        private HUDManager _hudManager;
        private MenuManager _menuManager;
        private GameObject _interactable;
        private InteractionPopup _interactionPopup;

        // Start is called before the first frame update
        void Start()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _hudManager = FindObjectOfType<HUDManager>();
            _menuManager = FindObjectOfType<MenuManager>();
            _interactionPopup = FindObjectOfType<InteractionPopup>();

            _fighter.EquipWeapon(_defaultWeapon);
        }

        // Update is called once per frame
        void Update()
        {
            if (InteractWithSaveSystem()) return;

            if (_health.IsDead()) return;
            if (_cancelPlayerControl) return;
            if (InteractWithPortal()) return;
            if (InteractWithMenu()) return;

            if (InteractWithNPC()) return;
            if (InteractWithInteractable()) return;
            if (ChangeSecondaryWeapon()) return;

            if (InteractWithBasicAttack()) return;
            if (InteractWithSpecialAttack()) return;

            if (InteractWithMovement()) return;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<QuestGiver>() || collision.gameObject.GetComponent<TalkGoal>() || collision.gameObject.GetComponent<NPC>())
            {
                _canInteractWithNPC = true;
                _interactable = collision.gameObject;
            }

            if (collision.gameObject.GetComponent<Blacksmith>() || collision.gameObject.GetComponent<ImageAltar>() || collision.gameObject.GetComponent<TreasureChest>())
            {
                _canInteractWithInteractable = true;
                _interactable = collision.gameObject;
            }


            if (collision.gameObject.GetComponent<Portal>())
            {
                _canInteractWithPortal = true;
                _interactable = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_canInteractWithNPC)
            {
                _interactable.GetComponent<QuestGiver>()?.CloseInteractionWithNPC();
                _interactable.GetComponent<TalkGoal>()?.CloseInteractionWithNPC();
                _interactable.GetComponent<NPC>()?.CloseInteractionWithNPC();
                _canInteractWithNPC = false;
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
            }

            if (_canInteractWithInteractable)
            {
                _interactable.GetComponent<Blacksmith>()?.CloseInteractionWithBlacksmith();
                _interactable.GetComponent<ImageAltar>()?.CloseInteractionWithAltar();
                _canInteractWithInteractable = false;
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
            }

            if (_canInteractWithPortal)
            {
                _canInteractWithPortal = false;
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
            }
        }

        private bool InteractWithPortal()
        {
            if (!_canInteractWithPortal) return false;
            if (!isCinematicScene)
                _interactionPopup.Appear();
            if (Input.GetButtonDown("Submit"))
            {
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
                _interactable.GetComponent<Portal>().UsePortal();
                return true;
            }
            return false;
        }

        private bool InteractWithNPC()
        {
            if (!_canInteractWithNPC) return false;
            if (!isCinematicScene)
                _interactionPopup.Appear();
            if (Input.GetButtonDown("Submit"))
            {
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
                _interactable.GetComponent<QuestGiver>()?.InteractWithNPC();
                _interactable.GetComponent<TalkGoal>()?.InteractWithNPC();
                _interactable.GetComponent<NPC>()?.InteractWithNPC();
                return true;
            }
            if (Input.GetButtonDown("Cancel"))
            {
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
                _interactable.GetComponent<QuestGiver>()?.CloseInteractionWithNPC();
                _interactable.GetComponent<TalkGoal>()?.CloseInteractionWithNPC();
                _interactable.GetComponent<NPC>()?.CloseInteractionWithNPC();
                return true;
            }

            return false;
        }

        private bool InteractWithInteractable()
        {
            if (!_canInteractWithInteractable) return false;
            if (!isCinematicScene)
                _interactionPopup.Appear();
            if (Input.GetButtonDown("Submit"))
            {
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
                _interactable.GetComponent<Blacksmith>()?.InteractWithBlacksmith();
                _interactable.GetComponent<ImageAltar>()?.InteractWithAltar();
                _interactable.GetComponent<TreasureChest>()?.InteractWithTreasureChest();
                return true;
            }
            if (Input.GetButtonDown("Cancel"))
            {
                if (!isCinematicScene)
                    _interactionPopup.Disappear();
                _interactable.GetComponent<Blacksmith>()?.CloseInteractionWithBlacksmith();
                _interactable.GetComponent<ImageAltar>()?.CloseInteractionWithAltar();
                return true;
            }

            return false;
        }


        private bool InteractWithMenu()
        {
            //if (!_menuManager.IsInGameScene() || !_menuManager.CheckReferences()) return false;
            if (_menuManager == null) return false;
            if (Input.GetButtonUp("Start"))
            {
                _menuManager.TogglePauseMenu();
                return true;
            }

            if (_menuManager.IsPaused())
                return true;

            return false;
        }

        private bool InteractWithSaveSystem()
        {
            //if (Input.GetButtonDown("Save") && !_health.IsDead())
            //{
            //    FindObjectOfType<SavingWrapper>().Save();
            //    return true;
            //}

            if (Input.GetButtonDown("Load"))
            {
                FindObjectOfType<SavingWrapper>().Load();
                return true;
            }

            return false;
        }

        private bool InteractWithBasicAttack()
        {
            if (!Input.GetButtonDown("BasicAttack")) return false;
            Debug.Log("Basic Attack Input, attack function called");

            //double time = Time.realtimeSinceStartup;
            //float time = Time.time;

            _fighter.EquipWeapon(_defaultWeapon);
            //Debug.Log("Weapon Equipped: " + (Time.realtimeSinceStartup - time).ToString() + " ms");
            _mover.UpdatePlayerWeaponSide();
            //Debug.Log("Weapon Pos Updated: " + (Time.realtimeSinceStartup - time).ToString() + " ms");
            //_fighter.PlayerBasicAttack(_defaultWeapon, time);
            _fighter.PlayerBasicAttack(_defaultWeapon);

            //StartCoroutine("BasicAttack");
            return true;
        }

        private bool InteractWithSpecialAttack()
        {
            if (Input.GetButtonDown("SpecialAttack"))
            {
                Debug.Log("Using Special Attack");
                StartCoroutine("SpecialAttack");
                return true;
            }

            //TODO _usingSpecialAttack = _fighter.GetUsingSpecialAttack();
            return _usingSpecialAttack;
        }

        //IEnumerator BasicAttack()
        //{
        //    yield return _fighter.EquipWeapon(_defaultWeapon);
        //    _mover.UpdatePlayerWeaponSide();

        //    yield return _fighter.PlayerBasicAttack(_defaultWeapon);
        //}

        IEnumerator SpecialAttack()
        {
            yield return _fighter.EquipWeapon(_secondaryWeapons[_activeSecondaryWeapon]);
            _mover.UpdatePlayerWeaponSide();
            yield return _fighter.PlayerSpecialAttack(_secondaryWeapons[_activeSecondaryWeapon]);
        }

        public void ToggleUsingSpecialAttack(bool toggle)
        {
            _usingSpecialAttack = toggle;
        }

        private bool InteractWithMovement()
        {
            return _mover.PlayerMove();
        }

        // Maybe change to a coroutine
        private bool ChangeSecondaryWeapon()
        {
            if (Input.GetButtonDown("NextWeapon"))
            {
                NextSecondaryWeapon();
                _fighter.EquipWeapon(_secondaryWeapons[_activeSecondaryWeapon]);
                return true;
            }

            if (Input.GetButtonDown("PreviousWeapon"))
            {
                PreviousSecondaryWeapon();
                _fighter.EquipWeapon(_secondaryWeapons[_activeSecondaryWeapon]);
                return true;
            }

            return false;
        }

        private void NextSecondaryWeapon()
        {
            if (_activeSecondaryWeapon + 1 == _secondaryWeapons.Length)
                _activeSecondaryWeapon = 0;
            else
                _activeSecondaryWeapon++;

            ChangeSpecialWeaponHUDImage();
        }

        private void PreviousSecondaryWeapon()
        {
            if (_activeSecondaryWeapon - 1 < 0)
                _activeSecondaryWeapon = _secondaryWeapons.Length - 1;
            else
                _activeSecondaryWeapon--;

            ChangeSpecialWeaponHUDImage();
        }

        private void ChangeSpecialWeaponHUDImage()
        {
            if (_hudManager == null)
                _hudManager = FindObjectOfType<HUDManager>();
            _hudManager.SetSpecialWeaponImage(_activeSecondaryWeapon);
        }

        public void TogglePlayerControl(bool toggle)
        {
            Debug.Log("Cancel Player Controller: " + !toggle);
            _cancelPlayerControl = !toggle;
        }
    }
}
