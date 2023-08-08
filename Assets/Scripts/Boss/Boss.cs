using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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
        Action,
        Stun,
        GetHit,
        TakeOff,
        Fly,
        FlyForward,
        Glide,
        Land,
        Die,
    }
    private BossState curState = BossState.Idle;
    private BossState nextState = BossState.Idle;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Rigidbody _rigidBody;

    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);
    
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
    private Vector3 agentPosition;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        
        foreach (var data in patternData)
        {
            data.InitPatternData(patternContainer);;
        }

        // 변수 초기화
        maxHp = maxHealth;
        currentHp = maxHp;
        isDead = false;

        updatePath = StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        agentPosition = transform.position;
        agentPosition.y -= _agent.baseOffset;

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
            case BossState.Idle:
                targetTransform = null;
                _agent.isStopped = true;
                _animator.Play("Idle");
                break;
            case BossState.Patrol:
                _agent.isStopped = false;
                _animator.Play("Walk");
                
                _agent.SetDestination(MyUtility.GetRandomPointOnNavmesh(agentPosition, patrolRadius));
                break;
            case BossState.Trace:
                _agent.isStopped = false;
                _animator.Play("Walk");
                
                _agent.SetDestination(targetTransform.position);
                break;
            case BossState.TakeOff:
                _animator.Play("Take Off");
                break;
            case BossState.Fly:
                _animator.Play("Fly Float");
                break;
            case BossState.Land:
                _animator.Play("Land");
                break;
            case BossState.Die:
                _animator.Play("Die");
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
                break;
            case BossState.Patrol:
                break;
            case BossState.Trace:
                break;
            case BossState.TakeOff:
                if (GetCurStateInfo(0).IsTag("TakeOff"))
                {
                    _agent.baseOffset =
                        Mathf.Lerp(0f, targetFlyOffset, GetCurStateInfo(0).normalizedTime);
                }
                // _agent.baseOffset = Mathf.SmoothDamp(_agent.baseOffset, targetFlyOffset, ref smoothFlyVelocity,
                //     smoothFlyTime);
                break;
            case BossState.Fly:
                break;
            case BossState.Land:
                if (GetCurStateInfo(0).IsTag("Land"))
                {
                    _agent.baseOffset =
                        Mathf.Lerp(targetFlyOffset, 0f, GetCurStateInfo(0).normalizedTime);
                }
                // if (_agent.baseOffset <= .1f) _agent.baseOffset = 0f;
                break;
            case BossState.Die:
                if (GetCurStateInfo(0).IsTag("Die"))
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
            case BossState.Idle:
                break;
            case BossState.Patrol:
                break;
            case BossState.Trace:
                break;
            case BossState.TakeOff:
                break;
            case BossState.Fly:
                break;
            case BossState.Land:
                break;
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

                if (GetCurStateInfo(0).IsTag("Idle")
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
                if (Vector3.Distance(agentPosition, _agent.destination) <= 5f)
                {
                    // patrol 중 destination 도착. 패턴 시작
                    print("trace destination 도착. 테스트로 이륙하기");
                    targetFlyOffset = Random.Range(minFlyOffset, maxFlyOffset);
                    print("target offset is : " + targetFlyOffset);
                    
                    nextState = BossState.TakeOff;
                    return true;
                }
                break;
            case BossState.TakeOff:
                if (GetCurStateInfo(0).IsTag("TakeOff")
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

                break;
            case BossState.Land:
                if (GetCurStateInfo(0).IsTag("Land")
                    && GetCurStateInfo(0).normalizedTime >= 1f)
                {
                    nextState = BossState.Idle;
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

    private void TakeOff()
    {
        
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

    public void EndPattern()
    {
        _animator.SetBool("Exit", false);
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
            agentPosition = transform.position;
            agentPosition.y -= _agent.baseOffset;
            switch (curState)
            {
                case BossState.Idle:
                    break;
                case BossState.Patrol:
                    break;
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
}
