using System;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private ComboData[] playerComboData;
    private ComboData curComboData;
    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);

    private Animator _animator;
    // private PlayerAnimationEvent _animationEvent;
    private PlayerInputControl _playerInput;
    private PlayerMove _playerMove;
    private Rigidbody _rigidbody;
    private GameObject _mainCamera;

    [SerializeField, Range(0.01f, 1f)] private float rotationSmoothTime;
    private float rotationSmoothVelocity;

    private Vector3 assaultVelocity;
    private AttackSo curAttackSo;
    
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
        
        // _animationEvent = GetComponentInChildren<PlayerAnimationEvent>();
        // _animationEvent.OnAssaultAction += Assault;
    }

    private void Start()
    {
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
        if (_playerInput.attackNormal) StartCombo(PlayerComboType.Nm);
        if (_playerInput.attackSpecial) StartCombo(PlayerComboType.Sp);
        ExitCombo();
    }

    private void FixedUpdate()
    {
        if (IsComboActive)
        {
            Rotate();
            Assault();
        }
    }

    private void StartCombo(PlayerComboType comboType)
    {
        // 공중이거나 구르기 중 공격 못함
        if (!_playerMove.Grounded || _playerMove.IsRolling) return;
        
        if (curComboData is null) curComboData = playerComboData[(int)comboType];
        // 다른 콤보중인 경우
        else if (curComboData.comboType != comboType) return;
        
        int curComboCounter = curComboData.comboCounter;
        
        if (curComboCounter < curComboData.combos.Count
            && Time.time > curComboData.nextComboStartTime)
        {
            // 첫 콤보 또는 콤보중
            if (curComboCounter == 0 ||
                (curComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                     && GetCurStateInfo(0).normalizedTime > curComboData
                                         .combos[curComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = curComboData.combos[curComboCounter].animatorOv;
                _animator.Play("Attack",0,0);
                
                // 현재 공격 애니메이션 Scriptable Object 설정
                curAttackSo = curComboData.combos[curComboCounter];

                // 반복 콤보가 아니면 다음 콤보 진행
                if (!curComboData.combos[curComboCounter].loop) curComboData.comboCounter++;

                // 콤보 끝 도달
                if (curComboCounter >= curComboData.combos.Count && curComboData.combos[curComboCounter - 1].nextComboInterval > 0f) EndCombo(comboType);
            }
        }
    }

    private void EndCombo(PlayerComboType comboType)
    {
        curComboData = playerComboData[(int)comboType];
        int curComboCounter = curComboData.comboCounter;

        // 다음 콤보 딜레이 적용
        if (curComboCounter >= curComboData.combos.Count && curComboData.combos[curComboCounter - 1].nextComboInterval > 0f)
        {
            curComboData.nextComboStartTime = Time.time + curComboData.combos[curComboCounter - 1].nextComboInterval;
        }

        // 콤보 초기화
        curComboData.comboCounter = 0;
        curComboData = null;
        curAttackSo = null;
    }

    private void ExitCombo()
    {
        if (curComboData is null) return;
        int curComboCounter = curComboData.comboCounter;

        // 공격 상태 벗어남
        // if (!GetCurStateInfo(0).IsTag("Attack")) EndCombo(curComboData.comboType);
        // 콤보 시간 종료
        if (curComboCounter > 0
                 && GetCurStateInfo(0).normalizedTime > curComboData.combos[curComboCounter - 1].normalizedExitTime)
        {
            EndCombo(curComboData.comboType);
        }
    }
    private void Rotate()
    {
        if (curAttackSo is null) return;
        Vector3 cameraForward = _mainCamera.transform.forward;
        cameraForward.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        
        transform.rotation=Quaternion.Euler(0f,
            Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y,
                ref rotationSmoothVelocity, rotationSmoothTime), 0f);
    }

    private void Assault()
    {
        if (curAttackSo is null) return;
        float curAnimNormTime = GetCurStateInfo(0).normalizedTime;
        assaultVelocity = curAttackSo.assaultSpeedCurve.Evaluate(curAnimNormTime) * curAttackSo.assaultDirection;
        _rigidbody.velocity = transform.TransformDirection(assaultVelocity);
    }

    public void TerminateCombo()
    {
        if (curComboData is null) return;
        EndCombo(curComboData.comboType);
    }
}
