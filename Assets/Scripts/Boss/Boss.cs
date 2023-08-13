using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Cinemachine;

public enum BossPatternType
{
    Nm,
    Sp,
    None
}
public class Boss : LivingEntity
{
    [SerializeField] private BossScriptableObject bossScriptableObject;
    private enum BossState
    {
        Idle,
        Patrol,
        Trace,
        TakeOff,
        Fly,
        FlyPatrol,
        FlyTrace,
        Land,
        Action,
        GetHit,
        Groggy,
        Scream,
        Die,
    }
    private BossState curState = BossState.Fly;
    private BossState nextState = BossState.Fly;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Rigidbody _rigidBody;
    private Rig _animRig;

    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    private void AnimCrossFade(int stateHashName) => _animator.CrossFade(stateHashName, animTransDuration);

    private float turnSmoothVelocity;
    private bool isStateChanged = true;

    [SerializeField] private Transform patternContainer;
    [SerializeField] private BossPatternData[] patternData;
    [SerializeField] private BossPatternData[] flyPatternData;
    private Coroutine curPatternRoutine;

    // private float maxHealth;
    public bool Grounded { get; private set; }

    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    public bool Fly = true;

    // [SerializeField] private float minFlyOffset;
    // [SerializeField] private float maxFlyOffset;
    private float smoothFlyVelocity;
    // private float targetFlyOffset;

    private bool isDead;
    
    // 보스 움직임
    public LayerMask whatIsTarget;
    
    private Coroutine updatePath;
    private Transform targetTransform;
    [SerializeField, Range(1f, 10f)] private float patrolRadius;
    private float findTargetRadius;
    private float patrolSpeed;
    private float traceSpeed;
    private float flyPatrolSpeed;
    private float flyTraceSpeed;
    private float takeOffSpeed;
    
    public float TakeOffSpeed => takeOffSpeed;

    private float restTime;
    private Vector3 agentPosition
    {
        get
        {
            Vector3 temp = transform.position;
            temp.y -= _agent.baseOffset;
            return temp;
        }
    }

    private float animTransDuration = .1f;
    
    #region Animator string ID
    private int _animIdIdle;
    private int _animIdWalk;
    private int _animIdTakeOff;
    private int _animIdFly;
    private int _animIdFlyForward;
    private int _animIdLand;
    private int _animIdGetHit;
    private int _animIdGroggy;
    private int _animIdDie;
    private int _animIdAction;
    private int _animIdScream;
    private int _animIdFlyScream;

    private string _animTagIdle;
    private string _animTagTakeOff;
    private string _animTagFly;
    private string _animTagLand;
    private string _animTagGetHit;
    private string _animTagGroggy;
    private string _animTagScream;
    // private string _animTagDie;
    #endregion

    public bool ActionEnded { get; private set; }
    
    public float MaxHP => maxHp;

    public float CurHP => currentHp;

    public float[] phaseCheckPoint;
    public int curPhase;
    public BossPatternType nextActionType = BossPatternType.Nm;
    public bool getHitTrigger;

    public bool fightStarted = false;

    [SerializeField] private CinemachineVirtualCamera _followVcam;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _animRig = GetComponentInChildren<Rig>();
        
        // 패턴 데이터 초기화
        foreach (var data in patternData)
        {
            data.InitPatternData(patternContainer, this);;
        }

        foreach (var data in flyPatternData)
        {
            data.InitPatternData(patternContainer, this);
        }

