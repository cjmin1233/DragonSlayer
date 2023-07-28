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
        Die
    }

    private State curState = State.Idle;
    private State nextState = State.Idle;

    [SerializeField] EnemyData enemyData;
    public EnemyData EnemyData { set { enemyData = value; } }

    private NavMeshAgent agent;
    private GameObject player;
    private float turnSmoothTime = 0.3f;
    private float turnSmoothVelocity;
    private bool isStateChanged = true;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
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
                break;
            case State.Attack:
                Debug.Log($"{enemyData.EnemyName}, 공격 시작");
                break;
            case State.Die:
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
                Debug.Log($"{enemyData.EnemyName}, 추격 중지");
                agent.velocity = Vector3.zero;
                agent.isStopped = true;
                break;
            case State.Attack:
                Debug.Log($"{enemyData.EnemyName}, 공격 중지");
                agent.isStopped = false;
                break;
            case State.Die:
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
                    return true;
                }
                break;
            case State.Attack:
                if (Vector3.Distance(transform.position, player.transform.position) > enemyData.EnemyAttackRange)
                {
                    nextState = State.Trace;
                    return true;
                }
                break;
            case State.Die:
                return true;
            //break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }
}
