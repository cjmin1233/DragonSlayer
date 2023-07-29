using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combos;

    [SerializeField] private float timeBetNormalCombo;
    // private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;

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
        if (_playerInput.attackNormal) Attack();
        ExitAttack();
    }

    private void Attack()
    {
        if (comboCounter < combos.Count)
            // if (Time.time > lastComboEnd + timeBetNormalCombo && comboCounter < combos.Count)
        {
            // CancelInvoke("EndCombo");
            // if (Time.time >= lastClickedTime + .5f)
            if (comboCounter == 0 ||
                (comboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                  && GetCurStateInfo(0).normalizedTime >
                                  combos[comboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = combos[comboCounter].animatorOV;
                _animator.Play("Attack",0,0);
                //weapon.damage=combo[comboCounter].damage;
                // fx,...
                comboCounter++;
                // lastClickedTime = Time.time;

                if (comboCounter > combos.Count) comboCounter = 0;
            }
        } 
    }

    private void ExitAttack()
    {
        if (comboCounter > combos.Count) return;

        if (comboCounter > 0
            && GetCurStateInfo(0).IsTag("Attack")
            && GetCurStateInfo(0).normalizedTime > combos[comboCounter - 1].normalizedExitTime)
        {
            print("exit attack");
            // Invoke("EndCombo",1);
            EndCombo();
        }
        else if (comboCounter > 0 && !GetCurStateInfo(0).IsTag("Attack")) EndCombo();
    }

    private void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}
