using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputControl))]
public class PlayerCombat : MonoBehaviour
{
    public enum PlayerComboType
    {
        Nm,
        Sp
    }
    [SerializeField] private List<AttackSo> nmCombos;
    [SerializeField] private List<AttackSo> spCombos;

    [SerializeField] private ComboData[] playerComboData;
    private ComboData curComboData;
    
    // private float lastClickedTime;
    private float lastNmComboEnd;
    private int nmComboCounter;
    private int spComboCounter;

    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);

    private Animator _animator;
    private PlayerInputControl _playerInput;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
    }

    private void Update()
    {
        if (_playerInput.attackNormal) NmAttack();
        if (_playerInput.attackSpecial) SpAttack();
        ExitNmAttack();
        ExitSpAttack();
    }

    private void StartCombo(PlayerComboType comboType)
    {
        ComboData comboData = playerComboData[(int)comboType];
        int curComboCounter = comboData.ComboCounter;
        
        if (curComboCounter < comboData.Combos.Count
            && Time.time > comboData.LastComboEnd)
        {
            if (curComboCounter == 0 ||
                (curComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                     && GetCurStateInfo(0).normalizedTime > comboData
                                         .Combos[curComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = comboData.Combos[curComboCounter].animatorOv;
                _animator.Play("Attack",0,0);

                if (!comboData.Combos[curComboCounter].loop) comboData.ComboCounter++;

                if (comboData.ComboCounter >= comboData.Combos.Count) EndCombo(comboType);
            }
        }
    }

    private void EndCombo(PlayerComboType comboType)
    {
        ComboData comboData = playerComboData[(int)comboType];
        int curComboCounter = comboData.ComboCounter;

        if (curComboCounter > 0 && comboData.Combos[curComboCounter - 1].nextComboBeginTime > 0f)
        {
            comboData.LastComboEnd = Time.time + comboData.Combos[curComboCounter - 1].nextComboBeginTime;
        }

        comboData.ComboCounter = 0;
    }
    private void NmAttack()
    {
        if (nmComboCounter < nmCombos.Count && Time.time > lastNmComboEnd)
            // if (Time.time > lastComboEnd + timeBetNormalCombo && comboCounter < combos.Count)
        {
            // CancelInvoke("EndCombo");
            // if (Time.time >= lastClickedTime + .5f)
            if (nmComboCounter == 0 ||
                (nmComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                  && GetCurStateInfo(0).normalizedTime >
                                  nmCombos[nmComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = nmCombos[nmComboCounter].animatorOv;
                _animator.Play("Attack",0,0);
                //weapon.damage=combo[comboCounter].damage;
                // fx,...
                nmComboCounter++;
                // lastClickedTime = Time.time;

                if (nmComboCounter >= nmCombos.Count)
                {
                    print("여기?");
                    EndNmCombo();
                }
            }
        } 
    }
    private void SpAttack()
    {
        if (spComboCounter < spCombos.Count)
        {
            // 새로운 콤보 시작
            if (spComboCounter == 0 ||
                (spComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                    && GetCurStateInfo(0).normalizedTime >
                                    spCombos[spComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = spCombos[spComboCounter].animatorOv;
                _animator.Play("Attack",0,0);
                
                // 루프가 아니면 다음 콤보로 이동
                if(!spCombos[spComboCounter].loop) spComboCounter++;
                
                // 마지막 콤보
                if (spComboCounter > spCombos.Count) spComboCounter = 0;
            }
        } 
    }

    private void ExitNmAttack()
    {
        if (nmComboCounter > nmCombos.Count) return;

        if (nmComboCounter > 0
            && GetCurStateInfo(0).IsTag("Attack")
            && GetCurStateInfo(0).normalizedTime > nmCombos[nmComboCounter - 1].normalizedExitTime)
        {
            // Invoke("EndCombo",1);
            EndNmCombo();
        }
        else if (nmComboCounter > 0 && !GetCurStateInfo(0).IsTag("Attack")) EndNmCombo();
    }
    private void ExitSpAttack()
    {
        if (spComboCounter > spCombos.Count) return;

        if (spComboCounter > 0
            && GetCurStateInfo(0).IsTag("Attack")
            && GetCurStateInfo(0).normalizedTime > spCombos[spComboCounter - 1].normalizedExitTime)
        {
            EndSpCombo();
        }
        else if (spComboCounter > 0 && !GetCurStateInfo(0).IsTag("Attack")) EndSpCombo();
    }

    private void EndNmCombo()
    {
        if (nmComboCounter > 0 && nmCombos[nmComboCounter - 1].nextComboBeginTime > 0f)
        {
            lastNmComboEnd = Time.time + nmCombos[nmComboCounter - 1].nextComboBeginTime;
        }
        nmComboCounter = 0;
        // lastComboEnd = Time.time;
    }

    private void EndSpCombo()
    {
        spComboCounter = 0;
    }
}
