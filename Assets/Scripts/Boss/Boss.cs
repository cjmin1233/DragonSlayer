using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum BossPatternType
{
    Nm,
    Sp,
    None
}
public class Boss : LIvingEntity
{
    private bool isFlying;
    private enum FlyingState
    {
        Float,
        Glide,
        Attack
    }

    private enum GroundState
    {
        Patrol,
        Track,
        Attack,
        Groggy,
        Sleep
    }

    private GroundState groundState;
    private FlyingState flyingState;

    private Animator _animator;

    [SerializeField] private Transform patternContainer;
    [SerializeField] private BossPatternData[] patternData;
    private Coroutine curPatternRoutine;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        groundState = GroundState.Patrol;
        
        foreach (var data in patternData)
        {
            data.InitPatternData(patternContainer);;
        }

        StartCoroutine(BossBrain());
    }

    private IEnumerator BossBrain()
    {
        while (true)
        {
            if (!isFlying)
            {
                if (groundState.Equals(GroundState.Patrol))
                {
                    // 패턴 선택
                    var selectedPattern = patternData[0].SelectPatternAction();
                    if (selectedPattern is not null)
                    {
                        if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                        curPatternRoutine = StartCoroutine(PatternRoutine(selectedPattern));
                        groundState = GroundState.Attack;
                        _animator.runtimeAnimatorController = selectedPattern.animatorOv;
                        _animator.Play("Pattern", 0, 0f);
                        print("패턴 발동");
                    }
                }
                else if (groundState.Equals(GroundState.Attack))
                {
                    // 공격 중...
                }
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    private IEnumerator PatternRoutine(BossPatternAction patternAction)
    {
        yield return new WaitUntil(() => _animator.GetBool("Exit"));
        patternAction.patternEnableTime = Time.time + patternAction.patternCooldown;
        _animator.SetBool("Exit", false);
        groundState = GroundState.Patrol;
    }
    public override void TakeDamage(DamageMessage damageMessage)
    {
        return;
    }
}