        // 보스 데이터 초기화
        InitBossData();
        // 애니메이션 관련 변수 초기화
        InitAnimatorParams();
        updatePath = StartCoroutine(UpdatePath());
    }

    private void InitBossData()
    {
        if (bossScriptableObject is null)
        {
            Debug.LogError("Boss data is missing");
            return;
        }
        isDead = false;
        Fly = true;
        _agent.baseOffset = 3.6f;
        this.maxHp = bossScriptableObject.maxHp;
        this.currentHp = this.maxHp;
        this.findTargetRadius = bossScriptableObject.findTargetRadius;
        this.patrolSpeed = bossScriptableObject.patrolSpeed;
        this.traceSpeed = bossScriptableObject.traceSpeed;
        this.flyPatrolSpeed = bossScriptableObject.flyPatrolSpeed;
        this.flyTraceSpeed = bossScriptableObject.flyTraceSpeed;
        this.findTargetRadius = bossScriptableObject.findTargetRadius;
        this.takeOffSpeed = bossScriptableObject.takeOffSpeed;
        this.restTime = bossScriptableObject.restTime;
        if (bossScriptableObject.phase > 1) this.phaseCheckPoint = new float[bossScriptableObject.phase];
        else this.phaseCheckPoint = new float[1];
        for (int i = 0; i < phaseCheckPoint.Length-1; i++)
        {
            phaseCheckPoint[i] = maxHp * (1f - (float)(i + 1) / phaseCheckPoint.Length);
        }

        phaseCheckPoint[phaseCheckPoint.Length - 1] = 0f;
        curPhase = 0;
    }
    private void InitAnimatorParams()
    {
        _animIdIdle = Animator.StringToHash("Idle");
        _animIdWalk = Animator.StringToHash("Walk");
        _animIdTakeOff = Animator.StringToHash("TakeOff");
        _animIdFly = Animator.StringToHash("Fly");
        _animIdFlyForward = Animator.StringToHash("FlyForward");
        _animIdLand = Animator.StringToHash("Land");
        _animIdGetHit = Animator.StringToHash("GetHit");
        _animIdGroggy = Animator.StringToHash("Groggy");
        _animIdScream = Animator.StringToHash("Scream");
        _animIdFlyScream = Animator.StringToHash("FlyScream");
        _animIdDie = Animator.StringToHash("Die");

        _animTagIdle = "Idle";
        _animTagTakeOff = "TakeOff";
        _animTagFly = "Fly";
        _animTagLand = "Land";
        _animTagGetHit = "GetHit";
        _animTagGroggy = "Groggy";
        _animTagScream = "Scream";
        // _animTagDie = "Die";
    }

    private void Update()
    {
        GroundedCheck();

        curState = nextState;

        if (isStateChanged) StateEnter();
        isStateChanged = false;
        
        StateUpdate();

        isStateChanged = TransitionCheck();

        if (isStateChanged) StateExit();
    }

    private void StateEnter()
    {
        switch (curState)
        {
            case BossState.Action:
                break;
            case BossState.Idle:
                _animRig.weight = targetTransform is not null ? 1f : 0f;
                targetTransform = null;
                _agent.isStopped = true;
                AnimCrossFade(_animIdIdle);
                break;
            case BossState.Patrol:
                _animRig.weight = 0f;
                _agent.isStopped = false;
                _agent.speed = patrolSpeed;
                AnimCrossFade(_animIdWalk);
                
                _agent.SetDestination(MyUtility.GetRandomPointOnNavmesh(agentPosition, patrolRadius));
                break;
            case BossState.Trace:
                _animRig.weight = 1f;
                _agent.isStopped = false;
                _agent.speed = traceSpeed;
                AnimCrossFade(_animIdWalk);
                
                _agent.SetDestination(targetTransform.position);
                break;
            case BossState.TakeOff:
                _animRig.weight = 0f;
                _agent.isStopped = true;
                AnimCrossFade(_animIdTakeOff);
                break;
            case BossState.Fly:
                _animRig.weight = targetTransform is not null ? 1f : 0f;
                _agent.isStopped = true;
                AnimCrossFade(_animIdFly);
                break;
            case BossState.FlyPatrol:
                _animRig.weight = 0f;
                _agent.isStopped = false;
                _agent.speed = flyPatrolSpeed;
                AnimCrossFade(_animIdFlyForward);
                
                _agent.SetDestination(MyUtility.GetRandomPointOnNavmesh(agentPosition, patrolRadius));
                break;
            case BossState.FlyTrace:
                _animRig.weight = 1f;
                _agent.isStopped = false;
                _agent.speed = flyTraceSpeed;
                AnimCrossFade(_animIdFlyForward);

                _agent.SetDestination(targetTransform.position);
                break;
            case BossState.Land:
                _animRig.weight = 0f;
                _agent.isStopped = true;
                AnimCrossFade(_animIdLand);
                break;
            case BossState.GetHit:
                _animRig.weight = 0f;
                AnimCrossFade(_animIdGetHit);
                break;
            case BossState.Groggy:
                _animRig.weight = 0f;
                AnimCrossFade(_animIdGroggy);
                break;
            case BossState.Scream:
                _animRig.weight = 0f;
                if (!Fly) AnimCrossFade(_animIdScream);
                else AnimCrossFade(_animIdFlyScream);
                ToggleFocusFollowVcam();
                break;
            case BossState.Die:
                _animRig.weight = 0f;
                AnimCrossFade(_animIdDie);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void StateUpdate()
    {
        switch (curState)
        {
            case BossState.Idle:
            case BossState.Patrol:
            case BossState.Trace:
            case BossState.Fly:
            case BossState.FlyPatrol:
            case BossState.FlyTrace:
            case BossState.Action:
            case BossState.GetHit:
            case BossState.Groggy:
            case BossState.Scream:
            case BossState.Die:
                break;
            case BossState.TakeOff:
                if (GetCurStateInfo(0).IsTag(_animTagTakeOff))
                {
                    _agent.baseOffset += takeOffSpeed * Time.deltaTime;
                    // _agent.baseOffset =
                    //     Mathf.Lerp(0f, targetFlyOffset, GetCurStateInfo(0).normalizedTime);
                }
                break;
            case BossState.Land:
                if (GetCurStateInfo(0).IsTag(_animTagLand))
                {
                    _agent.baseOffset -= takeOffSpeed * Time.deltaTime;
                    // _agent.baseOffset =
                    //     Mathf.Lerp(targetFlyOffset, 0f, GetCurStateInfo(0).normalizedTime);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private bool TransitionCheck()
    {
        // 보스 사망
        if (isDead && !curState.Equals(BossState.Die))
        {
            print("Boss dead");
            nextState = BossState.Die;
            return true;
        }
        if (getHitTrigger && !Fly)
        {
            getHitTrigger = false;
            nextState = BossState.GetHit;
            return true;
        }
        
        switch (curState)
        {
            case BossState.Idle:
                if (Fly)
                {
                    nextState = BossState.TakeOff;
                    return true;
                }

                if (GetCurStateInfo(0).IsTag(_animTagIdle)
                    && GetCurStateInfo(0).normalizedTime >= restTime)
                {
                    // 범위 내 타겟 있는지 확인
                    var colliders = Physics.OverlapSphere(agentPosition, findTargetRadius, whatIsTarget);
                    
                    foreach (var collider in colliders)
                    {
                        var livingEntity = collider.GetComponent<LivingEntity>();

                        if (livingEntity is not null)
                        {
                            targetTransform = livingEntity.transform;
                            break;
                        }
                    }
                    print("잠깐 쉬고 추적 대상 탐색");

                    if (targetTransform is not null)
                    {
                        // 범위 내 타겟 확인
                        print("타겟 확인. trace 시작");
                        nextState = BossState.Trace;
                        return true;
                    }
                    else
                    {
                        print("타겟 없음. patrol 시작");
                        nextState = BossState.Patrol;
                        return true;
                    }
                }
                break;
            case BossState.Patrol:
                if (Vector3.Distance(agentPosition, _agent.destination) <= _agent.stoppingDistance)
                {
                    // patrol 중 destination 도착
                    print("patrol destination 도착. Idle 상태 돌입");
                    
                    nextState = BossState.Idle;
                    return true;
                }
                break;
            case BossState.Trace:
                // trace 중 타겟이 사라지면 idle 상태 돌입
                if (targetTransform is null)
                {
                    nextState = BossState.Idle;
                    return true;
                }

                // 우선 순위, 시야 범위 고려하여 패턴 select
                var patternAction = patternData[(int)nextActionType].SelectPatternAction();
                if (patternAction is not null)
                {
                    patternAction.targetTransform = targetTransform;
                    if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                    curPatternRoutine = StartCoroutine(patternAction.PatternRoutine());
                    nextState = BossState.Action;
                    return true;
                }

                break;
            case BossState.TakeOff:
                if (GetCurStateInfo(0).IsTag(_animTagTakeOff)
                    && GetCurStateInfo(0).normalizedTime >= 1f)
                {
                    nextState = BossState.Fly;
                    return true;
                }
                break;
            case BossState.Fly:
                if (!Fly)
                {
                    // targetFlyOffset = _agent.baseOffset;
                    nextState = BossState.Land;
                    return true;
                }
                if (GetCurStateInfo(0).IsTag(_animTagFly)
                    && GetCurStateInfo(0).normalizedTime >= restTime)
                {
                    // 범위 내 타겟 있는지 확인
                    print("잠깐 쉬고 추적 대상 탐색");
                    var colliders = Physics.OverlapSphere(agentPosition, findTargetRadius, whatIsTarget);
                    
                    foreach (var col in colliders)
                    {
                        var livingEntity = col.GetComponent<LivingEntity>();

                        if (livingEntity is not null)
                        {
                            targetTransform = livingEntity.transform;
                            break;
                        }
                    }

                    if (targetTransform is not null)
                    {
                        // 범위 내 타겟 확인
                        print("타겟 확인. trace 시작");
                        if (!fightStarted)
                        {
                            nextState = BossState.Scream;
                            return true;
                        }
                        nextState = BossState.FlyTrace;
                        return true;
                    }
                    else
                    {
                        print("타겟 없음. fly patrol 시작");
                        nextState = BossState.FlyPatrol;
                        return true;
                    }
                }
                break;
            case BossState.FlyPatrol:
                if (Vector3.Distance(agentPosition, _agent.destination) <= _agent.stoppingDistance)
                {
                    // fly patrol 중 destination 도착
                    print("fly patrol destination 도착. Fly 상태 돌입");
                    
                    nextState = BossState.Fly;
                    return true;
                }
                break;
            case BossState.FlyTrace:
                // trace 중 타겟이 사라지면 fly 상태 돌입
                if (targetTransform is null)
                {
                    nextState = BossState.Fly;
                    return true;
                }
                // 우선 순위, 시야 범위 고려하여 패턴 select
                var flyPattern = flyPatternData[(int)nextActionType].SelectPatternAction();
                if (flyPattern is not null)
                {
                    flyPattern.targetTransform = targetTransform;
                    if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                    curPatternRoutine = StartCoroutine(flyPattern.PatternRoutine());
                    nextState = BossState.Action;
                    return true;
                }
                break;
            case BossState.Land:
                if (GetCurStateInfo(0).IsTag(_animTagLand)
                    && Grounded)
                {
                    nextState = BossState.Idle;
                    return true;
                }
                break;
            case BossState.Action:
                if (ActionEnded)
                {
                    // 패턴이 끝나면 idle 상태로
                    ActionEnded = false;
                    nextState = Fly ? BossState.Fly : BossState.Idle;
                    nextActionType = BossPatternType.Nm;
                    return true;
                }

                break;
            case BossState.GetHit:
                if (GetCurStateInfo(0).IsTag(_animTagGetHit)
                    && GetCurStateInfo(0).normalizedTime >= 1f)
                {
                    nextState = BossState.Groggy;
                    return true;
                }

                break;
            case BossState.Groggy:
                if (GetCurStateInfo(0).IsTag(_animTagGroggy)
                    && GetCurStateInfo(0).normalizedTime >= 5f)
                {
                    nextState = BossState.Idle;
                    nextActionType = BossPatternType.Sp;
                    return true;
                }

                break;
            case BossState.Scream:
                if (GetCurStateInfo(0).IsTag(_animTagScream)
                    && GetCurStateInfo(0).normalizedTime >= 1f)
                {
                    nextState = Fly ? BossState.Fly : BossState.Idle;
                    if (!fightStarted)
                    {
                        fightStarted = true;
                        Fly = false;
                    }
                    nextActionType = BossPatternType.Nm;
                    return true;
                }

                break;
            case BossState.Die:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }
    private void StateExit()
    {
        switch (curState)
        {
            // loop animation clip
            case BossState.Idle:
            case BossState.Patrol:
            case BossState.Trace:
            case BossState.Fly:
            case BossState.FlyPatrol:
            case BossState.FlyTrace:
            case BossState.Action:
            case BossState.Groggy:
                break;
            
            // exit time == 1f animation clip
            case BossState.TakeOff:
            case BossState.Land:
            case BossState.GetHit:
            case BossState.Die:
                break;
            case BossState.Scream:
                ToggleFocusFollowVcam();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = transform.position;
        spherePosition.y -= groundedOffset;
        
        Grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);
    }
    public void Rotate()
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
    public bool IsTargetOnSight(float fov, float viewDist)
    {
        if (targetTransform is null) return false;

        var direction = targetTransform.position - agentPosition;
        direction.y = 0f;

        if (Vector3.Angle(direction, transform.forward) > fov * .5f) return false;
        if (Physics.Raycast(agentPosition, direction, out RaycastHit hit, viewDist, whatIsTarget))
        {
            if (hit.transform.Equals(targetTransform)) return true;
        }
        return false;
    }
    // private bool IsTargetOnSight(Transform target)
    // {
    //     var direction = target.position - agentPosition;
    //     direction.y = 0f;
    //
    //     if (Vector3.Angle(direction, transform.forward) > fieldOfView * 0.5f)
    //     {
    //         return false;
    //     }
    //
    //     if (Physics.Raycast(agentPosition, direction, out RaycastHit hit, viewDistance, whatIsTarget))
    //     {
    //         if (hit.transform == target) return true;
    //     }
    //     
    //     return false;
    // }

    // private float TargetAngle(Transform target)
    // {
    //     var direction = target.position - agentPosition;
    //     direction.y = 0f;
    //     return Vector3.Angle(direction, transform.forward);
    // }
    // private IEnumerator BossThink()
    // {
    //     while (true)
    //     {
    //         // if (!isFlying)
    //         // {
    //         //     if (groundState.Equals(GroundState.Patrol))
    //         //     {
    //         //         // 패턴 선택
    //         //         var selectedPattern = patternData[0].SelectPatternAction();
    //         //         if (selectedPattern is not null)
    //         //         {
    //         //             groundState = GroundState.Attack;
    //         //             if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
    //         //             curPatternRoutine = StartCoroutine(selectedPattern.PatternRoutine());
    //         //             print("패턴 발동");
    //         //         }
    //         //     }
    //         //     else if (groundState.Equals(GroundState.Attack))
    //         //     {
    //         //         // 공격 중...
    //         //     }
    //         // }
    //
    //         yield return new WaitForSeconds(.1f);
    //     }
    // }

    public void EndAction()
    {
        ActionEnded = true;
        // _animator.SetBool("Exit", false);
    }
    public override void TakeDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject) return;

        currentHp = Mathf.Clamp(currentHp - damageMessage.damage, 0f, maxHp);
        // 마지막 페이즈 제외, 1칸 체력 소진시 피격 >> 그로기 애니메이션
        if (curPhase + 1 < phaseCheckPoint.Length && phaseCheckPoint[curPhase] > currentHp)
        {
            currentHp = phaseCheckPoint[curPhase];
            curPhase++;
            getHitTrigger = true;
        }

        if (currentHp <= 0f) Die();
    }

    // private void Groggy();

    private void Die()
    {
        isDead = true;
        // targetFlyOffset = _agent.baseOffset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Sphere의 색상 설정
        Vector3 spherePosition = transform.position;
        spherePosition.y -= groundedOffset;

        // 현재 오브젝트의 위치에 Sphere를 그림
        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }

    private IEnumerator UpdatePath()
    {
        while (!isDead)
        {
            switch (curState)
            {
                case BossState.Idle:
                case BossState.Patrol:
                case BossState.Fly:
                case BossState.FlyPatrol:
                    break;
                // target transform 업데이트해야하는 상태 (추적 갱신)
                case BossState.Trace:
                case BossState.FlyTrace:
                    if (targetTransform is not null)
                    {
                        _agent.SetDestination(targetTransform.position);
                    }
                    break;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void ToggleFocusFollowVcam()
    {
        _followVcam.Priority += _followVcam.Priority > 10 ? -10 : 10;
    }

    public bool HasReachedDestination() => Vector3.Distance(agentPosition, _agent.destination) <= _agent.stoppingDistance;
}
