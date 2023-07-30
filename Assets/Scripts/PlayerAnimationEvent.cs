using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _animator;
    public event Action OnRollFinishAction;
    public event Action OnAssaultAction;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        OnRollFinishAction = () => { };
        OnAssaultAction = () => { };
    }
    private void OnRollFinish() => OnRollFinishAction?.Invoke();
    private void OnAssault() => OnAssaultAction?.Invoke();
}
