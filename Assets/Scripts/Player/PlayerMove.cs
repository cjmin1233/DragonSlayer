using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInputControl))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float moveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float sprintSpeed = 5.335f;

    [Tooltip("Roll speed of the character in m/s")]
    public float rollSpeed = 10f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float speedChangeRate = 10.0f;

    //public AudioClip LandingAudioClip;
    //public AudioClip[] FootstepAudioClips;
    //[Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    
    [Space(10)]
    [Tooltip("The amount of force applied when player jump"), Range(5f, 20f)]
    public float jumpForce = 10f;
    
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float jumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float fallTimeout = 0.15f;

    [Tooltip("Time required to pass before being able to roll again.")]
    public float rollTimeout = 1f;

    public bool Grounded { get; private set; }

    [Tooltip("Useful for rough ground")]
    public float groundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float groundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask groundLayers;

    
    [Header("CineMachine")]
    [Tooltip("The follow target set in the CineMachine Virtual Camera that the camera will follow")]
    public GameObject cineMachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float topClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float bottomClamp = -30.0f;

    [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
    public float cameraAngleOverride;

    [Tooltip("For locking the camera position on all axis")]
    public bool lockCameraPosition;

    // cineMachine
    private float _cineMachineTargetYaw;
    private float _cineMachineTargetPitch;

    // player
    private float _speed;
    private float _speedSmoothVelocity;
    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    //private float _terminalVelocity = 53.0f;
    private float _camYawVelocity;

    // timeout delta time
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _rollTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    // private int _animIDMotionSpeed;
    private int _animIDRoll;

    private PlayerInputControl _playerInput;
    private Animator _animator;
    private Rigidbody _rigidBody;
    private PlayerCombat _playerCombat;
    private PlayerAnimationEvent _animationEvent;
    
    private GameObject _mainCamera;

    private const float Threshold = 0.01f;

    private bool HasAnimator => _animator is not null;

    public bool IsRolling { get; private set; }
    private Coroutine rolling;
    private float rollVitality;
    private void Awake()
    {
        // get a reference to our main camera
        _mainCamera ??= GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void OnEnable()
    {
        MovementInit(playerScriptableObject);
    }

    public void MovementInit(PlayerScriptableObject playerSo)
    {
        moveSpeed = playerSo.moveSpeed;
        sprintSpeed = playerSo.sprintSpeed;
        rollSpeed = playerSo.rollSpeed;
        rollVitality = playerSo.rollVitality;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _cineMachineTargetYaw = cineMachineCameraTarget.transform.rotation.eulerAngles.y;

        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
        _rigidBody = GetComponent<Rigidbody>();
        _playerCombat = GetComponent<PlayerCombat>();

        _animationEvent = GetComponentInChildren<PlayerAnimationEvent>();
        _animationEvent.OnRollFinishAction += RollFinish;

        AssignAnimationIDs();

        // reset our timeouts on start
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
        _rollTimeoutDelta = -1f;
    }
    private void Update()
    {
        if (Grounded && !IsRolling)
        {
            if (_playerInput.Jump 
                && _jumpTimeoutDelta <= 0f 
                && !_playerCombat.IsAttacking 
                && !_playerCombat.IsGuarding) Jump();
            if (_playerInput.Roll && _rollTimeoutDelta <= 0f) Roll();
        }
        
        GroundedCheck();

        // if (_playerInput.jump) SelfDamage();
    }
    private void FixedUpdate()
    {
        VerticalMovement();
        HorizontalMovement();
        CameraRotation();
    }
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        // _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        _animIDRoll = Animator.StringToHash("Roll");
    }
    private void Jump()
    {
        // add force to rigid body
        _rigidBody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);

        // update animator if using character
        if (HasAnimator)
        {
            _animator.SetBool(_animIDJump, true);
        }
    }
    private void Roll()
    {
        if (!PlayerHealth.Instance.IsVitalityEnough(rollVitality))
        {
            Debug.Log("구르기 기력이 부족합니다.");
            return;
        }
        if(rolling is not null) StopCoroutine(rolling);
        rolling = StartCoroutine(Rolling());
        PlayerHealth.Instance.SpendVitality(rollVitality);
    }
    private void VerticalMovement()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = fallTimeout;

            // update animator if using character
            if (HasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.fixedDeltaTime;
            }

        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.fixedDeltaTime;
            }
            else
            {
                // update animator if using character
                if (HasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }
        }
    }
    private void Rotate()
    {
        #region Rotation
        // input direction
        Vector3 inputDirection = new(_playerInput.MoveInput.x, 0f, _playerInput.MoveInput.y);

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_playerInput.MoveInput != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                rotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        else _targetRotation = transform.eulerAngles.y;

        #endregion
    }
    private void HorizontalMovement()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _playerInput.Sprint && Grounded ? sprintSpeed : moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        float inputMagnitude = _playerInput.MoveInput.magnitude;
        if (inputMagnitude < 0.01f) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        Vector3 currentHorizontalVelocity = _rigidBody.velocity;
        currentHorizontalVelocity.y = 0f;
        // float currentHorizontalSpeed = new Vector3(_rigidBody.velocity.x, 0.0f, _rigidBody.velocity.z).magnitude;
        
        _speed = Mathf.SmoothDamp(currentHorizontalVelocity.magnitude, targetSpeed * inputMagnitude, ref _speedSmoothVelocity, Time.fixedDeltaTime);
        // round speed to 3 decimal places
        //_speed = Mathf.Round(_speed * 1000f) / 1000f;

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.fixedDeltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        // update animator if using character
        if (HasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        if (_playerCombat.IsAttacking || _playerCombat.IsGuarding) return;
        Rotate();
        if (IsRolling) return;

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        Vector3 targetVelocity = targetDirection.normalized * _speed + Vector3.up * _rigidBody.velocity.y;
        _rigidBody.velocity = targetVelocity;
        
    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = transform.position;
        spherePosition.y -= groundedOffset;
        
        Grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (HasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }
    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_playerInput.LookInput.sqrMagnitude >= Threshold && !lockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            _cineMachineTargetYaw += _playerInput.LookInput.x;
            _cineMachineTargetPitch += _playerInput.LookInput.y;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cineMachineTargetYaw = ClampAngle(_cineMachineTargetYaw, float.MinValue, float.MaxValue);
        _cineMachineTargetPitch = ClampAngle(_cineMachineTargetPitch, bottomClamp, topClamp);

        // CineMachine will follow this target
        cineMachineCameraTarget.transform.rotation = Quaternion.Euler(_cineMachineTargetPitch + cameraAngleOverride,
            _cineMachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    /*
    public void OnFootstep(AnimationEvent animationEvent)
    {
        print("foot step!");
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            //if (FootstepAudioClips.Length > 0)
            //{
            //    var index = Random.Range(0, FootstepAudioClips.Length);
            //    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            //}
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            //AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }*/
    private IEnumerator Rolling()
    {
        IsRolling = true;
        _playerCombat.TerminateCombat();
        _rollTimeoutDelta = rollTimeout;
        if (HasAnimator)
        {
            _animator.SetBool(_animIDRoll, true);
        }
        Rotate();
        Vector3 rollingDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward; 
        _rigidBody.velocity = rollingDirection.normalized * rollSpeed;
        
        while (_rollTimeoutDelta > 0f)
        {
            _rollTimeoutDelta -= Time.deltaTime;

            yield return null;
        }
    }
    private void RollFinish()
    {
        IsRolling = false;
        _rigidBody.velocity = Vector3.zero;
        if (HasAnimator)
        {
            _animator.SetBool(_animIDRoll, false);
        }
    }

    //private void SelfDamage()
    //{
    //    DamageMessage dmg;
    //    dmg.damager = null;
    //    dmg.damage = 10f;
    //    dmg.stunTime = 0;

    //    LIvingEntity lIvingEntity = GetComponent<LIvingEntity>();
    //    if (lIvingEntity is not null) lIvingEntity.TakeDamage(dmg);
    //}
}
