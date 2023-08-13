using System.Collections;
using UnityEngine;

public class HellFireFWD : BossPatternAction
{
    public override IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }

        Debug.Log(gameObject.name + ": 불바다 패턴 시작");
        curAnimClipIndex = 0;
        // 포효 1번
        _agent.isStopped = true;
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));
        yield return new WaitUntil(() => IsAnimationEnded(animationClips[curAnimClipIndex]));
        // 이륙
        _boss.Fly = true;
        _agent.isStopped = false;
        _agent.speed = 5f;
        _agent.SetDestination(Vector3.zero);
        curAnimClipIndex++;
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));
        while (!IsAnimationEnded(animationClips[curAnimClipIndex]))
        {
            _agent.baseOffset += _boss.TakeOffSpeed * Time.deltaTime;
            yield return null;
        }

        curAnimClipIndex++;
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));
        yield return new WaitUntil(() => _boss.HasReachedDestination());
        // 타겟 주시 3초
        _agent.isStopped = true;
        curAnimClipIndex++;
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));
        float timer = 3f;
        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            _boss.Rotate();
            yield return null;
        }
        
        // 화염 방사
        curAnimClipIndex++;
        AnimCrossFade(ClipName2Hash(animationClips[curAnimClipIndex]));
        float lastAnimNormTime = 0f;
        while (!IsAnimationEnded(animationClips[curAnimClipIndex]))
        {
            _boss.Rotate();
            if (lastAnimNormTime < .5f && GetCurAnimationNormTime(animationClips[curAnimClipIndex]) >= .5f)
                BossActionManager.Instance.EnableHellFireField();
            lastAnimNormTime = GetCurAnimationNormTime(animationClips[curAnimClipIndex]);
            yield return null;
        }
        print("불바다 패턴 종료");
        patternEnableTime = Time.time + patternCooldown;
        _boss.EndAction();
    }
}