using ZL.Combat;
using ZL.Core;
using ZL.Monsters;
using ZL.Saving;
using System.Collections.Generic;
using UnityEngine;

namespace ZL.Movement
{
    public class Mover : MonoBehaviour, IAction, ISavable
    {
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _movementDeadzone = 0.1f;
        [SerializeField] private AnimatorOverrideController _frontAnimationsOverride;
        [SerializeField] private AnimatorOverrideController _backAnimationsOverride;
        [SerializeField] private Transform _weaponPoint;
        [SerializeField] private bool _equalVelocityInDiagonal = true;

        // Cached Reference
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _weaponSpriteRenderer;
        private Animator _weaponAnimator;
        private Rigidbody2D _rigidbody2D;

        private bool _moving = false;
        [SerializeField] private bool _isStopped = false;
        private float _dX, _dY;
        [SerializeField] private bool _isLookingRight = false;
        private bool _isLookingUp = false;
        private bool _isLookingDown = false;
        private Vector2 _speedVector;

        private float _initialSpeed;

        // Debug Varaibles
        private Vector2 _destinationCharge;
        private Vector2 _destinationMove;

        // Start is called before the first frame update
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _weaponSpriteRenderer = _weaponPoint.gameObject.GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            //_rightSideWeaponPointPosition = _weaponPoint.position;
            _initialSpeed = _speed;
            _speedVector = new Vector2(_speed, _speed);
        }

        public bool PlayerMove()
        {
            if (_isStopped) return false;
            _dX = Input.GetAxis("Horizontal");
            _dY = Input.GetAxis("Vertical");

            if (_equalVelocityInDiagonal)
                if (Mathf.Abs(_dX) == Mathf.Abs(_dY))
                {
                    _dX *= 0.75f;
                    _dY *= 0.75f;
                }

            Vector2 destination = new Vector2(_rigidbody2D.position.x + _dX * _speed * Time.deltaTime, _rigidbody2D.position.y + _dY * _speed * Time.deltaTime);


            StartMovementAction();
            UpdateMovementAnimation(_dX, _dY);
            StartMovementAnimation(_dX, _dY);

            return _moving;
        }

        public bool MoveToDestination(Vector2 destination, float speed, bool doMovementAnimation)
        {
            ComputeStep(destination);

            MoveTo(speed);
            if (doMovementAnimation)
            {
                UpdateMovementAnimation(_dX, _dY);
                StartMovementAnimation(_dX, _dY);
            }

            return _moving;
        }

        public bool AIMove(Vector2 destination)
        {
            if (_isStopped) return false;
            _destinationMove = destination;
            ComputeStep(destination);

            UpdateMovementAnimation(_dX, _dY);
            StartMovementAnimation(_dX, _dY);
            StartMovementAction();
            return _moving;
        }

        private void ComputeStep(Vector2 destination)
        {
            Vector2 step = destination - _rigidbody2D.position;
            step.Normalize();
            _dX = step.x;
            _dY = step.y;
        }

        private bool StartMovementAnimation(float dx, float dy)
        {
            float mag = Mathf.Sqrt((dx * dx) + (dy * dy));
            if (mag <= 0 + Mathf.Epsilon + _movementDeadzone)
            {
                if (_animator.GetBool("moving"))
                    _animator.SetBool("moving", false);
                if (_moving)
                    _moving = false;
                return false;
            }

            if (!_animator.GetBool("moving"))
                _animator.SetBool("moving", true);
            if (!_moving)
                _moving = true;
            return true;
        }

        private void UpdateMovementAnimation(float dx, float dy)
        {
            UpdateYAxisAnimations(dy);

            // moving up or down
            UpdateWeaponRotation(dy);

            // moving right
            if (dx > 0 + Mathf.Epsilon + _movementDeadzone)
            {
                _spriteRenderer.flipX = true;
                _isLookingRight = true;

                if (!this.CompareTag("Player") || this.GetComponent<Fighter>().IsAttacking()) return;
                UpdatePlayerWeaponSide();
                return;
            }

            // moving left
            if (dx < 0 - Mathf.Epsilon - _movementDeadzone)
            {
                _spriteRenderer.flipX = false;
                _isLookingRight = false;

                if (!this.CompareTag("Player") || this.GetComponent<Fighter>().IsAttacking()) return;
                UpdatePlayerWeaponSide();
            }
        }

