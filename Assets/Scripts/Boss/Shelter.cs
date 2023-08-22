using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : MonoBehaviour
{
    // private Animator _animator;
    private bool isPlayerIn;
    private Coroutine shelterRoutine;
    private PlayerHealth playerTarget;

    // private void Awake()
    // {
    //     _animator = GetComponent<Animator>();
    // }

    private void OnEnable()
    {
        // _animator.Play("ShelterEnable");
        shelterRoutine = StartCoroutine(ShelterRoutine());
    }

    private IEnumerator ShelterRoutine()
    {
        while (true)
        {
            if (isPlayerIn && playerTarget is not null) playerTarget.MakeInvincible(10f);
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(shelterRoutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        playerTarget = other.GetComponent<PlayerHealth>();
        if (playerTarget is not null) isPlayerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerTarget = other.GetComponent<PlayerHealth>();
        if (playerTarget is not null) isPlayerIn = false;
    }
}
