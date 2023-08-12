using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class GlideAttack : BossPatternAction
{
    private float glideSpeed = 100f;
    public override IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }
        Debug.Log("낙하 패턴 시작");
        
        // 포효 1번
        _agent.isStopped = true;
        AnimCrossFade(ClipName2Hash(animationClips[0]));
        yield return new WaitUntil(() => IsAnimationEnded(animationClips[0]));
        // 이륙
        AnimCrossFade(ClipName2Hash(animationClips[1]));
        while (!IsAnimationEnded(animationClips[1]))
        {
            if(GetCurStateInfo(0).IsName(animationClips[1].name)) _agent.baseOffset = Mathf.Lerp(0f, 10f, GetCurStateInfo(0).normalizedTime);
            yield return null;
        }
        // 타겟 주시 3초
        AnimCrossFade(ClipName2Hash(animationClips[2]));
        float timer = 3f;
        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            _boss.Rotate();
            yield return null;
        }
        // 낙하 시작
        AnimCrossFade(ClipName2Hash(animationClips[3]));
        float height = _agent.baseOffset;
        print("height is : " + height);
        _agent.isStopped = false;
        _agent.SetDestination(targetTransform.position);
        if (!_agent.hasPath)
        {
            Debug.LogError("낙하 경로 없음");
            patternEnableTime = Time.time + patternCooldown;
            _boss.EndAction();
            yield break;
        }
        float pathLength = GetPathLength(_agent.path);
        _agent.speed = glideSpeed;
        while (!_boss.Grounded)
        {
            float glideRatio = GetPathLength(_agent.path) / pathLength;
            print("glide progress : " + glideRatio * 100f + "%");
            _agent.baseOffset = height * (GetPathLength(_agent.path) / pathLength);
            yield return null;
        }
        // while (glideTimer < glideTime)
        // {
        //     glideTimer += Time.deltaTime;
        //     if (GetCurStateInfo(0).IsName(animationClips[3].name))
        //         _agent.baseOffset = Mathf.Lerp(10f, 0f, glideTimer / glideTime);
        //     yield return null;
        // }
        _agent.baseOffset = 0f;
        AnimCrossFade(ClipName2Hash(animationClips[4]));
        yield return new WaitUntil(() => IsAnimationEnded(animationClips[4]));
        
        print("낙하 패턴 종료");
        patternEnableTime = Time.time + patternCooldown;
        _boss.EndAction();
    }

}
