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
        print($"particle collision with {other.name}!");
        var livingEntity = other.GetComponent<LivingEntity>();
        if (!livingEntity) return;
        var col = livingEntity.GetComponent<Collider>();
        if (!col) return;
        
        Vector3 hitPoint = livingEntity.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        DamageMessage damageMessage = new DamageMessage(damager, hitPoint, damage, stunTime, isStiff);
        livingEntity.TakeDamage(damageMessage);
    }
}