        private void UpdateWeaponRotation(float dy)
        {
            if (tag != "Player") return;
            if (Mathf.Abs(dy) > 0 + Mathf.Epsilon + _movementDeadzone)
            {
                if (_weaponAnimator == null)
                    _weaponAnimator = _weaponPoint.gameObject.GetComponentInChildren<Animator>();
                if (_isLookingUp)
                {
                    if (_isLookingRight)
                        _weaponAnimator.gameObject.transform.eulerAngles = new Vector3(0, 0, 90);//right up
                    else
                        _weaponAnimator.gameObject.transform.eulerAngles = new Vector3(0, 0, -90);//left up
                }
                else // looking down
                {
                    _isLookingDown = true;
                    if (_isLookingRight)
                        _weaponAnimator.gameObject.transform.eulerAngles = new Vector3(0, 0, -90);//right down
                    else
                        _weaponAnimator.gameObject.transform.eulerAngles = new Vector3(0, 0, 90);//left down
                }
            }
            else // just looking left or right
            {
                // not moving up or down
                if (_weaponAnimator == null)
                    _weaponAnimator = _weaponPoint.gameObject.GetComponentInChildren<Animator>();
                if (_weaponAnimator != null)
                    _weaponAnimator.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        public void UpdatePlayerWeaponSide()
        {
            if (_weaponSpriteRenderer == null)
                _weaponSpriteRenderer = _weaponPoint.gameObject.GetComponentInChildren<SpriteRenderer>();

            if (_isLookingRight)
            {
                // define que a arma estará "olhando para a direita"
                _weaponSpriteRenderer.flipX = true;

                //// Weapon must be by player side when attacking
                //if (_weaponPoint.GetComponent<WeaponPoint>().DistanceToPlayer() > 0.2f)
                //    _weaponPoint.GetComponent<WeaponPoint>().TeleportToTarget();// teleport to player side

                // Executa animação de ataque para a esquerda
                if (_weaponAnimator == null)
                    _weaponAnimator = _weaponPoint.gameObject.GetComponentInChildren<Animator>();
                _weaponAnimator.SetBool("attack_left", false);
                //return;
            }
            else
            {
                // define que a arma estará "olhando para a esquerda"
                _weaponSpriteRenderer.flipX = false;

                //// Weapon must be by player side when attacking
                //if (_weaponPoint.GetComponent<WeaponPoint>().DistanceToPlayer() > 0.2f)
                //    _weaponPoint.GetComponent<WeaponPoint>().TeleportToTarget();// teleport to player side

                // Executa animação de ataque para a direita
                if (_weaponAnimator == null)
                    _weaponAnimator = _weaponPoint.gameObject.GetComponentInChildren<Animator>();
                _weaponAnimator.SetBool("attack_left", true);
            }

        }

        private void UpdateYAxisAnimations(float dy)
        {
            if (dy > 0 + Mathf.Epsilon + _movementDeadzone)
            {
                // moving up
                _animator.runtimeAnimatorController = _backAnimationsOverride;
                _isLookingUp = true;
                _isLookingDown = false;
            }
            else if (dy < 0 - Mathf.Epsilon - _movementDeadzone)
            {
                // moving down
                _animator.runtimeAnimatorController = _frontAnimationsOverride;
                _isLookingUp = false;
                _isLookingDown = true;
            }
        }

        public void Cancel()
        {
            //Debug.Log("Cancel Mover Actions");
        }

        public void CancelMove()
        {
            ClearDestinations();
            _dX = _dY = 0;
            _rigidbody2D.velocity = Vector2.zero;
        }

        public void ClearDestinations()
        {
            _destinationCharge = this.transform.position;
            _destinationMove = this.transform.position;
        }

        public void StartMovementAction()
        {
            _actionScheduler.StartAction(this);
            MoveTo();
        }

        public void MoveTo()
        {
            _speedVector.x = _dX * _speed;
            _speedVector.y = _dY * _speed;
            _rigidbody2D.velocity = _speedVector;
        }

        public void MoveTo(float speed)
        {
            _rigidbody2D.velocity = new Vector2(_dX * speed, _dY * speed);
        }

        public void Charge(Vector3 target, float chargeSpeedMultiplier)
        {
            ComputeStep(target);
            UpdateMovementAnimation(_dX, _dY);
            StartMovementAnimation(_dX, _dY);

            _destinationCharge = (Vector2)target;
            this.transform.position = Vector2.MoveTowards(this.transform.position, target, _speed * chargeSpeedMultiplier * Time.deltaTime);
        }

        // UTILS
        public void ToggleStopped(bool toggle)
        {
            _isStopped = toggle;
            _moving = !toggle;
            _animator.SetBool("moving", !toggle);

            //if (this.CompareTag("Player"))
            //    Debug.Log("Toggle Stopped");
        }

        public void AnimationStop(int toggle)
        {
            if (toggle == 1)
            {
                _isStopped = true;
                _moving = false;
                _animator.SetBool("moving", false);
            }
            else
            {
                _isStopped = false;
            }

            //if (this.CompareTag("Player"))
            //    Debug.Log("Toggle Stopped");
        }


        public float DistanceToDestination(Vector2 destination)
        {
            float distance = Vector2.Distance(this.transform.position, destination);
            return distance;
        }

        void OnDrawGizmosSelected()
        {
            if (this.tag == "Player") return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, _destinationMove);

            if (this.GetComponent<Boar>() == null) return;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(this.transform.position, _destinationCharge);
        }

        // GETTERS
        public bool IsLookingRight()
        {
            return _isLookingRight;
        }
        public bool IsLookingUp()
        {
            return _isLookingUp;
        }
        public bool IsLookingDown()
        {
            return _isLookingDown;
        }
        public bool IsMovingRight()
        {
            return _dX > 0 + Mathf.Epsilon + _movementDeadzone;
        }
        public bool IsMovingLeft()
        {
            return _dX < 0 - Mathf.Epsilon - _movementDeadzone;
        }
        public bool IsMovingUp()
        {
            return _dY > 0 + Mathf.Epsilon + _movementDeadzone;
        }
        public bool IsMovingDown()
        {
            return _dY < 0 - Mathf.Epsilon - _movementDeadzone;
        }
        public bool IsStopped()
        {
            return _isStopped;
        }

        public bool GetIsMoving()
        {
            return _moving;
        }

        public float GetSpeed()
        {
            return _speed;
        }

        //SETTERS
        public void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }

        public void IncreaseSpeed(float multiplier)
        {
            _speed = _initialSpeed * multiplier;
        }

        // ANIMATION EVENTS
        private void CanMove()
        {
            //Debug.Log("Toggle Stopped: True, CanMove()");
            ToggleStopped(false);
        }

        // SAVING SYSTEM
        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            this.transform.position = ((SerializableVector3)data["position"]).ToVector();
            this.transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            _isStopped = false;
            if (this.GetComponent<ActionScheduler>() != null)
                this.GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}