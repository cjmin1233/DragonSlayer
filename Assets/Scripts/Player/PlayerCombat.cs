using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 콤보가 종료되는 경우는 1. 콤보 도중 구르기 등의 캔슬 액션 사용
// 2. 콤보 시간 내에 입력하지 않아 자연 종료

[RequireComponent(typeof(PlayerInputControl)
    , typeof(PlayerMove)
    , typeof(Rigidbody))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    
    [SerializeField] private PlayerComboType curComboType;

    public bool IsComboActive
    {
        get { return curComboType != PlayerComboType.None; }
    }
    public bool IsAttacking { get; private set; }
    public bool IsGuarding { get; private set; }

    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    private Animator _animator;
    private PlayerInputControl _playerInput;
    private PlayerMove _playerMove;
    private Rigidbody _rigidbody;
    private GameObject _mainCamera;
    private PlayerAnimationEvent _animationEvent;

    [SerializeField, Range(0.01f, 1f)] private float rotationSmoothTime;
    private float rotationSmoothVelocity;

    private Vector3 assaultVelocity;
    
    [SerializeField] private ComboData[] playerComboData;
    // weapon select
    private WeaponType weaponType;
    [SerializeField] private Transform weaponParent;
    [SerializeField] private List<WeaponScriptableObject> weaponScriptableObjects;
    private Weapon activeWeapon;

    private int _animIDAttackSpeed;
    private int _animIDIsGuarding;
    
    private float playerPower;
    private float attackSpeed = 1f;

    [SerializeField] private Transform vfxParent;

    private Quaternion attackTargetRotation = Quaternion.identity;
    
    // Guard
    private Coroutine guardProcess;
    private float guardDuration;
    private float guardTimeOut;
    private float guardTimeOutDelta;
    private float parryingInvincibleDuration;
    private float parryingAngle;
    [SerializeField] private FxAnimator guardFx;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
        _playerMove = GetComponent<PlayerMove>();
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera ??= GameObject.FindGameObjectWithTag("MainCamera");
        _animationEvent = GetComponentInChildren<PlayerAnimationEvent>();
        
        AssignAnimationIDs();
    }
    private void AssignAnimationIDs()
    {
        _animIDAttackSpeed = Animator.StringToHash("AttackSpeed");
        _animIDIsGuarding = Animator.StringToHash("IsGuarding");
    }

    private void OnEnable()
    {
        CombatInit(playerScriptableObject);
    }

    public void CombatInit(PlayerScriptableObject playerSo)
    {
        playerPower = playerSo.playerPower;
        attackSpeed = playerSo.attackSpeed;
        guardDuration = playerSo.guardDuration;
        guardTimeOut = playerSo.guardTimeOut;
        parryingInvincibleDuration = playerSo.parryingInvincibleDuration;
        parryingAngle = playerSo.parryingAngle;
    }

    private void Start()
    {
        _animationEvent.OnStartComboAction += StartCombo;
        _animationEvent.OnEndComboAction += EndCombo;
        _animationEvent.OnEnableWeaponAction += EnableWeapon;
        _animationEvent.OnDisableWeaponAction += DisableWeapon;
        _animationEvent.OnEnableVfxAction += EnableVfx;
        _animationEvent.OnEndParryingAction += EndParrying;

        curComboType = PlayerComboType.None;

        
        foreach (var comboData in playerComboData)
        {
            comboData.InitComboData(vfxParent);
        }

        // weapon select
        WeaponScriptableObject weaponSo = weaponScriptableObjects.Find(weapon => weapon.type == weaponType);
        if (weaponSo is null)
        {
            Debug.LogError($"No WeaponScriptableObject found for WeaponType: {weaponType}");
            return;
        }
        activeWeapon = weaponSo.Spawn(weaponParent, this);
    }

    private void Update()
    {
        if (_playerInput.AttackNormal) Attack(PlayerComboType.Nm);
        if (_playerInput.AttackSpecial) Attack(PlayerComboType.Sp);
        if (_playerInput.Guard) Attempt2Guard();

        if (guardTimeOutDelta >= 0f) guardTimeOutDelta -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        Rotate2Target();
        Assault();
    }

    #region Combo Attack

    

    private void Attack(PlayerComboType comboType)
    {
        // 공중이거나 구르기 중, 공격 중일 때 리턴
        if (!_playerMove.Grounded || _playerMove.IsRolling || IsAttacking || IsGuarding) return;

        ComboData comboData = playerComboData[(int)comboType];
        if (comboData.comboCounter < comboData.Combos.Count
            && Time.time > comboData.nextComboStartTime)
        {
            ComboAnimation comboAnimation = comboData.Combos[comboData.comboCounter];
            if (!PlayerHealth.Instance.IsVitalityEnough(comboAnimation.ComboVitality))
            {
                print("콤보 기력이 부족합니다.");
                return;
            }
            // 다른 콤보 입력
            if (IsComboActive && !curComboType.Equals(comboType))
            {
                EndCombo();
            }
            curComboType = comboType;
            activeWeapon.WeaponSetup(comboAnimation);

            comboAnimation.EffectIndex = 0;
            _animator.runtimeAnimatorController = comboAnimation.AnimatorOv;
            _animator.SetFloat(_animIDAttackSpeed, attackSpeed);
            _animator.Play("Attack", 0, 0);
            PlayerHealth.Instance.SpendVitality(comboAnimation.ComboVitality);
            IsAttacking = true;
        }
    }

    private void StartCombo()
    {
        if (!IsComboActive) return;
        ComboData comboData = playerComboData[(int)curComboType];
        if (!comboData.Combos[comboData.comboCounter].Loop) comboData.comboCounter++;
        
        IsAttacking = false;
    }

    private void EndCombo()
    {
        if (!IsComboActive) return;
        ComboData comboData = playerComboData[(int)curComboType];
        if (comboData.comboCounter >= comboData.Combos.Count
            && comboData.Combos[comboData.comboCounter - 1].NextComboInterval > 0f)
            comboData.nextComboStartTime = Time.time + comboData.Combos[comboData.comboCounter - 1].NextComboInterval;

        comboData.comboCounter = 0;
        IsAttacking = false;
        curComboType = PlayerComboType.None;
        DisableWeapon();

        _rigidbody.velocity = Vector3.zero;
    }

    public void TerminateCombat()
    {
        if (IsComboActive)
        {
            ComboData comboData = playerComboData[(int)curComboType];
            comboData.comboCounter = 0;
        }
        
        IsAttacking = false;
        curComboType = PlayerComboType.None;
        DisableWeapon();

        if (IsGuarding) EndGuard();
        
        _rigidbody.velocity = Vector3.zero;
    }
    #endregion
    private void Rotate2Target()
    {
        if (!IsAttacking && !IsGuarding) return;
        
        if (IsAttacking && playerComboData[(int)curComboType].comboCounter == 0)
        {
            Vector3 cameraForward = _mainCamera.transform.forward;
            cameraForward.y = 0f;
            attackTargetRotation = Quaternion.LookRotation(cameraForward);
        }

        // Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        
        transform.rotation=Quaternion.Euler(0f,
            Mathf.SmoothDampAngle(transform.eulerAngles.y, attackTargetRotation.eulerAngles.y,
                ref rotationSmoothVelocity, rotationSmoothTime), 0f);
    }

    private void Assault()
    {
        if (!IsAttacking) return;
        float curAnimNormTime = GetCurStateInfo(0).normalizedTime;
        
        ComboData comboData = playerComboData[(int)curComboType];
        assaultVelocity = comboData.Combos[comboData.comboCounter].AssaultSpeedCurve.Evaluate(curAnimNormTime) 
                          * attackSpeed * comboData.Combos[comboData.comboCounter].AssaultDirection;
        _rigidbody.velocity = transform.TransformDirection(assaultVelocity);
    }
    private void EnableWeapon()
    {
        if (activeWeapon is not null) activeWeapon.EnableWeapon();
    }
    private void DisableWeapon()
    {
        if (activeWeapon is not null) activeWeapon.DisableWeapon();
    }

    private void EnableVfx()
    {
        if(IsComboActive)
        {
            ComboData comboData = playerComboData[(int)curComboType];
            comboData.Combos[comboData.comboCounter].EnableParticle(attackSpeed);
        }
    }

    #region Guard
    private void Attempt2Guard()
    {
        // 가드 시작 > n초동안 가드 유지(코루틴) 1. > 노피격시 종료 
        //                                    2. > 중간에 피격 > 패링 함수 호출(이전 코루틴 종료, 무적 시간 코루틴 시작) > 애니메이션 종료
        if (!_playerMove.Grounded || _playerMove.IsRolling || IsGuarding) return;
        if (guardTimeOutDelta >= 0f) return;
        
        TerminateCombat();
        
        if (guardProcess is not null) StopCoroutine(guardProcess);
        guardProcess = StartCoroutine(Guarding());
    }
    
    private IEnumerator Guarding()
    {
        float timer = guardDuration;
        IsGuarding = true;
        _rigidbody.velocity = Vector3.zero;
        _animator.SetBool(_animIDIsGuarding, true);
        
        if(guardFx is not null)guardFx.EnableFx();

        // 가드시작시 방향전환
        Vector3 cameraForward = _mainCamera.transform.forward;
        cameraForward.y = 0f;
        attackTargetRotation = Quaternion.LookRotation(cameraForward);
        
        // TerminateCombat();
        
        while (IsGuarding)
        {
            timer -= Time.deltaTime;
            // 가드 시간 종료 또는 가드 해제시
            if (timer <= 0f || !_playerInput.Guard) EndGuard();

            yield return null;
        }
    }

    public float CheckParrying(GameObject damager)
    {
        Vector3 parryingDirection = damager.transform.position - transform.position;
        float parryingAngle = MyUtility.AngleBetweenVectors(transform.forward, parryingDirection);

        // 패링 각도(범위)로 판단
        if (parryingAngle <= this.parryingAngle)
        {
            // 패링 성공, 가드 해제
            guardTimeOutDelta = guardTimeOut;
            if (guardFx is not null) guardFx.DisableFx();
            if (guardProcess is not null) StopCoroutine(guardProcess);
            
            _animator.SetBool("Parry", true);
            
            // 패링 성공시 무적 시간값 리턴
            return parryingInvincibleDuration;
        }
        
        // 패링 실패시 -값 리턴
        return -1f;

    }
    private void EndParrying()
    {
        IsGuarding = false;
        _animator.SetBool(_animIDIsGuarding, false);
        _animator.SetBool("Parry", false);
    }
    private void EndGuard()
    {
        guardTimeOutDelta = guardTimeOut;
        if (guardFx is not null) guardFx.DisableFx();
        
        IsGuarding = false;
        _animator.SetBool(_animIDIsGuarding, false);
    }
    #endregion

    public void AddCombo(ComboScriptableObject comboScriptableObject)
    {
        var comboType = comboScriptableObject.comboType;
        if (comboType.Equals(PlayerComboType.None))
        {
            Debug.LogError("Combo type invalid");
            return;
        }
        playerComboData[(int)comboType].AddCombo(comboScriptableObject, vfxParent);
    }

    public float CalculateDamage()
    {
        if (activeWeapon is null || !IsComboActive) return 0f;
        ComboData comboData = playerComboData[(int)curComboType];
        return activeWeapon.WeaponDamage * comboData.Combos[comboData.comboCounter].AnimationDamage * playerPower;
    }

    public void UpgradeStatus(float power, float speed)
    {
        // 전투 능력치 업그레이드
        playerPower += power;
        attackSpeed += speed;
    }
}
