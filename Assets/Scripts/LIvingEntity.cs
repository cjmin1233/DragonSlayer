using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIvingEntity : MonoBehaviour, IDamagable
{
    protected float maxHp;
    protected float currentHp;

    public virtual void TakeDamage(DamageMessage damageMessage)
    {
        if (damageMessage.damager == gameObject) return;

        currentHp = Mathf.Clamp(currentHp - damageMessage.damage, 0f, maxHp);

        //if (currentHp <= 0f) Die();
    }
}
