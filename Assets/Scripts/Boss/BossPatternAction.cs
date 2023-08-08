using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class BossPatternAction : MonoBehaviour
{
    public AnimatorOverrideController[] animatorOv;
    public int priority;
    public float patternCooldown;
    public float patternEnableTime;
    public Transform targetTransform;

    protected AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    private void AnimCrossFade(int stateHashName) => _animator.CrossFade(stateHashName, animTransDuration);

    protected Animator _animator;
    protected Boss _boss;
    protected NavMeshAgent _agent;

    protected int _animIdAction;
    protected string _animTagAction;

    private float animTransDuration;
    public void Init(AnimatorOverrideController[] animatorOvParam, int priorityParam, float patternCooldownParam)
    {
        this.animatorOv = animatorOvParam;
        this.priority = priorityParam;
        this.patternCooldown = patternCooldownParam;

        _animator = GetComponentInParent<Animator>();
        _boss = GetComponentInParent<Boss>();
        _agent = GetComponentInParent<NavMeshAgent>();
        
        // assign animation id
        _animIdAction = Animator.StringToHash("Action");
        _animTagAction = "Action";
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
        _agent.SetDestination(targetTransform.position);
        _agent.isStopped = true;
        AnimCrossFade(_animIdAction);

        yield return new WaitUntil(() => GetCurStateInfo(0).IsTag(_animTagAction)
                                         && GetCurStateInfo(0).normalizedTime >= 1f);
        patternEnableTime = Time.time + patternCooldown;
        
        _boss.EndAction();
    }
}
