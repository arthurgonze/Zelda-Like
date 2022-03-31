using ZL.Combat;
using ZL.Control;
using ZL.Core;
using ZL.Monsters;
using ZL.Movement;
using ZL.Vision;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ZL.Bosses
{
    public class SlimeBoss : MonoBehaviour
    {
        [Header("-------- Global Variables --------")]
        [SerializeField] private Image _healthImage;
        [SerializeField] private Transform _weaponPoint = null;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _knockbackForce = 15f;
        [SerializeField] private float _touchDamage = 15f;

        [Header("-------- Shot Variables --------")]
        [SerializeField] private AnimatorOverrideController _upOverrideController;
        [SerializeField] private AnimatorOverrideController _downOverrideController;

        [Header("-------- Shot Variables --------")]
        [SerializeField] private Weapon _rangedWeapon;
        [SerializeField] private float _bulletHellTime = 4f;

        [Header("-------- Divide Variables --------")]
        [SerializeField] private GameObject _slimesDefaultPrefab;
        [SerializeField] private int _numberOfSlimes = 6; // 6 a 9 segundos
        [SerializeField] private float _slimeFollowTime = 10f;
        [SerializeField] private Vector2 _leftUpSlimeLaunchLimit;
        [SerializeField] private Vector2 _rightDownSlimeLaunchLimit;
        [SerializeField] private List<GameObject> _spawnedDivideSlimes;
        [SerializeField] private float _damagePerDeadSlime = 10f;
        [SerializeField] private float _distanceFromTheBossToReunite = 4f;
        [SerializeField] private float _slimeReturnSpeedMultiplier = 2f;


        [Header("-------- Jump Variables --------")]
        [SerializeField] private float _jumpFlyTime = 5;// 5 a 10 segundos
        [SerializeField] private float _jumpFallTime = 1f; // can remove
        [SerializeField] private float _jumpHeight = 15f;
        [SerializeField] private float _jumpSpeed = 5f;
        [SerializeField] private float _jumpDamage = 25f;
        [SerializeField] private GameObject _shadow;
        [SerializeField] private Transform _body;
        [SerializeField] private float _playerDistanceFromTheCenter;
        [SerializeField] private float _thresholdDistanceFromTheCenter;
        [SerializeField] private float _playerDistanceFromDamage = 5f;
        [SerializeField] private Vector2 _leftUpFollowLimit;
        [SerializeField] private Vector2 _rightDownFollowLimit;
        [SerializeField] private float _followSpeed = 5f;
        [SerializeField] private float _cameraShakeMagnitude;
        [SerializeField] private float _cameraShakeDuration;

        [Header("-------- Super Move Variables --------")]
        [SerializeField] private int _superMoveSlimes = 4;
        [SerializeField] private GameObject _rangedSlimePrefab;
        [SerializeField] [ReorderableList] private List<Vector2> _roomCorners;
        [SerializeField] private float _slimeShootTime = 4f;
        [SerializeField] private float _waitToShootTime = 2f;
        [SerializeField] private List<GameObject> _spawnedRangedSlimes;
        [SerializeField] private Vector2 _mapCenterCoordinate;
        [SerializeField] private float _distanceFromTheCenterToReunite = 4f;


        // jump variables
        private float _flyingTime = 0;
        private bool _haveJumped = false;
        private bool _isFlying = false;
        private bool _countFlyTime = false;
        private bool _jumping = false;
        private bool _falling = false;
        private bool _fallingImpact = false;
        private bool _updateTargetPosition = true;
        private bool _hammerTime = false;


        // divide variables
        private float _slimeFollowingTime = 0;
        private bool _haveDivided = false;
        private bool _slimesSpawned = false;
        private bool _countSlimeFollowingTime = false;
        private bool _reunite = false;
        private int _countedDeadSlimes = 0;

        // shoot variables
        private float _shootingTime = 0;
        private bool _haveShooted = false;
        private bool _countBulletHellTime = false;

        // super moves variables
        private bool _playedFirstSuperMove = false;
        private bool _playedSecondSuperMove = false;
        private bool _countSlimeShootingTime = false;
        private bool _rangedSlimesSpawned = false;
        private bool _countWaitToShootTime = false;
        private float _countingWaitToShootTime = 0f;
        private float _slimesShootingTime = 0;
        private bool _resetSuperMove = false;


        // general variables
        private float _timeSinceLastAttack = Mathf.Infinity;
        private Vector3 _playerLastKnownPosition;
        private Health _target;

        // Cached References
        private Health _health;
        private Animator _animator;
        private Animator _shadowAnimator;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;
        //private SlimeBossMover _mover;

        void Awake()
        {
            _health = this.gameObject.GetComponent<Health>();
            _animator = GetComponent<Animator>();
            _shadowAnimator = _shadow.GetComponent<Animator>();
            _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
            _collider = this.GetComponent<Collider2D>();
            //_mover = GetComponent<SlimeBossMover>();
            _target = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            _spawnedDivideSlimes = new List<GameObject>();
        }

        // Use this for initialization
        void Start()
        {
            _numberOfSlimes = Random.Range(6, 9);
            _jumpFlyTime = Random.Range(5, 10);
            //_jumpFallTime = 1f;
            SetHealthValue(1);
            _shadow.SetActive(false);
            SetHealthValue(_health.GetHealthPercentage() / 100);
        }

        // Update is called once per frame
        void Update()
        {
            if (_target.IsDead() || _health.IsDead()) return;
            UpdateTimers();
            UpdateAnimator();

            UpdateTargetPosition();

            BossMovesets();
        }

        private void UpdateTargetPosition()
        {
            if (_updateTargetPosition)
            {
                _playerLastKnownPosition = new Vector3(_target.transform.position.x,
                    _target.transform.position.y + 2.75f, _target.transform.position.z);
                if (_playerLastKnownPosition.x > _rightDownFollowLimit.x)
                    _playerLastKnownPosition.x = _rightDownFollowLimit.x;
                if (_playerLastKnownPosition.x < _leftUpFollowLimit.x)
                    _playerLastKnownPosition.x = _leftUpFollowLimit.x;
                if (_playerLastKnownPosition.y < _rightDownFollowLimit.y)
                    _playerLastKnownPosition.y = _rightDownFollowLimit.y;
                if (_playerLastKnownPosition.y > _leftUpFollowLimit.y)
                    _playerLastKnownPosition.y = _leftUpFollowLimit.y;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //Debug.Log(collision.gameObject.name + " collided with the boss, tag: " + collision.gameObject.tag);
            if (collision.gameObject.tag == "PlayerWeapon")
                BeingHitBehavior(collision);
            if (collision.gameObject == _target.gameObject && !_countFlyTime)
            {
                //Debug.Log("Player was hit by boss");
                _target.GetComponent<Fighter>().DamageBehavior(this.GetComponent<Collider2D>(), _knockbackForce, _touchDamage);
            }
        }

        private void UpdateAnimator()
        {
            Vector3 dir = _target.transform.position - this.transform.position;
            dir.Normalize();
            if (dir.y >= 0 && (_animator.runtimeAnimatorController.name != _upOverrideController.name))
                _animator.runtimeAnimatorController = _upOverrideController;
            else if (dir.y < 0 && (_animator.runtimeAnimatorController.name != _downOverrideController.name))
                _animator.runtimeAnimatorController = _downOverrideController;
        }

        private void UpdateTimers()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (_countBulletHellTime)
                _shootingTime += Time.deltaTime;
            if (_countFlyTime)
                _flyingTime += Time.deltaTime;
            if (_countSlimeFollowingTime)
                _slimeFollowingTime += Time.deltaTime;
            if (_countSlimeShootingTime)
                _slimesShootingTime += Time.deltaTime;
            if (_countWaitToShootTime)
                _countingWaitToShootTime += Time.deltaTime;
        }

        // Movesets
        private void BossMovesets()
        {
            if ((_health.GetHealthPercentage() > 20 && _health.GetHealthPercentage() <= 60 && !_playedFirstSuperMove) ||
                (_health.GetHealthPercentage() <= 20 && !_playedSecondSuperMove))
            {
                SuperMove();
                return;
            }

            if (_timeSinceLastAttack < _timeBetweenAttacks) return;

            float healthPercentage = _health.GetHealthPercentage();
            if (healthPercentage > 60)
                FirstMoveset();

            if (healthPercentage > 20 && healthPercentage <= 60)
                SecondMoveset();

            if (healthPercentage <= 20)
                ThirdMoveset();

        }

        private void ThirdMoveset()
        {
            // Divisao - Pulo - Tiro

            if (!_haveShooted && !_haveJumped && !_haveDivided)
            {
                Divide();
                return;
            }

            if (!_haveShooted && !_haveJumped && _haveDivided)
            {
                Jump();
                return;
            }
            if (_haveJumped)
            {
                ResetJumpVariables();
            }

            if (_shootingTime > _bulletHellTime)
            {
                _shootingTime = 0;
                _countBulletHellTime = false;
                _haveShooted = true;
                _timeSinceLastAttack = 0;
                ResetAttackBools();
                return;
            }
            if (_shootingTime <= _bulletHellTime && !_haveShooted && _haveJumped && _haveDivided)
            {
                Shoot();
                _countBulletHellTime = true;
                //Debug.Log("Reset time attack cooldown");
                return;
            }
        }

        private void SecondMoveset()
        {
            // Tiro - Pulo - Tiro
            if (_shootingTime > _bulletHellTime)
            {
                _shootingTime = 0;
                _countBulletHellTime = false;
                _timeSinceLastAttack = 0;
                if (!_haveShooted)
                    _haveShooted = true;
                else if (_haveJumped)
                    ResetAttackBools();
                return;
            }

            if (_shootingTime <= _bulletHellTime && !_haveShooted && !_haveJumped)
            {
                Shoot();
                _countBulletHellTime = true;

                return;
            }

            if (_haveShooted && !_haveJumped)
            {
                Jump();
                return;
            }
            if (_haveJumped)
            {
                ResetJumpVariables();
            }

            if (_haveShooted && _haveJumped)
            {
                Shoot();
                _countBulletHellTime = true;
                _timeSinceLastAttack = 0;
                return;
            }
        }

        private void FirstMoveset()
        {
            // Pulo - Divisao - Tiro
            if (!_haveShooted && !_haveJumped && !_haveDivided)
            {
                Jump();
                return;
            }
            if (_haveJumped)
            {
                ResetJumpVariables();
            }

            if (!_haveShooted && _haveJumped && !_haveDivided)
            {
                Divide();
                return;
            }

            if (_shootingTime > _bulletHellTime)
            {
                _shootingTime = 0;
                _countBulletHellTime = false;
                _haveShooted = true;
                _timeSinceLastAttack = 0;
                ResetAttackBools();
                return;
            }

            if (_shootingTime <= _bulletHellTime && !_haveShooted && _haveJumped && _haveDivided)
            {
                Shoot();
                _countBulletHellTime = true;
                return;
            }
        }


        // Behaviors
        // O boss permanecerá parado enquanto atira projéteis retos em direção ao jogador por 4s
        private void Shoot()
        {
            //Debug.Log("Shooting");
            if (_target.IsDead()) return;
            if (_target == null) return;

            _animator.SetTrigger("shoot");

            //LaunchProjectile();
        }

        // O boss dará um pulo que irá sair da tela do jogador.
        // Ao subir, o boss ficará imune, e a sua sombra é a unica coisa que permanecerá na tela.
        // Esta irá seguir o jogador por uns 5~10s e após esse tempo acabar, ele cairá sobre o player.
        // Ela também cairá sobre o player se este passar no centro da sua sombra.
        private bool Jump()
        {
            //Debug.Log("Jumping");
            if (_target.IsDead() || _target == null) return false;

            // Jumping movement
            if (!_falling && _flyingTime <= _jumpFlyTime)
            {
                // set the jump animations
                if (!_animator.GetBool("jump"))
                    _animator.SetBool("jump", true);

                // defined by animation
                if (!_jumping) return false;

                Vector2 flyPos = new Vector2(this.transform.position.x, _jumpHeight);
                // Fly
                if (Vector2.Distance(_body.position, flyPos) > 0.2f && !_isFlying)
                {
                    _body.position = new Vector3(_body.position.x, _body.position.y + _jumpSpeed * Time.deltaTime, _body.position.z);
                    if (!_shadow.activeSelf)
                        _shadow.SetActive(true);
                    return false;
                }

                // follow player
                _isFlying = true;
                _countFlyTime = true;
                _updateTargetPosition = true;
                MoveTo(_playerLastKnownPosition);
            }
            else
                _falling = true;

            if (!_falling && TargetDistanceFromTheCenter() <= _thresholdDistanceFromTheCenter)
                _falling = true;

            // Falling
            if (_falling)
            {
                //Debug.Log("Falling");
                // stop following time and count the delay to fall down
                _updateTargetPosition = false;

                // falling animations
                _shadowAnimator.SetTrigger("fall");
                if (!_animator.GetBool("fall") && !_fallingImpact)
                    _animator.SetBool("fall", true);

                // fall motion
                if (_body.position.x != transform.position.x)
                    _body.position = new Vector3(this.transform.position.x, _body.position.y, _body.position.z);

                //StartCoroutine("FallingBehavior");
                // Fly
                float d = Vector2.Distance(_body.position, this.transform.position);
                if (d > 0.2f && !_hammerTime)
                {
                    _body.position = new Vector3(_body.position.x, _body.position.y - _jumpSpeed * Time.deltaTime, _body.position.z);
                    return false;
                }
                else if (d <= 0.1f)
                {
                    _hammerTime = true;
                }

                // defined by coroutine
                //if (!_hammerTime) return false;

                // Impact animations and bools
                ResetJumpAnimation();
                _shadow.SetActive(false);

                // defined by animation
                if (_fallingImpact) return false;

                // compute impact damage to player
                if (Vector2.Distance(_body.position, _target.gameObject.transform.position) <= _playerDistanceFromDamage)
                    _target.GetComponent<Fighter>().DamageBehavior(this.GetComponent<Collider2D>(), _knockbackForce, _jumpDamage);

                _haveJumped = true;
                _timeSinceLastAttack = 0;
                return true;
            }

            return false;
        }

        // O boss irá se dividir em várias(6~9) slimes pequenas(mob normal) que irão seguir o jogador por um tempo de 10s,
        // ou até o player matar 3 delas
        private void Divide()
        {
            //Debug.Log("Dividing");
            if (_target.IsDead()) return;
            if (_target == null) return;

            // boss disappear
            if (!_animator.GetBool("divide"))
            {
                _animator.SetBool("divide", true);
            }

            // spawn slimes
            //StartCoroutine("SpawnDefaultSlimes");
            if (_spawnedDivideSlimes.Count < _numberOfSlimes)
            {
                GameObject slime = Instantiate(_slimesDefaultPrefab, _weaponPoint.position, Quaternion.identity);
                _spawnedDivideSlimes.Add(slime);
                slime.GetComponent<FOV>().SetViewRadius(50);
                slime.GetComponent<Slime>().SetKnockbackForce(15);
                slime.GetComponent<AIController>().SetSuspicionTime(0.1f);
                slime.GetComponent<Inventory.Inventory>().ToggleCanDropItemsOnDeath(false);
                // move slime to random pos

                StartCoroutine(LaunchSlime(slime));
            }
            else
            {
                _countSlimeFollowingTime = true;
                _slimesSpawned = true;
            }

            // slimes behavior for a time
            int countDeadSlimes = 0;
            foreach (GameObject slime in _spawnedDivideSlimes)
                if (slime.GetComponent<Health>().IsDead())
                    countDeadSlimes++;
            // if any slime that died and not discounted from boss health
            if (countDeadSlimes != _countedDeadSlimes)
            {
                int dif = countDeadSlimes - _countedDeadSlimes;
                _health.TakeDamage(_damagePerDeadSlime * dif);
                SetHealthValue(_health.GetHealthPercentage() / 100);
                _countedDeadSlimes = countDeadSlimes;
            }
            if (_slimeFollowingTime <= _slimeFollowTime && countDeadSlimes < 3)// || !_slimesSpawned)
                return;

            // make slimes return to initial point;
            foreach (GameObject slime in _spawnedDivideSlimes)
            {
                if (slime.GetComponent<Health>().IsDead()) continue;
                slime.GetComponent<Mover>().IncreaseSpeed(_slimeReturnSpeedMultiplier);
                AIController slimeAI = slime.GetComponent<AIController>();
                slimeAI.ToggleUpdateTargets(false);
                slimeAI.ClearTargets();
            }

            _reunite = true;
            foreach (GameObject slime in _spawnedDivideSlimes)
            {
                if (slime.GetComponent<Health>().IsDead()) continue;
                float d = Vector2.Distance(slime.transform.position, this.transform.position);
                //Debug.Log("Slimes Distance: " + d);
                if (d > _distanceFromTheBossToReunite)
                    _reunite = false;
            }
            if (!_reunite) return;

            // boss appear
            if (!_animator.GetBool("reunite"))
                _animator.SetBool("reunite", true);

            // destroy the slimes
            //DestroySpawnedSlime();

            _haveDivided = true;
            _timeSinceLastAttack = 0;
            _slimeFollowingTime = 0;
            _countSlimeFollowingTime = false;
            _slimesSpawned = false;
            //_slimesSpawned = false;
        }

        public void DestroySpawnedSlime()
        {
            foreach (GameObject slime in _spawnedDivideSlimes)
            {
                //// boss lose health with relation to how many slimes were killed
                //if (slime.GetComponent<Health>().IsDead())
                //{
                //    _health.TakeDamage(_damagePerDeadSlime);
                //    SetHealthValue(_health.GetHealthPercentage() / 100);
                //}
                slime.GetComponent<Health>().TakeDamage(99999);
                Destroy(slime, 1f);
            }
            _spawnedDivideSlimes.Clear();
        }

        // Divisão + Tiro(Super Move) - O boss irá se dividir em 4 slimes com posições fixas na sala,
        // depois de 2s após ocorrer a divisão, todas as slimes atirarão na direção do player por 4s
        private void SuperMove()
        {
            if (_target.IsDead()) return;
            if (_target == null) return;
            if (_playedFirstSuperMove && !_playedSecondSuperMove && _resetSuperMove)
            {
                ResetSuperMoveVariables();
                _resetSuperMove = false;
            }
            ResetAttackBools();

            // boss disappear
            if (!_animator.GetBool("divide"))
            {
                _animator.SetBool("divide", true);
            }

            // spawn ranged slimes
            if (_spawnedRangedSlimes.Count < _superMoveSlimes)
            {
                GameObject slime = Instantiate(_rangedSlimePrefab, _weaponPoint.position, Quaternion.identity);
                StartCoroutine(LaunchSlimeToPos(slime, _roomCorners[_spawnedRangedSlimes.Count]));


                Slime slimeFighter = slime.GetComponent<Slime>();
                slimeFighter.ToggleAIAttack(false);
                if (_spawnedRangedSlimes.Count == 0)
                    slimeFighter.UpdateWeaponPointPosition(new Vector2(-0.4f, -0.4f));
                if (_spawnedRangedSlimes.Count == 1)
                    slimeFighter.UpdateWeaponPointPosition(new Vector2(-0.4f, 0.4f));
                if (_spawnedRangedSlimes.Count == 2)
                    slimeFighter.UpdateWeaponPointPosition(new Vector2(0.4f, 0.4f));
                if (_spawnedRangedSlimes.Count == 3)
                    slimeFighter.UpdateWeaponPointPosition(new Vector2(0.4f, -0.4f));

                AIController slimeAiController = slime.GetComponent<AIController>();
                slimeAiController.ToggleFixedTarget(true);
                slimeAiController.SetTarget(_target);
                slime.GetComponent<FOV>().SetViewRadius(50);
                slimeAiController.SetSuspicionTime(0);
                slimeAiController.ToggleDontMove(true);
                slime.GetComponent<Inventory.Inventory>().ToggleCanDropItemsOnDeath(false);

                _spawnedRangedSlimes.Add(slime);
            }
            else
            {
                _rangedSlimesSpawned = true;
                _countWaitToShootTime = true;
            }

            if (!_rangedSlimesSpawned)
                return;
            // wait 2 seconds to start shooting
            if (_countingWaitToShootTime <= _waitToShootTime)
                return;
            // set that slimes can attack
            foreach (GameObject slime in _spawnedRangedSlimes)
                slime.GetComponent<Slime>().ToggleAIAttack(true);


            // count slimes shooting time
            _countSlimeShootingTime = true;
            // slimes shooting behavior for a time
            if ((_slimesShootingTime <= _slimeShootTime))
                return;

            // go to center
            // make slimes return to initial point;
            foreach (GameObject slime in _spawnedRangedSlimes)
            {
                if (slime.GetComponent<Health>().IsDead()) continue;
                slime.GetComponent<Mover>().IncreaseSpeed(_slimeReturnSpeedMultiplier);
                AIController slimeAI = slime.GetComponent<AIController>();
                slimeAI.ToggleDontMove(false);
                slimeAI.SetGuardPosition(_mapCenterCoordinate);
                slimeAI.ToggleUpdateTargets(false);
                slimeAI.ClearTargets();
            }
            _reunite = true;
            foreach (GameObject slime in _spawnedRangedSlimes)
            {
                if (slime.GetComponent<Health>().IsDead()) continue;
                float d = Vector2.Distance(slime.transform.position, _mapCenterCoordinate);
                //Debug.Log("Slimes Distance: " + d);
                if (d > _distanceFromTheCenterToReunite)
                    _reunite = false;
            }
            if (!_reunite) return;

            // destroy the slimes
            foreach (GameObject slime in _spawnedRangedSlimes)
            {
                if (slime.GetComponent<Health>().IsDead())
                {
                    _health.TakeDamage(_damagePerDeadSlime);
                    SetHealthValue(_health.GetHealthPercentage() / 100);
                }
                Destroy(slime);
            }
            _spawnedRangedSlimes.Clear();

            // reunite
            transform.position = _mapCenterCoordinate;
            transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z);
            if (!_animator.GetBool("reunite"))
                _animator.SetBool("reunite", true);

            // Reset attack variables 
            _timeSinceLastAttack = 0;

            // set that super move finished
            if (!_playedFirstSuperMove && !_playedSecondSuperMove)
            {
                _playedFirstSuperMove = true;
                _resetSuperMove = true;
            }
            else if (_playedFirstSuperMove && !_playedSecondSuperMove)
            {
                _playedSecondSuperMove = true;
            }
        }

        private void BeingHitBehavior(Collider2D collision)
        {
            if (_health.IsDead()) return;

            float damageBonus = FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().GetDamageBonus();
            float weaponDamage = FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().GetWeaponDamage();
            //Debug.Log("Boss was hit, wpd: " + weaponDamage + ", bd: " + damageBonus);
            _health.TakeDamage(weaponDamage + damageBonus);
            SetHealthValue(_health.GetHealthPercentage() / 100);

            if (_health.IsDead())
                DestroySpawnedSlime();
        }

        // Coroutines

        private IEnumerator LaunchSlime(GameObject slime)
        {
            Vector2 point_00 = _weaponPoint.position;
            Vector2 controlPointRandomOffset = new Vector2(Random.Range(-4, 4), Random.Range(-4, 4));
            Vector2 controlPoint = new Vector2(_weaponPoint.position.x + controlPointRandomOffset.x, _weaponPoint.position.y + controlPointRandomOffset.y);
            Vector2 point_02 = new Vector2(_weaponPoint.position.x + (2 * controlPointRandomOffset.x), _weaponPoint.position.y);
            if (point_02.x >= _rightDownSlimeLaunchLimit.x)
                point_02.x = _rightDownSlimeLaunchLimit.x;
            if (point_02.x <= _leftUpSlimeLaunchLimit.x)
                point_02.x = _leftUpSlimeLaunchLimit.x;

            for (float t = 0f; t <= 1; t += Time.deltaTime)
            {
                Vector3 m1 = Vector3.Lerp(point_00, controlPoint, t);
                Vector3 m2 = Vector3.Lerp(controlPoint, point_02, t);
                slime.transform.position = Vector3.Lerp(m1, m2, t);
                yield return null;
            }
        }

        private IEnumerator LaunchSlimeToPos(GameObject slime, Vector2 toPos)
        {
            Vector2 point_00 = _weaponPoint.position;
            Vector2 controlPoint = new Vector2((_weaponPoint.position.x + toPos.x) / 2, (_weaponPoint.position.y + toPos.y) / 2);

            for (float t = 0f; t <= 1; t += Time.deltaTime)
            {
                Vector3 m1 = Vector3.Lerp(point_00, controlPoint, t);
                Vector3 m2 = Vector3.Lerp(controlPoint, toPos, t);
                slime.transform.position = Vector3.Lerp(m1, m2, t);
                yield return null;
            }
        }

        private IEnumerator FallingBehavior()
        {
            Vector2 groundPos = this.transform.position;
            Vector2 fromPos = _body.position;
            for (float t = 0f; t <= 1f; t += Time.deltaTime)
            {
                _body.position = Vector3.Lerp(fromPos, groundPos, t);
                float d = Vector2.Distance(fromPos, groundPos);
                //Debug.Log("Distance: " + d);
                if (d < 0.5f)
                    _hammerTime = true;
                yield return null;
            }
        }

        // Utils
        private float TargetDistanceFromTheCenter()
        {
            //Vector2 selfPos = new Vector2(this.transform.position.x, this.transform.position.y - 2.5f);
            float distance = Vector2.Distance(this.transform.position, _playerLastKnownPosition);
            //Debug.Log("Distance: " + distance);
            return distance;
        }

        public void MoveTo(Vector2 destination)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, destination, _followSpeed * Time.deltaTime);
        }

        void OnDrawGizmosSelected()
        {
            // Desenho do limite que o jogador pode chegar do centro para ativar o fall
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _thresholdDistanceFromTheCenter);

            // Player last known Position
            if (_playerLastKnownPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(_playerLastKnownPosition, new Vector3(0.5f, 0.5f, 0.5f));
            }

            if (_target != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(_target.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
            }

            // Desenho da posicao em que o body tem que se mover ao pular
            Gizmos.color = Color.green;
            Vector2 flyPos = new Vector2(this.transform.position.x, this.transform.position.y + _jumpHeight);
            Gizmos.DrawCube(flyPos, new Vector3(0.5f, 0.5f, 0.5f));

        }

        // Getters

        // Setters
        public void SetHealthValue(float value)
        {
            _healthImage.fillAmount = value;
        }

        // Resets
        private void ResetAttackBools()
        {
            _haveShooted = _haveJumped = _haveDivided = false;
        }

        public void ResetJumpAnimation()
        {
            _animator.SetBool("fall", false);
            _animator.SetBool("jump", false);
            _shadowAnimator.ResetTrigger("fall");
        }

        private void ResetJumpVariables()
        {
            _updateTargetPosition = true;
            _jumping = false;
            _isFlying = false;
            _countFlyTime = false;
            _flyingTime = 0;
            _hammerTime = false;
            _fallingImpact = false;
            _falling = false;
        }

        private void ResetSuperMoveVariables()
        {
            _reunite = false;
            _countSlimeShootingTime = false;
            _slimesShootingTime = 0;
            _rangedSlimesSpawned = false;
            _countWaitToShootTime = false;
            _countingWaitToShootTime = 0;
        }

        // animation events
        public void ToggleSpriteRenderer(int toggle)
        {
            if (toggle == 1)
                _spriteRenderer.enabled = true;
            else
                _spriteRenderer.enabled = false;
        }

        public void ToggleJumping(int toggle)
        {
            if (toggle == 1)
                _jumping = true;
            else
                _jumping = false;
        }

        public void ToggleFallingImpact(int toggle)
        {
            if (toggle == 1)
                _fallingImpact = true;
            else
                _fallingImpact = false;
        }

        public void LaunchProjectile()
        {
            if (_rangedWeapon.HasProjectile())
                _rangedWeapon.LaunchProjectile(_weaponPoint, _target);
        }

        public void ResetDivideAnimation()
        {
            _animator.SetBool("divide", false);
            _animator.SetBool("reunite", false);
        }

        private void ShakeCamera()
        {
            FindObjectOfType<CameraUtils>().ShakeCamera(_cameraShakeMagnitude, _cameraShakeDuration);
        }

        //_fallingImpact
    }
}