using System;
using System.Collections.Generic;
using UnityEngine;

// 콤보가 종료되는 경우는 1. 콤보 도중 구르기 등의 캔슬 액션 사용
// 2. 콤보 시간 내에 입력하지 않아 자연 종료

[RequireComponent(typeof(PlayerInputControl), typeof(PlayerMove))]
public class PlayerCombat : MonoBehaviour
{
    public enum PlayerComboType
    {
        Nm,
        Sp
    }

    public bool IsComboActive
    {
        get
        {
            return curComboData is not null;
        }
    }

    private ComboData curComboData;
    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    public bool IsAttacking { get; private set; }

    private Animator _animator;
    private PlayerInputControl _playerInput;
    private PlayerMove _playerMove;
    private Rigidbody _rigidbody;
    private GameObject _mainCamera;
    private PlayerAnimationEvent _animationEvent;

    [SerializeField, Range(0.01f, 1f)] private float rotationSmoothTime;
    private float rotationSmoothVelocity;

    private Vector3 assaultVelocity;
    private AttackSo curAttackSo;
    
    [SerializeField] private ComboData[] playerComboData;
    // weapon select
    private WeaponType weaponType;
    [SerializeField] private Transform weaponParent;
    [SerializeField] private List<WeaponScriptableObject> weapons;

