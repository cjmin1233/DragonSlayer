using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShockWave : MonoBehaviour
{
    [SerializeField] private float shockRadius;
    [SerializeField] private LayerMask whatIsTarget;
    private void OnEnable()
    {
        print("shock wave enabled");
        Collider[] hitsInfo = new Collider[10];
        int numColliders =
            Physics.OverlapSphereNonAlloc(transform.position, shockRadius, hitsInfo, whatIsTarget);
        for (int i = 0; i < numColliders; i++)
        {
            var playerHealth = hitsInfo[i].GetComponent<PlayerHealth>();
            if (playerHealth is not null)
            {
                var boss = GetComponentInParent<Boss>().gameObject;
                DamageMessage damageMessage = new DamageMessage(boss, 0f, 2f);
                playerHealth.TakeDamage(damageMessage);
            }
        }
    }
}
