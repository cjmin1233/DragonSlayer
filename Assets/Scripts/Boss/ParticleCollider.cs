using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    private GameObject _boss;
    private void Awake()
    {
        _boss = GetComponentInParent<Boss>().gameObject;
    }

    private void OnParticleCollision(GameObject other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            print("particle player hit");
            DamageMessage damageMessage = new DamageMessage(_boss, 10f, 0f, true);
            playerHealth.TakeDamage(damageMessage);
        }
    }
}
