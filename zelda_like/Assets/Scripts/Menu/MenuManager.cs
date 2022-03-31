using ZL.Control;
using ZL.Core;
using ZL.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZL.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [Space(10)]
        [Header("-------- Fader Time Variables --------")]
        [SerializeField] private float _fadeOutTime = 1f;
        [SerializeField] private float _fadeInTime = 0.5f;
        [SerializeField] private float _fadeWaitTime = 0.5f;

        [Space(10)]
        [Header("-------- Splash Screen Variables --------")]
        private GameObject _splashScreen;
        [SerializeField] private float _splashImageWaitTime = 2f;
        private List<Image> _splashScreenImages = new List<Image>();

        [Space(10)]
        [Header("-------- Menu Variables --------")]
        [SerializeField] private TextMeshProUGUI _resolutionTMP;
        private GameObject _mainMenu;
        private GameObject _pauseMenu;
        private GameObject _optionsMenu;
        private Button _defaultSelectedButton;
        private Slider _defaultOptionsMenuSelectedButton;
        private Button _defaultClosedOptionsMenuSelectedButton;

        //[Space(10)] [Header("-------- Credits --------")]
        private GameObject _creditsScreen;
        private Button _defaultClosedCreditsScreenSelectedButton;

        private GameObject _gameOverScreen;
        private GameObject _gameOverLogo;

        private GameObject _historyScene;

        private GameObject _parent;
        //private Button _currentSelectedButton;

        private int _sceneToLoad = -1;
        private Fader _fader;


        private int _resolutionIndex = 5;
        private bool _isFullScreen = true;
        private AudioController _audioController;

        private void Awake()
        {
            _fader = FindObjectOfType<Fader>();
            _parent = transform.parent.gameObject;
            _audioController = FindObjectOfType<AudioController>();
            //UpdateReferences();
        }

        private void Start()
        {
            InitialScreenConfiguration();

            //ActiveCore();
            UpdateReferences();
            DeactiveMenus();

            PlaySplashScreen();
        }

        private void PlaySplashScreen()
        {
            if (_splashScreen == null) return;
            foreach (Image image in _splashScreen.GetComponentsInChildren<Image>())
            {
                _splashScreenImages.Add(image);
                image.gameObject.SetActive(false);
            }

            StartCoroutine("SplashScreen");
        }

        private void InitialScreenConfiguration()
        {
            _resolutionIndex = 5;
            _isFullScreen = true;
            Screen.fullScreen = true;
            Screen.SetResolution(1920, 1080, _isFullScreen);
        }

        private void LateUpdate()
        {
            if (!CheckReferences() && IsInGameScene())
                UpdateReferences();


            if (_mainMenu == null && _pauseMenu == null) return; // if there is no main menu nor pause menu, return
            if (_mainMenu == null && _pauseMenu != null && _pauseMenu.GetComponent<RectTransform>()?.localScale.x == 0) return; // if there is no main
            // menu but there is a pause menu and it's disable, return
            //Debug.Log("Nenhum botao selecionado e menus ativos");
            // options menu active
            if (EventSystem.current.currentSelectedGameObject != null) return; // if there is a current button selected, return
            if (EventSystem.current.currentSelectedGameObject == null && _optionsMenu.GetComponent<RectTransform>()?.localScale.x != 0)
                EventSystem.current.SetSelectedGameObject(_defaultOptionsMenuSelectedButton.gameObject);
            else if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(_defaultSelectedButton.gameObject);
        }

        private bool UpdateReferences()
        {
            foreach (ScreenCategories screen in FindObjectsOfType<ScreenCategories>())
            {
                if (screen.GetScreenCategory() == ScreenCategories.ScreenCategory.SPLASH && _splashScreen == null)
                    _splashScreen = screen.gameObject;
                if (screen.GetScreenCategory() == ScreenCategories.ScreenCategory.CREDITS && _creditsScreen == null)
                    _creditsScreen = screen.gameObject;
                if (screen.GetScreenCategory() == ScreenCategories.ScreenCategory.GAMEOVER && _gameOverScreen == null)
                    _gameOverScreen = screen.gameObject;
            }

            foreach (MenuCategories menu in FindObjectsOfType<MenuCategories>())
            {
                //Debug.Log("Menus Categories: " + menu.GetMenuCategory().ToString());
                if (menu.GetMenuCategory() == MenuCategories.MenuCategory.MAIN && _mainMenu == null)
                    _mainMenu = menu.gameObject;
                if (menu.GetMenuCategory() == MenuCategories.MenuCategory.PAUSE && _pauseMenu == null)
                    _pauseMenu = menu.gameObject;
                if (menu.GetMenuCategory() == MenuCategories.MenuCategory.OPTIONS && _optionsMenu == null)
                    _optionsMenu = menu.gameObject;
            }

            _historyScene = GameObject.FindGameObjectWithTag("HistoryScene");
            _gameOverLogo = _gameOverScreen?.transform.GetChild(0).gameObject;

            foreach (ButtonCategories button in FindObjectsOfType<ButtonCategories>())
            {
                if (button.GetButtonCategory() == ButtonCategories.ButtonCategory.MAIN_DEFAULT && _defaultSelectedButton == null)
                    _defaultSelectedButton = button.gameObject.GetComponent<Button>();
                if (button.GetButtonCategory() == ButtonCategories.ButtonCategory.OPTIONS_DEFAULT && _defaultOptionsMenuSelectedButton == null)
                    _defaultOptionsMenuSelectedButton = button.gameObject.GetComponent<Slider>();
                if (button.GetButtonCategory() == ButtonCategories.ButtonCategory.CLOSED_OPTIONS && _defaultClosedOptionsMenuSelectedButton == null)
                    _defaultClosedOptionsMenuSelectedButton = button.gameObject.GetComponent<Button>();
                if (button.GetButtonCategory() == ButtonCategories.ButtonCategory.CLOSED_CREDITS && _defaultClosedCreditsScreenSelectedButton == null)
                    _defaultClosedCreditsScreenSelectedButton = button.gameObject.GetComponent<Button>();
            }
            bool referencesUpdated = CheckReferences();
            //Debug.Log("Menu "+this.name+" References Updated: " + referencesUpdated);
            return referencesUpdated;
        }

        private bool CheckReferences()
        {
            bool condition = _pauseMenu != null &&
            _optionsMenu != null &&
            _defaultSelectedButton != null &&
            _defaultOptionsMenuSelectedButton != null &&
            _defaultClosedOptionsMenuSelectedButton != null &&
            _defaultClosedCreditsScreenSelectedButton != null &&
            _gameOverLogo != null &&
            _gameOverScreen != null;

            return condition;
        }

        private void DeactiveMenus()
        {
            //Debug.Log("Menus Disable: "+this.name);
            if (_pauseMenu != null)
                _pauseMenu.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            //_pauseMenu.SetActive(false);
            if (_optionsMenu != null)
                _optionsMenu.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            //_optionsMenu.SetActive(false);
            if (_creditsScreen != null)
                _creditsScreen.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            //_creditsScreen.SetActive(false);
            if (_gameOverScreen != null)
                _gameOverScreen.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            //_gameOverScreen.SetActive(false);
            if (_gameOverLogo != null)
                _gameOverLogo.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            //_gameOverLogo.SetActive(false);
        }

        private void ActiveCore()
        {
            GameObject core = GameObject.FindGameObjectWithTag("Core");
            if (core != null && !IsInGameScene())
                core.SetActive(false);
            else if (core != null && !core.activeSelf && IsInGameScene())
                core.SetActive(true);
        }

        private void InteractWithMenu()
        {
            if (Input.GetButtonUp("Start"))
                TogglePauseMenu();
        }

        public void LoadScene(int sceneToLoad)
        {
            _sceneToLoad = sceneToLoad;
            DisablePlayerInputs();
            StartCoroutine("Transition");
        }

        public void LoadNextScene()
        {
            _sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
            StartCoroutine("Transition");
        }

        private IEnumerator Transition()
        {
            if (_sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set");
                yield break;
            }



            DontDestroyOnLoad(_parent);
            //DontDestroyOnLoad(GameObject.FindWithTag("Core"));
            _fader = FindObjectOfType<Fader>();
            //SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            yield return _fader.FadeOut(_fadeOutTime);
            //Debug.Log("Fade Out Transition, Active scene: " + SceneManager.GetActiveScene().name);

            yield return SceneManager.LoadSceneAsync(_sceneToLoad);

            //DisableChildrenUI();

            yield return new WaitForSeconds(_fadeWaitTime);
            yield return _fader.FadeIn(_fadeInTime);
            //Debug.Log("Fade In Transition");

            Destroy(_parent);
            //Destroy(GameObject.FindWithTag("Core"));
        }

        private void DisablePlayerInputs()
        {
            Debug.Log("Input Disabled");
            foreach (ButtonCategories buttonCategories in FindObjectsOfType<ButtonCategories>())
            {
                //if (buttonCategories.GetButtonCategory() == ButtonCategories.ButtonCategory.MAIN_DEFAULT)
                //{

                Button button = buttonCategories.gameObject.GetComponent<Button>();
                if (button != null)
                    button.interactable = false;
                //}
            }

            StandaloneInputModule inputModule = FindObjectOfType<StandaloneInputModule>();
            if (inputModule != null)
            {
                Debug.Log("Encontrou input module");
                inputModule.enabled = false;
            }

            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                Debug.Log("Encontrou player controller");
                playerController.enabled = false;
            }
        }

        private void DisableChildrenUI()
        {
            foreach (Image image in this.gameObject.GetComponentsInChildren<Image>())
                image.enabled = false;
            foreach (TextMeshProUGUI text in this.gameObject.GetComponentsInChildren<TextMeshProUGUI>())
                text.enabled = false;
        }

        private IEnumerator SplashScreen()
        {
            foreach (Image image in _splashScreenImages)
            {
                image.gameObject.SetActive(true);
                _fader = FindObjectOfType<Fader>();
                yield return _fader.FadeIn(_fadeInTime);
                //Debug.Log("Fade In SplashScreen");
                yield return new WaitForSeconds(_splashImageWaitTime);
                yield return _fader.FadeOut(_fadeOutTime);
                //Debug.Log("Fade Out SplashScreen");
                image.gameObject.SetActive(false);
            }

            _sceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;

            yield return StartCoroutine("Transition");
        }

        public void QuitGame()
        {
            CheckAudioControllerReference();
            _audioController.PlayReturnSound();
            Application.Quit();
        }

        private void CheckAudioControllerReference()
        {
            if (_audioController == null)
                _audioController = FindObjectOfType<AudioController>();
        }

        public void TogglePauseMenu()
        {
            if (_pauseMenu.GetComponent<RectTransform>()?.localScale.x == 0)
            {
                _audioController.PlayPauseInSound();
                //_pauseMenu.SetActive(true);
                _pauseMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                FindObjectOfType<PlayerController>().TogglePlayerControl(false);
                Time.timeScale = 0;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_defaultSelectedButton.gameObject);
            }
            else
            {
                _audioController.PlayPauseOutSound();
                EventSystem.current.SetSelectedGameObject(null);
                //_pauseMenu.SetActive(false);
                _pauseMenu.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);

                FindObjectOfType<PlayerController>().TogglePlayerControl(true);
                Time.timeScale = 1f;
            }
        }

        public void ToggleOptionsMenu()
        {
            CheckAudioControllerReference();
            if (_optionsMenu.GetComponent<RectTransform>()?.localScale.x == 0)
            {
                _audioController.PlayConfirmSound();
                //_optionsMenu.SetActive(true);
                _optionsMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_defaultOptionsMenuSelectedButton.gameObject);
            }
            else
            {
                _audioController.PlayReturnSound();
                //_optionsMenu.SetActive(false);
                _optionsMenu.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_defaultClosedOptionsMenuSelectedButton.gameObject);
            }
        }

        public void ToggleCreditsScreen()
        {
            CheckAudioControllerReference();
            if (_creditsScreen.GetComponent<RectTransform>()?.localScale.x == 0)
            {
                _audioController.PlayConfirmSound();
                //_creditsScreen.SetActive(true);
                _creditsScreen.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                _creditsScreen.GetComponentInParent<CreditsScreen>().ToggleMoveUp(true);
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                _audioController.PlayReturnSound();
                //_creditsScreen.SetActive(false);
                _creditsScreen.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                _creditsScreen.GetComponentInParent<CreditsScreen>().ToggleMoveUp(false);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(_defaultClosedCreditsScreenSelectedButton.gameObject);
            }
        }

        public void ToggleGameOverScreen()
        {
            CheckAudioControllerReference();
            if (_gameOverScreen.gameObject.GetComponent<RectTransform>()?.localScale.x == 0)
            {
                //Debug.Log("Game Over Screen ON");
                //_gameOverScreen.SetActive(true);
                //_gameOverLogo.SetActive(true);
                _audioController.PlayGameOverSound();
                _gameOverScreen.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                _gameOverLogo.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                _gameOverLogo.GetComponent<Animator>().Play("Idle");
                FindObjectOfType<SavingWrapper>().Load();
            }
            else
            {
                //Debug.Log("Game Over Screen OFF");
                //_gameOverScreen.SetActive(false);
                //_gameOverLogo.SetActive(false);
                _gameOverScreen.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                _gameOverLogo.gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            }
        }

        public bool IsInGameScene()
        {
            return _mainMenu == null && _splashScreen == null && _historyScene == null;
        }

        public bool IsPaused()
        {
            if (_pauseMenu == null)
                return false;
            return _pauseMenu.GetComponent<RectTransform>()?.localScale.x != 0;
        }

        public void ChangeResolution()
        {
            _audioController.PlayConfirmSound();
            _resolutionIndex++;
            if (_resolutionIndex > 7)
                _resolutionIndex = 0;
            switch (_resolutionIndex)
            {
                case 0:
                    Screen.SetResolution(1024, 576, _isFullScreen);
                    _resolutionTMP.text = "1024 x 576";
                    break;
                case 1:
                    Screen.SetResolution(1152, 648, _isFullScreen);
                    _resolutionTMP.text = "1152 x 648";
                    break;
                case 2:
                    Screen.SetResolution(1280, 720, _isFullScreen);
                    _resolutionTMP.text = "1280 x 720";
                    break;
                case 3:
                    Screen.SetResolution(1366, 768, _isFullScreen);
                    _resolutionTMP.text = "1366 x 768";
                    break;
                case 4:
                    Screen.SetResolution(1600, 900, _isFullScreen);
                    _resolutionTMP.text = "1600 x 900";
                    break;
                case 5:
                    Screen.SetResolution(1920, 1080, _isFullScreen);
                    _resolutionTMP.text = "1920 x 1080";
                    break;
                case 6:
                    Screen.SetResolution(2560, 1440, _isFullScreen);
                    _resolutionTMP.text = "2560 x 1440";
                    break;
                case 7:
                    Screen.SetResolution(3840, 2160, _isFullScreen);
                    _resolutionTMP.text = "3840 x 2160";
                    break;
                default:
                    Debug.Log("Resolucao invalida");
                    break;
            }

        }

        public void ToggleFullScreen()
        {
            _audioController.PlayConfirmSound();
            Screen.fullScreen = !Screen.fullScreen;
            _isFullScreen = Screen.fullScreen;
        }
    }
}
