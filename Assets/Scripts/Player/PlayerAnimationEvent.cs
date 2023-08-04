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
    public event Action OnEnableVfxAction;
    public event Action OnEndParryingAction;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        OnRollFinishAction = () => { };
        OnStartComboAction = () => { };
        OnEndComboAction = () => { };
        OnEnableVfxAction = () => { };
        OnEndParryingAction = () => { };
    }
    private void OnRollFinish() => OnRollFinishAction?.Invoke();

    private void OnStartCombo() => OnStartComboAction?.Invoke();
    private void OnEndCombo() => OnEndComboAction?.Invoke();
    private void OnEnableWeapon() => OnEnableWeaponAction?.Invoke();
    private void OnDisableWeapon() => OnDisableWeaponAction?.Invoke();
    private void OnEnableVfx() => OnEnableVfxAction?.Invoke();
    private void OnEndParrying() => OnEndParryingAction?.Invoke();
}
