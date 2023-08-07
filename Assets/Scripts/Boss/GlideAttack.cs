using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GlideAttack : BossPatternAction
{
    public override IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }

        Debug.Log("낙하 패턴 시작");
        
        _animator.runtimeAnimatorController = animatorOv[0];
        _animator.Play("Pattern", 0, 0f);
        yield return new WaitUntil(() => _animator.GetBool("Exit"));
        _animator.SetBool("Exit", false);
        _animator.runtimeAnimatorController = animatorOv[1];
        _animator.Play("Pattern", 0, 0f);
        yield return new WaitUntil(() => _animator.GetBool("Exit"));
        patternEnableTime = Time.time + patternCooldown;
        _boss.EndPattern();
    }
}
