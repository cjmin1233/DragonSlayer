using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BackStep : BossPatternAction
{
    private float backStepDistance = 10f;
    private Vector3 backStepSmoothVelocity;
    private float backStepSmoothTime = .2f;
    private float backStepStartTime = .5f;
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
        Vector3 backStepDirection = _boss.transform.position - targetTransform.position;
        backStepDirection.y = 0f;
        backStepDirection.Normalize();
        Vector3 backStepPosition = _boss.transform.position + backStepDirection * backStepDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(backStepPosition, out hit, backStepDistance, NavMesh.AllAreas))
        {
            backStepPosition = hit.position;
        }
        else backStepPosition = _boss.transform.position;
        while (!IsAnimationEnded(animationClips[curAnimClipIndex]))
        {
            _boss.RotateWhenAgentStopped();
            
            if (GetCurAnimationNormTime(animationClips[curAnimClipIndex]) >= backStepStartTime)
            {
                _boss.transform.position = Vector3.SmoothDamp(_boss.transform.position, backStepPosition,
                    ref backStepSmoothVelocity, backStepSmoothTime);
            }
            yield return null;
        }
        patternEnableTime = Time.time + patternCooldown;
        _boss.EndAction(nextState);
    }
}