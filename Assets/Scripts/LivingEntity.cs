using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamagable
{
    [SerializeField] protected Transform headPoint;
    protected Rigidbody rb;
    protected float maxHp;
    protected float currentHp;

    protected bool isStunned;

    protected Coroutine stunning;
    protected float RemainingStunTime;

    public virtual void TakeDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject) return;

        currentHp = Mathf.Clamp(currentHp - damageMessage.damage, 0f, maxHp);
        if (damageMessage.stunTime > 0f && damageMessage.stunTime > RemainingStunTime)
        {
            if (stunning is not null) StopCoroutine(stunning);
            stunning = StartCoroutine(StunProcess(damageMessage.stunTime));
        }
        //if (currentHp <= 0f) Die();
    }

    protected virtual IEnumerator StunProcess(float stunTime)
    {
        RemainingStunTime = stunTime;
        isStunned = true;

        if (headPoint is not null)
        {
            var stunVfx = EffectManager.Instance.GetFromPool((int)EffectType.StunCirclingStars);
            stunVfx.transform.SetParent(headPoint);
            stunVfx.transform.localPosition = Vector3.zero;
            ParticleSystem.MainModule mainModule = stunVfx.GetComponent<ParticleSystem>().main;
            mainModule.duration = stunTime;
            stunVfx.SetActive(true);
        }
        while (isStunned)
        {
            RemainingStunTime -= Time.deltaTime;
            if (RemainingStunTime <= 0f) isStunned = false;
            yield return null;
        }

    }
}

