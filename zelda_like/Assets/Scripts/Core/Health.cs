using ZL.Control;
using ZL.Menu;
using ZL.Quests;
using ZL.Saving;
using ZL.SceneManagement;
using System.Collections;
using UnityEngine;

namespace ZL.Core
{
    public class Health : MonoBehaviour, ISavable
    {

        [SerializeField] private float _healthPoints = 100f;
        [SerializeField] private bool _die = false;
        [SerializeField] private float _delayToDisableColliderAfterDeath = 1f;
        [SerializeField] private float _maxHealthPoints = 100f;
        [Header("-------- Boss Death Variables --------")]
        [SerializeField] private float _cameraShakeMagnitude;
        [SerializeField] private float _cameraShakeDuration;
        [SerializeField] private float _oneFlashTime;
        [SerializeField] private float _flashScreenDurantion;

        private bool _isDead = false;

        public void Update()
        {
            if (_die) Die();
        }

        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            UpdateHealthUI();
            if (_healthPoints <= 0)
                Die();
        }

        private void Die()
        {
            if (_isDead) return;
            _isDead = true;

            if (!this.GetComponent<Animator>().GetBool("dead"))
                this.GetComponent<Animator>().SetBool("dead", true);

            if (this.gameObject.tag == "Boss")
            {
                BossDeath();
                return;
            }

            if (this.gameObject.tag == "Player")
            {
                StartCoroutine("GameOver");
                return;
            }

            MonsterDeath();
        }

        private void MonsterDeath()
        {
            StartCoroutine("DeactivateCollider");

            Inventory.Inventory inventory = this.GetComponent<Inventory.Inventory>();
            if (inventory)
                inventory.CheckIfAnyItemWillDrop();

            KillGoal killGoal = GetComponent<KillGoal>();
            if (killGoal)
            {
                killGoal.SubtractFromKillGoal();
                FindObjectOfType<SavingWrapper>().Save();
            }
        }

        private void BossDeath()
        {
            Debug.Log("Boss Died");
            // remove control from player
            FindObjectOfType<PlayerController>().TogglePlayerControl(false);

            // shake the screen
            FindObjectOfType<CameraUtils>().ShakeCamera(_cameraShakeMagnitude, _cameraShakeDuration);

            // flash white
            Fader fader = FindObjectOfType<Fader>();
            fader.FlashScreen(_oneFlashTime, _flashScreenDurantion, Color.white);

            // play the end death animation
            StartCoroutine("BossDeathEndAnimation");
            return;
        }

        IEnumerator BossDeathEndAnimation()
        {
            yield return new WaitForSeconds(_flashScreenDurantion);
            if (!this.GetComponent<Animator>().GetBool("endDeath"))
                this.GetComponent<Animator>().SetBool("endDeath", true);

            // after boss death things
            // player can move again
            FindObjectOfType<PlayerController>().TogglePlayerControl(true);
        }

        IEnumerator DeactivateCollider()
        {
            yield return new WaitForSeconds(_delayToDisableColliderAfterDeath);
            if (GetComponent<Collider2D>().enabled)
                GetComponent<Collider2D>().enabled = false;
        }

        IEnumerator GameOver()
        {
            FindObjectOfType<MenuManager>().ToggleGameOverScreen();
            FindObjectOfType<AudioController>().PlayGameOverSound();
            //FindObjectOfType<SavingWrapper>().Load();
            //Revive();
            yield return null;
            FindObjectOfType<MenuManager>().LoadScene(3);
            yield return null;
            FindObjectOfType<SavingWrapper>().Load();
            yield return null;
            //yield return new WaitForSeconds(2f);
            //FindObjectOfType<MenuManager>().ToggleGameOverScreen();
        }

        public void Revive()
        {
            if (!_isDead) return;
            _healthPoints = _maxHealthPoints;
            _isDead = false;
            this.GetComponent<Animator>().SetBool("dead", false);
            GetComponent<Collider2D>().enabled = true;
            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            if (!this.CompareTag("Player")) return;
            FindObjectOfType<HUDManager>().SetHealthValue(_healthPoints / _maxHealthPoints);
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public object CaptureState()
        {
            return this._healthPoints;
        }

        public void RestoreState(object state)
        {
            this._healthPoints = (float)state;
            if (this._healthPoints <= 0)
                Die();
        }

        public float GetHealthPercentage()
        {
            return (_healthPoints / _maxHealthPoints) * 100f;
        }
    }
}
