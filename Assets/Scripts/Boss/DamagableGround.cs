using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableGround : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float radius;
    [SerializeField] private float height;
    [SerializeField] private float damageDuration;
    [SerializeField] private float damageInterval;
    [SerializeField] private bool isStiff;
    [SerializeField] private LayerMask whatIsTarget;
    private Coroutine damageRoutine;
    private void OnEnable()
    {
        if (damageRoutine is not null) StopCoroutine(damageRoutine);
        damageRoutine = StartCoroutine(DamageRoutine());
    }

    private IEnumerator DamageRoutine()
    {
        float timer = damageDuration;
        while (timer > 0f)
        {
            timer -= damageInterval;
            // 데미지 계산
            DamageEntities();
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void DamageEntities()
    {
        DamageMessage damageMessage = new DamageMessage(gameObject, damage, 0f, isStiff);
        var colliders = Physics.OverlapSphere(transform.position, radius, whatIsTarget);

        foreach (var coll in colliders)
        {
            if (Mathf.Abs(coll.transform.position.y - transform.position.y) > height) continue;
            var livingEntity = coll.GetComponent<LivingEntity>();
            if (livingEntity is not null) livingEntity.TakeDamage(damageMessage);
        }
    }
}
