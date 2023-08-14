using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using UnityEngine.Serialization;

public enum BossPatternType
{
    Nm,
    Sp,
    None
}
public class Boss : LivingEntity
{
    [SerializeField] private BossScriptableObject bossScriptableObject;
    [SerializeField] private Transform patternContainer;
    [SerializeField] private BossPatternData[] patternData;
    // [SerializeField] private BossPatternData[] flyPatternData;
    public enum BossState
    {
        Idle,
        Patrol,
        Trace,
        TakeOff,
        Land,
        Action,
        GetHit,
        Groggy,
        Scream,
        Die,
    }
    private BossState curState;
    private BossState nextState;
    private bool isStateChanged = true;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Rigidbody _rigidBody;
    private Rig _animRig;
    private CinemachineVirtualCamera _followVcam;

    #region 애니메이션 메서드
    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    private void AnimCrossFade(int stateNameHash) => _animator.CrossFade(stateNameHash, animTransDuration);
    private float GetCurAnimationNormTime(int stateNameHash)
    {
        if (!GetCurStateInfo(0).shortNameHash.Equals(stateNameHash)) return -1f;
        return GetCurStateInfo(0).normalizedTime;
    }
    private bool IsAnimationEnded(int stateNameHash) => GetCurAnimationNormTime(stateNameHash) >= 1f;
    #endregion

    private float turnSmoothVelocity;

    private Coroutine curPatternRoutine;

    public bool Grounded { get; private set; }

    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;
    public LayerMask whatIsTarget;

    public bool Fly;

    // private float smoothFlyVelocity;

    private bool isDead;
    
    // 보스 움직임
    
    private Coroutine updatePath;
    private Transform targetTransform;
    private float patrolRadius = 10f;
    private float findTargetRadius;
    private float patrolSpeed;
    private float traceSpeed;
    private float flyPatrolSpeed;
    private float flyTraceSpeed;
    private float takeOffSpeed;
    private float restTime;
    
    public float TakeOffSpeed => takeOffSpeed;

    private Vector3 agentPosition
    {
        get
        {
            Vector3 temp = transform.position;
            temp.y -= _agent.baseOffset;
            return temp;
        }
    }

    private readonly float animTransDuration = .1f;
    
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

    // private string _animTagIdle;
    // private string _animTagTakeOff;
    // private string _animTagFly;
    // private string _animTagLand;
    // private string _animTagGetHit;
    // private string _animTagGroggy;
    // private string _animTagScream;
    // private string _animTagDie;
    #endregion
    public bool ActionEnded { get; private set; }
    public BossState NextStateAfterAction { get; private set; }
    public float MaxHp => maxHp;
    public float CurHp => currentHp;


    private float[] phaseCheckPoint;
    private int curPhase;
    private BossPatternType nextActionType = BossPatternType.Nm;
    private bool getHitTrigger;
    private bool fightStarted = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _animRig = GetComponentInChildren<Rig>();
        _followVcam = GetComponentInChildren<CinemachineVirtualCamera>();
        
        
        // 패턴 데이터 초기화
        foreach (var data in patternData)
        {
            data.InitPatternData(patternContainer, this);;
        }

        // foreach (var data in flyPatternData)
        // {
        //     data.InitPatternData(patternContainer, this);
        // }

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

        Fly = true;
        curState = BossState.Idle;
        nextState = BossState.Idle;
        _agent.baseOffset = 3.6f;
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

