using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum BossPatternType
{
    Nm,
    Sp,
    None
}
public class Boss : LivingEntity
{
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
        Stun,
        GetHit,
        Glide,
        Die,
    }
    private BossState curState = BossState.Idle;
    private BossState nextState = BossState.Idle;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Rigidbody _rigidBody;

    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    private void AnimCrossFade(int stateHashName) => _animator.CrossFade(stateHashName, animTransDuration);

    // [SerializeField] private float turnSmoothTime = .3f;
    private float turnSmoothVelocity;
    private bool isStateChanged = true;
    private Coroutine thinkRoutine;

    [SerializeField] private Transform patternContainer;
    [SerializeField] private BossPatternData[] patternData;
    private Coroutine curPatternRoutine;

    [SerializeField] private float maxHealth;
    public bool Grounded;

    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    public bool Fly;

    [SerializeField] private float minFlyOffset;
    [SerializeField] private float maxFlyOffset;
    // [SerializeField, Range(.1f, 3f)] private float smoothFlyTime; 
    private float smoothFlyVelocity;
    private float targetFlyOffset;

    private bool isDead;
    
    // 보스 움직임
    public LayerMask whatIsTarget;
    
    private Coroutine updatePath;
    private Transform targetTransform;
    [SerializeField, Range(1f, 10f)] private float patrolRadius;
    [SerializeField] private float findTargetRadius;

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
    private int _animIdDie;
    private int _animIdAction;

    private string _animTagIdle;
    private string _animTagTakeOff;
    private string _animTagFly;
    private string _animTagLand;
    private string _animTagDie;
    #endregion

    private float fieldOfView = 50f;
    private float viewDistance = 10f;

    public bool ActionEnded { get; private set; }
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        
        foreach (var data in patternData)
        {
            data.InitPatternData(patternContainer, this);;
        }

        // 애니메이션 관련 변수 초기화
        InitAnimatorParams();
        
        maxHp = maxHealth;
        currentHp = maxHp;
        isDead = false;

        updatePath = StartCoroutine(UpdatePath());
    }
    private void InitAnimatorParams()
    {
        _animIdIdle = Animator.StringToHash("Idle");
        _animIdWalk = Animator.StringToHash("Walk");
        _animIdTakeOff = Animator.StringToHash("TakeOff");
        _animIdFly = Animator.StringToHash("Fly");
        _animIdFlyForward = Animator.StringToHash("FlyForward");
        _animIdLand = Animator.StringToHash("Land");
        _animIdDie = Animator.StringToHash("Die");

        _animTagIdle = "Idle";
        _animTagTakeOff = "TakeOff";
        _animTagFly = "Fly";
        _animTagLand = "Land";
        _animTagDie = "Die";
    }

    private void Update()
    {
        // // 보스 데미지 테스트
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     DamageMessage damageMessage = new DamageMessage(null, 50f, 0f);
        //     TakeDamage(damageMessage);
        // }
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
                targetTransform = null;
                _agent.isStopped = true;
                AnimCrossFade(_animIdIdle);
                break;
            case BossState.Patrol:
                _agent.isStopped = false;
                AnimCrossFade(_animIdWalk);
                
                _agent.SetDestination(MyUtility.GetRandomPointOnNavmesh(agentPosition, patrolRadius));
                break;
            case BossState.Trace:
                _agent.isStopped = false;
                AnimCrossFade(_animIdWalk);
                
                _agent.SetDestination(targetTransform.position);
                break;
            case BossState.TakeOff:
                _agent.isStopped = true;
                AnimCrossFade(_animIdTakeOff);
                break;
            case BossState.Fly:
                _agent.isStopped = true;
                AnimCrossFade(_animIdFly);
                break;
            case BossState.FlyPatrol:
                _agent.isStopped = false;
                AnimCrossFade(_animIdFlyForward);
                
                _agent.SetDestination(MyUtility.GetRandomPointOnNavmesh(agentPosition, patrolRadius));
                break;
            case BossState.FlyTrace:
                _agent.isStopped = false;
                AnimCrossFade(_animIdFlyForward);

                _agent.SetDestination(targetTransform.position);
                break;
            case BossState.Land:
                _agent.isStopped = true;
                AnimCrossFade(_animIdLand);
                break;
            case BossState.Die:
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
                break;
            case BossState.TakeOff:
                if (GetCurStateInfo(0).IsTag(_animTagTakeOff))
                {
                    _agent.baseOffset =
                        Mathf.Lerp(0f, targetFlyOffset, GetCurStateInfo(0).normalizedTime);
                }
                break;
            case BossState.Land:
                if (GetCurStateInfo(0).IsTag(_animTagLand))
                {
                    _agent.baseOffset =
                        Mathf.Lerp(targetFlyOffset, 0f, GetCurStateInfo(0).normalizedTime);
                }
                break;
            case BossState.Die:
                if (GetCurStateInfo(0).IsTag(_animTagDie))
                {
                    _agent.baseOffset =
                        Mathf.Lerp(targetFlyOffset, 0f, GetCurStateInfo(0).normalizedTime);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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
                break;
            
            // exit time == 1f animation clip
            case BossState.TakeOff:
            case BossState.Land:
            case BossState.Die:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private bool TransitionCheck()
    {
        if (isDead)
        {
            nextState = BossState.Die;
            return true;
        }
        switch (curState)
        {
            case BossState.Idle:
                if (Fly)
                {
                    targetFlyOffset = Random.Range(minFlyOffset, maxFlyOffset);
                    print("target offset is : " + targetFlyOffset);
                    
                    nextState = BossState.TakeOff;
                    return true;
                }

                if (GetCurStateInfo(0).IsTag(_animTagIdle)
                    && GetCurStateInfo(0).normalizedTime >= 3f)
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
                var _selectedPattern = patternData[0].SelectPatternAction();
                if (_selectedPattern is not null)
                {
                    _selectedPattern.targetTransform = targetTransform;
                    if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                    curPatternRoutine = StartCoroutine(_selectedPattern.PatternRoutine());
                    nextState = BossState.Action;
                    return true;
                }

                break;
                // // 타겟이 시야 범위 내에 포착
                // if (IsTargetOnSight(targetTransform))
                // {
                //     var selectedPattern = patternData[0].SelectPatternAction(targetTransform);
                //     if (selectedPattern is not null)
                //     {
                //         selectedPattern.targetTransform = targetTransform;
                //         if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                //         curPatternRoutine = StartCoroutine(selectedPattern.PatternRoutine());
                //         print("패턴 발동");
                //         nextState = BossState.Action;
                //     }
                //     
                //     return true;
                // }
                // break;
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
                    targetFlyOffset = _agent.baseOffset;
                    
                    nextState = BossState.Land;
                    return true;
                }
                if (GetCurStateInfo(0).IsTag(_animTagFly)
                    && GetCurStateInfo(0).normalizedTime >= 3f)
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
                if (Vector3.Distance(agentPosition, _agent.destination) <= 5f)  // 패턴 시작 범위 넣을 것
                {
                    // fly trace 중 destination 도착. 패턴 시작
                    print("fly trace destination 도착. 테스트로 착지하기");
                    Fly = false;
                    targetFlyOffset = _agent.baseOffset;
                    
                    nextState = BossState.Land;
                    return true;
                }
                break;
            case BossState.Land:
                if (GetCurStateInfo(0).IsTag(_animTagLand)
                    && GetCurStateInfo(0).normalizedTime >= 1f)
                {
                    nextState = BossState.Idle;
                    return true;
                }
                break;
            case BossState.Action:
                if (ActionEnded)
                {
                    ActionEnded = false;
                    nextState = Fly ? BossState.Fly : BossState.Idle;
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
    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = transform.position;
        spherePosition.y -= groundedOffset;
        
        Grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);
    }

    public bool IsTargetOnSight(float fov, float viewDist)
    {
        if (targetTransform is null) return false;

        var direction = targetTransform.position - agentPosition;
        direction.y = 0f;

        if (Vector3.Angle(direction, transform.forward) > fov * .5f) return false;
        if (Physics.Raycast(agentPosition, direction, out RaycastHit hit, viewDistance, whatIsTarget))
        {
            if (hit.transform.Equals(targetTransform)) return true;
        }
        return false;
    }
    private bool IsTargetOnSight(Transform target)
    {
        var direction = target.position - agentPosition;
        direction.y = 0f;

        if (Vector3.Angle(direction, transform.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        if (Physics.Raycast(agentPosition, direction, out RaycastHit hit, viewDistance, whatIsTarget))
        {
            if (hit.transform == target) return true;
        }
        
        return false;
    }

    private float TargetAngle(Transform target)
    {
        var direction = target.position - agentPosition;
        direction.y = 0f;
        return Vector3.Angle(direction, transform.forward);
    }
    private IEnumerator BossThink()
    {
        while (true)
        {
            // if (!isFlying)
            // {
            //     if (groundState.Equals(GroundState.Patrol))
            //     {
            //         // 패턴 선택
            //         var selectedPattern = patternData[0].SelectPatternAction();
            //         if (selectedPattern is not null)
            //         {
            //             groundState = GroundState.Attack;
            //             if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
            //             curPatternRoutine = StartCoroutine(selectedPattern.PatternRoutine());
            //             print("패턴 발동");
            //         }
            //     }
            //     else if (groundState.Equals(GroundState.Attack))
            //     {
            //         // 공격 중...
            //     }
            // }

            yield return new WaitForSeconds(.1f);
        }
    }

    public void EndAction()
    {
        ActionEnded = true;
        // _animator.SetBool("Exit", false);
    }
    public override void TakeDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject) return;

        currentHp = Mathf.Clamp(currentHp - damageMessage.damage, 0f, maxHp);

        if (currentHp <= 0f) Die();
    }

    private void Die()
    {
        isDead = true;
        targetFlyOffset = _agent.baseOffset;
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
}
