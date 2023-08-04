using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIvingEntity : MonoBehaviour, IDamagable
{
    protected Rigidbody rb;
    protected float maxHp;
    protected float currentHp;

    protected bool isStunned;

    protected Coroutine stunning;
    protected float remainTime;

    public virtual void TakeDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject) return;

        currentHp = Mathf.Clamp(currentHp - damageMessage.damage, 0f, maxHp);
        if (damageMessage.stunTime > 0f)
        {
            if (stunning is not null) StopCoroutine(stunning);
            stunning = StartCoroutine(StunProcess(damageMessage.stunTime));
        }
        // if (stunning is not null) StopCoroutine(stunning);
        // if (damageMessage.stunTime > 0f) stunning = StartCoroutine(StunProcess(damageMessage.stunTime));

        //if (currentHp <= 0f) Die();
    }

    protected IEnumerator StunProcess(float time)
    {
        remainTime = time;
        isStunned = true;

        while (isStunned)
        {
            remainTime -= Time.deltaTime;
            if (remainTime <= 0f) isStunned = false;
            yield return null;
        }

    }
}

