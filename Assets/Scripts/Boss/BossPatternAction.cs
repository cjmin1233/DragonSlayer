using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class BossPatternAction : MonoBehaviour
{
    public AnimatorOverrideController[] animatorOv;
    public int priority;
    public float patternCooldown;
    public float patternEnableTime;
    public Transform target;

    protected AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    protected Animator _animator;
    protected Boss _boss;
    public void Init(AnimatorOverrideController[] animatorOvParam, int priorityParam, float patternCooldownParam)
    {
        this.animatorOv = animatorOvParam;
        this.priority = priorityParam;
        this.patternCooldown = patternCooldownParam;

        _animator = GetComponentInParent<Animator>();
        _boss = GetComponentInParent<Boss>();
    }

    public virtual IEnumerator PatternRoutine()
    {
        if (_animator is null)
        {
            Debug.LogError("Animator error");
            yield break;
        }
        Debug.Log("일반 패턴 시작");
        _animator.runtimeAnimatorController = animatorOv[0];
        _animator.Play("Pattern", 0, 0f);

        yield return new WaitUntil(() => _animator.GetBool("Exit"));
        patternEnableTime = Time.time + patternCooldown;
        
        _boss.EndPattern();
    }
}
