using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingAttack : BossPatternAction
{
    private float rotateTime = .5f;
    public override IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }

        Debug.Log(gameObject.name + " : 일반 패턴 시작");
        curAnimClipIndex = 0;
        _agent.isStopped = true;
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));

        while (!IsAnimationEnded(animationClips[curAnimClipIndex]))
        {
            if (GetCurAnimationNormTime(animationClips[curAnimClipIndex]) <= rotateTime)_boss.RotateWhenAgentStopped();
            yield return null;
        }
        
        patternEnableTime = Time.time + patternCooldown;
        _boss.EndAction(nextState);
    }
}
