using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private List<AttackSO> normalCombos;
    [SerializeField] private List<AttackSO> specialCombos;
    
    // [SerializeField] private float timeBetNormalCombo;
    // private float lastClickedTime;
    // private float lastComboEnd;
    private int normComboCounter;
    private int spComboCounter;

    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);

    private Animator _animator;
    private PlayerInputControl _playerInput;
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
    }

    private void Update()
    {
        if (_playerInput.attackNormal) NormalAttack();
        if (_playerInput.attackSpecial) SpAttack();
        ExitNormalAttack();
        ExitSpAttack();
    }
    private void NormalAttack()
    {
        if (normComboCounter < normalCombos.Count)
            // if (Time.time > lastComboEnd + timeBetNormalCombo && comboCounter < combos.Count)
        {
            // CancelInvoke("EndCombo");
            // if (Time.time >= lastClickedTime + .5f)
            if (normComboCounter == 0 ||
                (normComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                  && GetCurStateInfo(0).normalizedTime >
                                  normalCombos[normComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = normalCombos[normComboCounter].animatorOV;
                _animator.Play("Attack",0,0);
                //weapon.damage=combo[comboCounter].damage;
                // fx,...
                normComboCounter++;
                // lastClickedTime = Time.time;

                if (normComboCounter > normalCombos.Count) normComboCounter = 0;
            }
        } 
    }
    private void SpAttack()
    {
        if (spComboCounter < specialCombos.Count)
        {
            if (spComboCounter == 0 ||
                (spComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                    && GetCurStateInfo(0).normalizedTime >
                                    specialCombos[spComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = specialCombos[spComboCounter].animatorOV;
                _animator.Play("Attack",0,0);
                
                spComboCounter++;
                
                if (spComboCounter > specialCombos.Count) spComboCounter = 0;
            }
        } 
    }

    private void ExitNormalAttack()
    {
        if (normComboCounter > normalCombos.Count) return;

        if (normComboCounter > 0
            && GetCurStateInfo(0).IsTag("Attack")
            && GetCurStateInfo(0).normalizedTime > normalCombos[normComboCounter - 1].normalizedExitTime)
        {
            // Invoke("EndCombo",1);
            EndNormalCombo();
        }
        else if (normComboCounter > 0 && !GetCurStateInfo(0).IsTag("Attack")) EndNormalCombo();
    }
    private void ExitSpAttack()
    {
        if (spComboCounter > specialCombos.Count) return;

        if (spComboCounter > 0
            && GetCurStateInfo(0).IsTag("Attack")
            && GetCurStateInfo(0).normalizedTime > specialCombos[spComboCounter - 1].normalizedExitTime)
        {
            EndSpCombo();
        }
        else if (spComboCounter > 0 && !GetCurStateInfo(0).IsTag("Attack")) EndSpCombo();
    }

    private void EndNormalCombo()
    {
        normComboCounter = 0;
        // lastComboEnd = Time.time;
    }

    private void EndSpCombo()
    {
        spComboCounter = 0;
    }
}
