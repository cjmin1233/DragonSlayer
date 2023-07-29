using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combos;

    private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;

    private Animator _animator;
    private PlayerInputControl _playerInput;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
    }

    private void Update()
    {
        if (_playerInput.attackNormal) Attack();
        ExitAttack();
    }

    private void Attack()
    {
        if (Time.time - lastComboEnd > .2f && comboCounter < combos.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= .2f)
            {
                _animator.runtimeAnimatorController = combos[comboCounter].animatorOV;
                _animator.Play("Attack",0,0);
                //weapon.damage=combo[comboCounter].damage;
                // fx,...
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= combos.Count) comboCounter = 0;
            }
        } 
    }

    private void ExitAttack()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            print("exit attack");
            Invoke("EndCombo",1);
        }
    }

    private void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}
