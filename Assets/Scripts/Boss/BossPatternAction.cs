using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class BossPatternAction : MonoBehaviour
{
    // public AnimatorOverrideController[] animatorOv;
    public AnimationClip[] animationClips;
    public int priority;
    public float patternCooldown;
    public float fieldOfView;
    public float viewDistance;
    public float patternEnableTime;
    public Transform targetTransform;

    protected AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    protected void AnimCrossFade(int stateHashName) =>
        _animator.CrossFade(stateHashName, animTransDuration);
    protected int ClipName2Hash(AnimationClip clip) => Animator.StringToHash(clip.name);

    protected bool IsAnimationEnded(AnimationClip clip) => GetCurStateInfo(0).IsName(clip.name)
                                                           && GetCurStateInfo(0).normalizedTime >= 1f;
    
    protected Animator _animator;
    protected Boss _boss;
    protected NavMeshAgent _agent;

    protected int _animIdAction;
    protected int _animIdActionChange;
    protected string _animTagAction;

    private float animTransDuration = .1f;
    public void Init(AnimationClip[] animationClipsParam, int priorityParam, float patternCooldownParam,
        float fovParam, float viewDistanceParam)
    {
        this.animationClips = animationClipsParam;
        // this.animatorOv = animatorOvParam;
        this.priority = priorityParam;
        this.patternCooldown = patternCooldownParam;
        this.fieldOfView = fovParam;
        this.viewDistance = viewDistanceParam;

        _animator = GetComponentInParent<Animator>();
        _boss = GetComponentInParent<Boss>();
        _agent = GetComponentInParent<NavMeshAgent>();
        
        // assign animation id
        _animIdAction = Animator.StringToHash("Action");
        _animIdActionChange = Animator.StringToHash("ActionChange");
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
        // _animator.runtimeAnimatorController = animatorOv[0];
        
        _agent.isStopped = true;
        AnimCrossFade(_animIdAction);
        
        yield return new WaitUntil(() => GetCurStateInfo(0).IsTag(_animTagAction)
                                         && GetCurStateInfo(0).normalizedTime >= 1f);
        patternEnableTime = Time.time + patternCooldown;
        
        _boss.EndAction();
    }

    protected void Rotate()
    {
        if (_agent.isStopped && targetTransform is not null)
        {
            // 목적지와의 방향을 계산하여 에이전트를 회전시킵니다.
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * _agent.angularSpeed);
        }
    }
    protected float GetPathLength(NavMeshPath path)
    {
        float length = 0f;

        if (path.corners.Length < 2)
            return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return length;
    }
}
