using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _animator;
    public event Action onRollFinish;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        onRollFinish = () => { };
    }
    private void OnRollFinish() => onRollFinish?.Invoke();
}
