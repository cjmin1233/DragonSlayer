using System.Collections;
using UnityEngine;

public class FlyFlameAttackDWN : BossPatternAction
{
    public override IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }
        Debug.Log("불바다 패턴 시작");
        
        // 포효 1번
        _agent.isStopped = true;
        AnimCrossFade(ClipName2Hash(animationClips[0]));
        yield return new WaitUntil(() => IsAnimationEnded(animationClips[0]));
        // 이륙
        _boss.Fly = true;
        AnimCrossFade(ClipName2Hash(animationClips[1]));
        while (!IsAnimationEnded(animationClips[1]))
        {
            if(GetCurStateInfo(0).IsName(animationClips[1].name)) _agent.baseOffset = Mathf.Lerp(0f, 5f, GetCurStateInfo(0).normalizedTime);

            yield return null;
        }
        // 쉘터 활성화
        BossActionManager.Instance.EnableShelters();
        // 타겟 주시 3초
        AnimCrossFade(ClipName2Hash(animationClips[2]));
        float timer = 3f;
        while (timer >= 0f)
        {
            timer -= Time.deltaTime;
            _boss.Rotate();
            yield return null;
        }
        
        // 화염 방사
        AnimCrossFade(ClipName2Hash(animationClips[3]));
        BossActionManager.Instance.EnableHellFireField();
        
        yield return new WaitUntil(() => IsAnimationEnded(animationClips[3]));
        print("불바다 패턴 종료");
        patternEnableTime = Time.time + patternCooldown;
        _boss.EndAction();
    }
}
