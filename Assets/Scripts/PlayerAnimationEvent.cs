using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _animator;
    public event Action OnRollFinishAction;
    public event Action OnStartComboAction;
    public event Action OnEndComboAction;
    public event Action OnEnableWeaponAction;
    public event Action OnDisableWeaponAction;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        OnRollFinishAction = () => { };
        OnStartComboAction = () => { };
        OnEndComboAction = () => { };
    }
    private void OnRollFinish() => OnRollFinishAction?.Invoke();

    private void OnStartCombo() => OnStartComboAction?.Invoke();
    private void OnEndCombo() => OnEndComboAction?.Invoke();
    private void OnEnableWeapon() => OnEnableWeaponAction?.Invoke();
    private void OnDisableWeapon() => OnDisableWeaponAction?.Invoke();
}