    [SerializeField] private WeaponScriptableObject activeWeapon;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
        _playerMove = GetComponent<PlayerMove>();
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera ??= GameObject.FindGameObjectWithTag("MainCamera");
        _animationEvent = GetComponentInChildren<PlayerAnimationEvent>();
        _animationEvent.OnStartComboAction += Start_Combo;
        _animationEvent.OnEndComboAction += End_Combo;
    }

    private void Start()
    {
        // weapon select
        WeaponScriptableObject weapon = weapons.Find(weapon => weapon.type == weaponType);
        if (weapon is null)
        {
            Debug.LogError($"No WeaponScriptableObject found for WeaponType: {weaponType}");
            return;
        }
        activeWeapon = weapon;
        weapon.Spawn(weaponParent, this);
    }

    private void Update()
    {
        if (_playerInput.attackNormal) Attack(PlayerComboType.Nm);
        if (_playerInput.attackSpecial) Attack(PlayerComboType.Sp);
    }

    private void FixedUpdate()
    {
        if (curAttackSo is not null)
        {
            Rotate();
            Assault();
        }
    }

    private void Attack(PlayerComboType comboType)
    {
        // 공중이거나 구르기 중 공격 못함
        if (!_playerMove.Grounded || _playerMove.IsRolling) return;

        if (curComboData is null) curComboData = playerComboData[(int)comboType];
        else if (curComboData.comboType != comboType) return;

        int curComboCounter = curComboData.comboCounter;
        if (curComboCounter < curComboData.combos.Count
            && Time.time > curComboData.nextComboStartTime
            && !IsAttacking)
        {
            curAttackSo = curComboData.combos[curComboCounter];
            _animator.runtimeAnimatorController = curAttackSo.animatorOv;
            _animator.Play("Attack", 0, 0);
            IsAttacking = true;
        }
    }

    private void Start_Combo()
    {
        if (!curAttackSo.loop) curComboData.comboCounter++;
        IsAttacking = false;
    }

    private void End_Combo()
    {
        if (curComboData.comboCounter >= curComboData.combos.Count
            && curAttackSo.nextComboInterval > 0f) curComboData.nextComboStartTime = Time.time + curAttackSo.nextComboInterval;

        curComboData.comboCounter = 0;
        IsAttacking = false;
        curComboData = null;
        curAttackSo = null;
    }

    public void Terminate_Combo()
    {
        if (curComboData is null) return;
        End_Combo();
    }
    // private void Update()
    // {
    //     if (_playerInput.attackNormal) StartCombo(PlayerComboType.Nm);
    //     else if (_playerInput.attackSpecial) StartCombo(PlayerComboType.Sp);
    //     ExitCombo();
    // }

    // private void FixedUpdate()
    // {
    //     print(IsComboActive);
    //     if (IsComboActive)
    //     {
    //         Rotate();
    //         Assault();
    //     }
    // }

    // private void StartCombo(PlayerComboType comboType)
    // {
    //     // 공중이거나 구르기 중 공격 못함
    //     if (!_playerMove.Grounded || _playerMove.IsRolling) return;
    //     
    //     if (curComboData is null) curComboData = playerComboData[(int)comboType];
    //     // 다른 콤보중인 경우
    //     else if (curComboData.comboType != comboType)
    //     {
    //         print("다른 콤보 활성중");
    //         return;
    //     }
    //     
    //     int curComboCounter = curComboData.comboCounter;
    //     
    //     if (curComboCounter < curComboData.combos.Count
    //         && Time.time > curComboData.nextComboStartTime)
    //     {
    //         // 첫 콤보 또는 콤보중
    //         if (curComboCounter == 0 ||
    //             (curComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
    //                                  && GetCurStateInfo(0).normalizedTime > curAttackSo.normalizedComboTime))
    //         {
    //             // 공격 애니메이션 실행
    //             _animator.runtimeAnimatorController = curComboData.combos[curComboCounter].animatorOv;
    //             _animator.Play("Attack",0,0);
    //             
    //             // 현재 공격 애니메이션 Scriptable Object 설정
    //             curAttackSo = curComboData.combos[curComboCounter];
    //             
    //             // 반복 콤보가 아니면 다음 콤보 진행
    //             if (!curAttackSo.loop) curComboData.comboCounter++;
    //
    //             // // 콤보 끝 도달
    //             // if (curComboCounter >= curComboData.combos.Count && curAttackSo.nextComboInterval > 0f) EndCombo(comboType);
    //         }
    //     }
    // }
    //
    // private void EndCombo(PlayerComboType comboType)
    // {
    //     curComboData = playerComboData[(int)comboType];
    //     int curComboCounter = curComboData.comboCounter;
    //
    //     // 다음 콤보 딜레이 적용
    //     if (curComboCounter >= curComboData.combos.Count && curAttackSo.nextComboInterval > 0f)
    //     {
    //         curComboData.nextComboStartTime = Time.time + curAttackSo.nextComboInterval;
    //     }
    //
    //     // 콤보 초기화
    //     curComboData.comboCounter = 0;
    //     playerComboData[(int)comboType] = curComboData;
    //     
    //     curComboData = null;
    //     curAttackSo = null;
    // }
    //
    // private void ExitCombo()
    // {
    //     if (!IsComboActive) return;
    //     int curComboCounter = curComboData.comboCounter;
    //     
    //     // 콤보 시간 종료
    //     if (curComboCounter > 0
    //         && GetCurStateInfo(0).IsTag("Attack")
    //         && GetCurStateInfo(0).normalizedTime > curAttackSo.normalizedExitTime)
    //     {
    //         print("콤보 시간 종료");
    //         EndCombo(curComboData.comboType);
    //     }
    // }
    private void Rotate()
    {
        Vector3 cameraForward = _mainCamera.transform.forward;
        cameraForward.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        
        transform.rotation=Quaternion.Euler(0f,
            Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y,
                ref rotationSmoothVelocity, rotationSmoothTime), 0f);
    }

    private void Assault()
    {
        float curAnimNormTime = GetCurStateInfo(0).normalizedTime;
        assaultVelocity = curAttackSo.assaultSpeedCurve.Evaluate(curAnimNormTime) * curAttackSo.assaultDirection;
        _rigidbody.velocity = transform.TransformDirection(assaultVelocity);
    }

    // public void TerminateCombo()
    // {
    //     if (!IsComboActive) return;
    //     EndCombo(curComboData.comboType);
    // }
}
