using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RunAttack : BossPatternAction
{
    private float runDistance = 20f;
    private float runSpeed = 20f;
    public override IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }

        Debug.Log(gameObject.name + " : 일반 패턴 시작");
        curAnimClipIndex = 0;
        _agent.isStopped = false;
        _agent.speed = runSpeed;
        _agent.SetDestination(GetForwardDestination());
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));
        while (!_boss.HasReachedDestination())
        {
            yield return null;
        }

        patternEnableTime = Time.time + patternCooldown;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;
        _boss.EndAction(nextState);
    }

    private Vector3 GetForwardDestination()
    {
        Vector3 direction = _boss.transform.forward;
        direction.y = 0f;
        direction.Normalize();
        NavMeshHit hit;
        Vector3 runDestination = _boss.transform.position + direction * runDistance;
        if (NavMesh.SamplePosition(runDestination, out hit, runDistance, NavMesh.AllAreas))
            runDestination = hit.position;
        else runDestination = _boss.transform.position;
        
        return runDestination;
    }
}
