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
}
