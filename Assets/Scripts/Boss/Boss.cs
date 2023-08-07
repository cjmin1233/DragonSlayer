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
    private bool isFlying;
    private enum FlyingState
    {
        Float,
        Glide,
        Attack
    }

    private enum GroundState
    {
        Patrol,
        Track,
        Attack,
        Groggy,
        Sleep
    }

    private enum BossState
    {
        Idle,
        Patrol,
        Trace,
        Action,
        Stun,
        GetHit,
        Fly,
        FlyForward,
        Glide,
        Dead,
    }
    
    private GroundState groundState;
    private FlyingState flyingState;
    private BossState curState = BossState.Idle;
    private BossState nextState = BossState.Idle;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Rigidbody _rigidBody;

    [SerializeField] private float turnSmoothTime = .3f;
    private float turnSmoothVelocity;
    private bool isStateChanged = true;
    private Coroutine thinkRoutine;

    [SerializeField] private Transform patternContainer;
    [SerializeField] private BossPatternData[] patternData;
    private Coroutine curPatternRoutine;

    public bool Grounded;

    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.28f;
    public LayerMask groundLayers;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        groundState = GroundState.Patrol;
        
        foreach (var data in patternData)
        {
            data.InitPatternData(patternContainer);;
        }

        // StartCoroutine(BossBrain());
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
            case BossState.Idle:
                _animator.Play("Idle");
                break;
            case BossState.Fly:
                _animator.Play("Fly Float");
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
            case BossState.Fly:
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
            case BossState.Fly:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool TransitionCheck()
    {
        switch (curState)
        {
            case BossState.Idle:
                if (!Grounded)
                {
                    nextState = BossState.Fly;
                    return true;
                }
                break;
            case BossState.Fly:
                if (Grounded)
                {
                    nextState = BossState.Idle;
                    return true;
                }

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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Sphere의 색상 설정
        Vector3 spherePosition = transform.position;
        spherePosition.y -= groundedOffset;

        // 현재 오브젝트의 위치에 Sphere를 그림
        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }
    private IEnumerator BossThink()
    {
        while (true)
        {
            if (!isFlying)
            {
                if (groundState.Equals(GroundState.Patrol))
                {
                    // 패턴 선택
                    var selectedPattern = patternData[0].SelectPatternAction();
                    if (selectedPattern is not null)
                    {
                        groundState = GroundState.Attack;
                        if (curPatternRoutine is not null) StopCoroutine(curPatternRoutine);
                        curPatternRoutine = StartCoroutine(selectedPattern.PatternRoutine());
                        print("패턴 발동");
                    }
                }
                else if (groundState.Equals(GroundState.Attack))
                {
                    // 공격 중...
                }
            }

            yield return new WaitForSeconds(.1f);
        }
    }

    // private IEnumerator PatternRoutine(BossPatternAction patternAction)
    // {
    //     yield break;
    // }

    public void EndPattern()
    {
        _animator.SetBool("Exit", false);
        groundState = GroundState.Patrol;
    }
    public override void TakeDamage(DamageMessage damageMessage)
    {
        return;
    }
}
