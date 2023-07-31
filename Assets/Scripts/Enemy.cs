using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum State
    {
        Idle,
        Trace,
        Attack,
        GetHit,
        Die
    }

    private State curState = State.Idle;
    private State nextState = State.Idle;

    [SerializeField] EnemyData enemyData;
    public EnemyData EnemyData { set { enemyData = value; } }
    public int hp;

    private NavMeshAgent agent;
    private GameObject player;
    [SerializeField] private Animator animator;
    private float turnSmoothTime = 0.3f;
    private float turnSmoothVelocity;
    private bool isStateChanged = true;

    void Awake()
    {
        hp = enemyData.EnemyHp;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
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
                break;
            case State.Trace:
                Debug.Log($"{enemyData.EnemyName}, 추격 시작");
                agent.isStopped = false;
                break;
            case State.Attack:
                Debug.Log($"{enemyData.EnemyName}, 공격 시작");
                break;
            case State.GetHit:
                animator.SetTrigger("GetHit");
                animator.SetBool("State", false);
                break;
            case State.Die:
                animator.SetTrigger("Die");
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
                animator.SetTrigger("Find Player");
                break;
            case State.Trace:
                Debug.Log($"{enemyData.EnemyName}, 추격 중지");
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                break;
            case State.Attack:
                Debug.Log($"{enemyData.EnemyName}, 공격 중지");
                break;
            case State.GetHit:
                break;
            case State.Die:
                Debug.Log($"{enemyData.EnemyName}, 죽음...");
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
                    nextState = State.Attack;
                    animator.SetBool("State", true);
                    return true;
                }
                if (animator.GetBool("isGetHit"))
                {
                    nextState = State.GetHit;
                    animator.SetTrigger("GetHit");
                    return true;
                }
                break;
            case State.Attack:
                if (Vector3.Distance(transform.position, player.transform.position) > enemyData.EnemyAttackRange)
                {
                    nextState = State.Trace;
                    animator.SetBool("State", false);
                    return true;
                }
                if (animator.GetBool("isGetHit"))
                {
                    nextState = State.GetHit;
                    animator.SetTrigger("GetHit");
                    return true;
                }
                break;
            case State.GetHit:
                if(hp <= 0) nextState = State.Die;
                else nextState = State.Trace;
                return true;
                //break;
            case State.Die:
                nextState = State.Idle;
                return true;
            //break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    }
    private void AfterDie() => EnemySpawner.Instance.Add2Pool((int)enemyData.EnemyType, gameObject);
}