        // _animTagIdle = "Idle";
        // _animTagTakeOff = "TakeOff";
        // _animTagFly = "Fly";
        // _animTagLand = "Land";
        // _animTagGetHit = "GetHit";
        // _animTagGroggy = "Groggy";
        // _animTagScream = "Scream";
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
                _agent.isStopped = true;
                if (!Fly) _agent.baseOffset = 0f;
                int idIdle = Fly ? _animIdFly : _animIdIdle;
                AnimCrossFade(idIdle);
                break;
            case BossState.Patrol:
                _animRig.weight = 0f;
                targetTransform = null;
                _agent.isStopped = false;
                _agent.speed = patrolSpeed;
                int idPatrol = Fly ? _animIdFlyForward : _animIdWalk;
                AnimCrossFade(idPatrol);
                _agent.SetDestination(MyUtility.GetRandomPointOnNavmesh(agentPosition, patrolRadius));
                break;
            case BossState.Trace:
                _animRig.weight = 1f;
                _agent.isStopped = false;
                _agent.speed = traceSpeed;
                int idTrace = Fly ? _animIdFlyForward : _animIdWalk;
                AnimCrossFade(idTrace);
                _agent.SetDestination(targetTransform.position);
                break;
            case BossState.TakeOff:
                _animRig.weight = 0f;
                _agent.isStopped = true;
                Fly = true;
                AnimCrossFade(_animIdTakeOff);
                break;
            case BossState.Land:
                _animRig.weight = 0f;
                _agent.isStopped = true;
                Fly = false;
                AnimCrossFade(_animIdLand);
                break;
            case BossState.GetHit:
                _animRig.weight = 0f;
                _agent.isStopped = true;
                if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                AnimCrossFade(_animIdGetHit);
                break;
            case BossState.Groggy:
                _animRig.weight = 0f;
                AnimCrossFade(_animIdGroggy);
                break;
            case BossState.Scream:
                _animRig.weight = 0f;
                int idScream = Fly ? _animIdFlyScream : _animIdScream;
                AnimCrossFade(idScream);
                ToggleFocusFollowVcam();
                break;
            case BossState.Die:
                _animRig.weight = 0f;
                _agent.isStopped = true;
                if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
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
            // case BossState.Fly:
            // case BossState.FlyPatrol:
            // case BossState.FlyTrace:
            case BossState.Action:
            case BossState.GetHit:
            case BossState.Groggy:
            case BossState.Scream:
            case BossState.Die:
                break;
            case BossState.TakeOff:
                if (GetCurAnimationNormTime(_animIdTakeOff) >= 0f) _agent.baseOffset += takeOffSpeed * Time.deltaTime;
                // if (GetCurStateInfo(0).IsTag(_animTagTakeOff)) _agent.baseOffset += takeOffSpeed * Time.deltaTime;
                break;
            case BossState.Land:
                if (GetCurAnimationNormTime(_animIdLand) >= 0f) _agent.baseOffset -= takeOffSpeed * Time.deltaTime;
                // if (GetCurStateInfo(0).IsTag(_animTagLand)) _agent.baseOffset -= takeOffSpeed * Time.deltaTime;
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
                // Idle 애니메이션중 이륙 시작
                if (GetCurAnimationNormTime(_animIdIdle) >= 0f && Fly)
                {
                    nextState = BossState.TakeOff;
                    return true;
                }
                // Fly 애니메이션중 착지 시작
                if (GetCurAnimationNormTime(_animIdFly) >= 0f && !Fly)
                {
                    nextState = BossState.Land;
                    return true;
                }

                if (GetCurAnimationNormTime(_animIdIdle) >= restTime ||
                    GetCurAnimationNormTime(_animIdFly) >= restTime)
                {
                    var colliders = Physics.OverlapSphere(agentPosition, findTargetRadius, whatIsTarget);
                    foreach (var coll in colliders)
                    {
                        var livingEntity = coll.GetComponent<LivingEntity>();

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
                        if (!fightStarted)
                        {
                            nextState = BossState.Scream;
                            return true;
                        }
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
                if (HasReachedDestination())
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

                if (HasReachedDestination()) Rotate2Target(2f);
                // 우선 순위, 시야 범위 고려하여 패턴 select
                var selectedAction = patternData[(int)nextActionType].SelectPatternAction();
                if (selectedAction is not null)
                {
                    selectedAction.targetTransform = targetTransform;
                    if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                    curPatternRoutine = StartCoroutine(selectedAction.PatternRoutine());
                    nextState = BossState.Action;
                    return true;
                }

                break;
            case BossState.TakeOff:
                if (GetCurAnimationNormTime(_animIdTakeOff) >= 1f)
                {
                    nextState = BossState.Idle;
                    return true;
                }
                break;
            case BossState.Land:
                if (GetCurAnimationNormTime(_animIdLand) >= 1f
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
                    nextState = NextStateAfterAction;
                    // nextState = BossState.Idle;
                    // if (Fly && nextActionType.Equals(BossPatternType.Nm))
                    // {
                    //     nextState = BossState.Land;
                    //     return true;
                    // }
                    nextActionType = BossPatternType.Nm;
                    return true;
                }

                break;
            case BossState.GetHit:
                if (GetCurAnimationNormTime(_animIdGetHit) >= 1f)
                {
                    nextState = BossState.Groggy;
                    return true;
                }

                break;
            case BossState.Groggy:
                if (GetCurAnimationNormTime(_animIdGroggy) >= 5f)
                {
                    nextState = BossState.Idle;
                    nextActionType = BossPatternType.Sp;
                    return true;
                }
                break;
            case BossState.Scream:
                if ((GetCurAnimationNormTime(_animIdScream) >= 1f && !Fly) ||
                    (GetCurAnimationNormTime(_animIdFlyScream) >= 1f && Fly))
                {
                    nextState = BossState.Idle;
                    if (!fightStarted)
                    {
                        fightStarted = true;
                        nextState = BossState.Land;
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
    public void RotateWhenAgentStopped()
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

    public void Rotate2Target(float spMultiplier)
    {
        if (targetTransform is not null)
        {
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation,
                    Time.deltaTime * _agent.angularSpeed * spMultiplier);
            print("rotate 2 target called");
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

    public void EndAction(BossState nextStateAfterAction)
    {
        ActionEnded = true;
        NextStateAfterAction = nextStateAfterAction;
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
                    break;
                // target transform 업데이트해야하는 상태 (추적 갱신)
                case BossState.Trace:
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
