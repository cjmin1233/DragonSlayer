using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : LIvingEntity
{
    private enum State
    {
        Idle,
        Trace,
        Battle,
        Attack,
        GetHit,
        Die
    }

    private State curState = State.Idle;
    private State nextState = State.Idle;

    [SerializeField] EnemyData enemyData;
    protected EnemyData EnemyData { set { enemyData = value; } }

    private NavMeshAgent agent;
    private GameObject player;
    protected Animator animator;
    protected EnemyEvent enemyEvent;
    private float turnSmoothTime = 0.3f;
    private float turnSmoothVelocity;
    private bool isStateChanged = true;

    private Coroutine timer;
    [SerializeField] float attackDuration;
    [SerializeField] float getHitDuration;
    private bool battleToAttack, attackToBattle, getHitAgain;

    protected virtual void Awake()
    {
        maxHp = enemyData.EnemyHp;
        currentHp = enemyData.EnemyHp;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        enemyEvent = GetComponent<EnemyEvent>();
    }

    private void Update()
    {
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
            case State.Idle:
                animator.Play("IdleNormal");
                break;
            case State.Trace:
                animator.Play("WalkFWD");
                animator.SetBool("isAttacking", false);
                agent.isStopped = false;
                break;
            case State.Battle:
                animator.Play("IdleBattle");
                timer = StartCoroutine(Battle2Attack());
                break;
            case State.Attack:
                animator.Play("Attack01");
                timer = StartCoroutine(Attack2Battle());
                animator.SetBool("isAttacking", true);
                break;
            case State.GetHit:
                animator.Play("GetHit", -1, 0f);
                animator.SetBool("isGetHit", false);
                timer = StartCoroutine(GetHitAgain());
                break;
            case State.Die:
                animator.Play("Die");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void StateUpdate()
    {
        switch (curState)
        {
            case State.Idle:
                break;
            case State.Trace:
                var lookRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                var targetAngleY = lookRotation.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
                
                agent.SetDestination(player.transform.position);
                break;
            case State.Battle:
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                targetAngleY = lookRotation.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
                break;
            case State.Attack:
                lookRotation = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
                targetAngleY = lookRotation.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, ref turnSmoothVelocity, turnSmoothTime);
                break;
            case State.GetHit:
                break;
            case State.Die:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private void StateExit()
    {
        switch (curState)
        {
            case State.Idle:
                break;
            case State.Trace:
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                break;
            case State.Battle:
                StopCoroutine(timer);
                battleToAttack = false;
                break;
            case State.Attack:
                StopCoroutine(timer);
                attackToBattle = false;
                break;
            case State.GetHit:
                StopCoroutine(timer);
                getHitAgain = false;
                break;
            case State.Die:
                Debug.Log($"{enemyData.EnemyName}, Á×À½...");
                Invoke("AfterDie", 5f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private bool TransitionCheck()
    {
        switch (curState)
        {
            case State.Idle:
                nextState = State.Trace;
                return true;
            case State.Trace:
                if (Vector3.Distance(transform.position, player.transform.position) <= enemyData.EnemyAttackRange)
                {
                    nextState = State.Battle;
                    return true;
                }
                if (animator.GetBool("isGetHit"))
                {
                    nextState = State.GetHit;
                    return true;
                }
                break;
            case State.Battle:
                if (Vector3.Distance(transform.position, player.transform.position) > enemyData.EnemyAttackRange)
                {
                    nextState = State.Trace;
                    return true;
                }
                if (animator.GetBool("isGetHit"))
                {
                    nextState = State.GetHit;
                    return true;
                }
                if (!animator.GetBool("isAttacking"))
                {
                    nextState = State.Attack;
                    return true;
                }
                else if (battleToAttack)
                {
                    nextState = State.Attack;
                    return true;
                }
                break;
            case State.Attack:
                if (animator.GetBool("isGetHit"))
                {
                    nextState = State.GetHit;
                    return true;
                }
                if (attackToBattle)
                {
                    nextState = State.Battle;
                    return true;
                }
                break;
            case State.GetHit:
                if (currentHp <= 0)
                {
                    nextState = State.Die;
                    return true;
                }
                if (animator.GetBool("isGetHit"))
                {
                    return true;
                }
                if (getHitAgain)
                {
                    nextState = State.Trace;
                    return true;
                }
                break;
            case State.Die:
                nextState = State.Idle;
                return true;
            //break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    }

    public override void TakeDamage(DamageMessage damageMessage)
    {
        base.TakeDamage(damageMessage);

        animator.SetBool("isGetHit", true);
    }

    private IEnumerator Battle2Attack()
    {
        yield return new WaitForSeconds(2f);
        battleToAttack = true;
    }
    private IEnumerator Attack2Battle()
    {
        yield return new WaitForSeconds(attackDuration);
        attackToBattle = true;
    }
    private IEnumerator GetHitAgain()
    {
        yield return new WaitForSeconds(getHitDuration);
        getHitAgain = true;
    }
    private void AfterDie() => EnemySpawner.Instance.Add2Pool((int)enemyData.EnemyType, gameObject);
}
