using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZL.Core
{
    public class AudioController : MonoBehaviour
    {
        private enum ThemeIdentifier
        {
            Dungeon, AREA_01, BOSS_SLIME, MENU, INTRO, UNDEFINED
        }

        [SerializeField] private ThemeIdentifier _playZoneTheme;

        [Header("-------- Audio Configs --------")]
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private GameObject _musicVolumeSlider;
        [SerializeField] private GameObject _sfxVolumeSlider;

        [Header("-------- Menu Sounds --------")]
        [SerializeField] private AudioClip _confirm_sound; // playoneshot ok
        [SerializeField] private AudioClip _return_sound; // playoneshot ok
        [SerializeField] private AudioClip _hover_button_sound; // playoneshot ok

        [Header("-------- Event Sounds --------")]
        [SerializeField] private AudioClip _gameOver_sound; // playoneshot ok
        [SerializeField] private AudioClip _pauseIn_sound; // playoneshot ok
        [SerializeField] private AudioClip _pauseOut_sound; // playoneshot ok
        [SerializeField] private AudioClip _portal_sound; // playoneshot ok
        [SerializeField] private AudioClip _talk_sound; // playoneshot ok

        [Header("-------- Theme Sounds --------")]
        [SerializeField] private AudioClip _area_01_sound; // play ok
        [SerializeField] private AudioClip _dungeon_sound; // play ok

        [Header("-------- Objects Sounds --------")]
        [SerializeField] private AudioClip _coin_sound; // playoneshot ok
        [SerializeField] private AudioClip _pickup_quest_sound; // playoneshot ok

        [Header("-------- Player Sounds --------")]
        [SerializeField] private AudioClip _player_walk_sound; // playoneshot ok
        [SerializeField] private AudioClip _player_hurt_sound; // playoneshot ok
        [SerializeField] private AudioClip _player_attack_sound; // playoneshot ok
        [SerializeField] private AudioClip _player_skill_sound; // playoneshot ok
        [SerializeField] private AudioClip _player_fireRing_sound; // playoneshot ok

        [Header("-------- Monsters Sounds --------")]
        [SerializeField] private AudioClip _monster_hurt_sound; // playoneshot ok

        [Header("-------- Slime Boss Sounds --------")]
        [SerializeField] private AudioClip _slime_boss_intro_sound; // play ok
        [SerializeField] private AudioClip _slime_boss_main_sound; // play ok
        [SerializeField] private AudioClip _slime_boss_jump_sound; // playoneshot ok
        [SerializeField] private AudioClip _slime_boss_landing_sound; // playoneshot ok

        // cached References
        private AudioSource _mainAudioSource;
        //private AudioSource _sfxAudioSource;

        public void Awake()
        {
            _mainAudioSource = GetComponent<AudioSource>();
            //_sfxAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        }

        public void Start()
        {
            CheckAreaTheme();
        }

        private void CheckAreaTheme()
        {
            switch (_playZoneTheme)
            {
                case ThemeIdentifier.AREA_01:
                    PlayArea01Theme();
                    break;
                case ThemeIdentifier.Dungeon:
                    PlayDungeonTheme();
                    break;
                case ThemeIdentifier.BOSS_SLIME:
                    PlaySlimeBossTheme();
                    break;
                case ThemeIdentifier.MENU:
                    Debug.Log("Menu theme song not avaiable yet");
                    break;
                case ThemeIdentifier.INTRO:
                    Debug.Log("Intro theme song not avaiable yet");
                    break;
                default:
                    Debug.Log("Invalid theme song");
                    break;
            }
        }

        /***********************************
         ********** Audio Configs **********
         ***********************************/
        public void ChangeMusicVolume()
        {
            _mainAudioSource.PlayOneShot(_hover_button_sound);
            _mainAudioSource.volume = _musicVolumeSlider.GetComponent<Slider>().value;
        }

        public void ChangeSFXVolume()
        {
            PlayHoverSound();
            _sfxAudioSource.volume = _sfxVolumeSlider.GetComponent<Slider>().value;
        }

        /*********************************
         ********** Menu Sounds **********
         *********************************/
        public void PlayConfirmSound()
        {
            _sfxAudioSource.PlayOneShot(_confirm_sound);
        }

        public void PlayReturnSound()
        {
            _sfxAudioSource.PlayOneShot(_return_sound);
        }

        public void PlayHoverSound()
        {
            _sfxAudioSource.PlayOneShot(_hover_button_sound);
        }

        /***********************************
         ********** Events Sounds **********
         ***********************************/
        public void PlayGameOverSound()
        {
            _mainAudioSource.PlayOneShot(_gameOver_sound);
        }

        public void PlayPauseInSound()
        {
            _sfxAudioSource.PlayOneShot(_pauseIn_sound);
        }

        public void PlayPauseOutSound()
        {
            _sfxAudioSource.PlayOneShot(_pauseOut_sound);
        }

        public void PlayPortalSound()
        {
            _sfxAudioSource.PlayOneShot(_portal_sound);
        }

        public void PlayTalkSound()
        {
            _sfxAudioSource.PlayOneShot(_talk_sound);
        }

        /**********************************
         ********** Theme Sounds **********
         **********************************/
        public void PlayArea01Theme()
        {
            _mainAudioSource.Stop();
            _mainAudioSource.clip = _area_01_sound;
            _mainAudioSource.Play();
        }

        public void PlayDungeonTheme()
        {
            _mainAudioSource.Stop();
            _mainAudioSource.clip = _dungeon_sound;
            _mainAudioSource.Play();
        }

        /************************************
         ********** Objects Sounds **********
         ************************************/
        public void PlayCoinSound()
        {
            _sfxAudioSource.PlayOneShot(_coin_sound);
        }

        public void PlayPickupQuestSound()
        {
            _sfxAudioSource.PlayOneShot(_pickup_quest_sound);
        }

        /***********************************
         ********** Player Sounds **********
         ***********************************/
        public void PlayPlayerWalkSound()
        {
            _sfxAudioSource.PlayOneShot(_player_walk_sound);
        }

        public void PlayPlayerHurtSound()
        {
            _sfxAudioSource.PlayOneShot(_player_hurt_sound);
        }

        public void PlayPlayerAttackSound()
        {
            _sfxAudioSource.PlayOneShot(_player_attack_sound);
        }

        public void PlayPlayerSkillSound()
        {
            _sfxAudioSource.PlayOneShot(_player_skill_sound);
        }

        public void PlayPlayerFireRingSound()
        {
            _sfxAudioSource.PlayOneShot(_player_fireRing_sound);
        }

        /*************************************
         ********** Monsters Sounds **********
         *************************************/
        public void PlayMonsterHurtSound()
        {
            _sfxAudioSource.PlayOneShot(_monster_hurt_sound);
        }

        /***************************************
         ********** Slime Boss Sounds **********
         ***************************************/
        public void PlaySlimeBossTheme()
        {
            StartCoroutine(PlaySlimeBossThemes());
            //_mainAudioSource.Stop();
            //_mainAudioSource.clip = _slime_boss_intro_sound;
            //_mainAudioSource.Play();
        }
        IEnumerator PlaySlimeBossThemes()
        {
            _mainAudioSource.clip = _slime_boss_intro_sound;
            _mainAudioSource.Play();
            yield return new WaitForSeconds(_mainAudioSource.clip.length);
            _mainAudioSource.clip = _slime_boss_main_sound;
            _mainAudioSource.Play();
        }

        public void PlaySlimeBossMainTheme()
        {
            _mainAudioSource.Stop();
            _mainAudioSource.clip = _slime_boss_main_sound;
            _mainAudioSource.Play();
        }

        public void PlaySlimeBossJumpSound()
        {
            _sfxAudioSource.PlayOneShot(_slime_boss_jump_sound);
        }

        public void PlaySlimeBossLandingSound()
        {
            _sfxAudioSource.PlayOneShot(_slime_boss_landing_sound);
        }
    }
}