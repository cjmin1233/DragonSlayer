using System;
using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float stunTime;
    [SerializeField] private bool isStiff;
    [SerializeField] private GameObject damager;

    private void OnParticleCollision(GameObject other)
    {
        print("particle collision!");
        var livingEntity = other.GetComponent<LivingEntity>();
        if (livingEntity is not null)
        {
            Vector3 hitPoint = other.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            DamageMessage damageMessage = new DamageMessage(damager, hitPoint, damage, stunTime, isStiff);
            livingEntity.TakeDamage(damageMessage);
        }
    }

    private void OnParticleTrigger()
    {
        // print("particle trigger!");
    }
}
