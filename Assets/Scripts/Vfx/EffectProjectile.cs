using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EffectProjectile : MonoBehaviour
{
    [SerializeField] private EffectType effectType;
    [SerializeField] private float damage;
    [SerializeField] private float stunTime;
    [SerializeField] private bool isStiff;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private EffectType hitVfxType;
    private List<GameObject> hitList = new List<GameObject>();
    private GameObject _attacker;
    private Collider _collider;
    public void SetAttacker(GameObject attacker) => _attacker = attacker;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject)) return;
        var damagable = other.GetComponent<IDamagable>();
        if (damagable is null) return;
        if (whatIsTarget == (whatIsTarget | (1 << other.gameObject.layer)))
        {
            Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
            DamageMessage damageMessage = new DamageMessage(_attacker, hitPos, damage, stunTime, isStiff);

            damagable.TakeDamage(damageMessage);
            SpawnEffects(damageMessage);
            hitList.Add(other.gameObject);
        }
    }

    private void OnParticleSystemStopped()
    {
        ClearHitList();
        EffectManager.Instance.Add2Pool((int)effectType, gameObject);
    }
    private void SpawnEffects(DamageMessage damageMessage)
    {
        var hitVfx = EffectManager.Instance.GetFromPool((int)hitVfxType);
        if (hitVfx is not null)
        {
            hitVfx.transform.position = damageMessage.hitPoint;
            hitVfx.SetActive(true);
        }

        var damageText = EffectManager.Instance.GetFromPool((int)EffectType.DamageText);
        if (damageText is not null)
        {
            damageText.transform.position = damageMessage.hitPoint;
            damageText.GetComponent<DamageText>().SetDamageValue(damageMessage.damage);
            damageText.transform.rotation = Quaternion.LookRotation(damageMessage.damager.transform.forward);
            damageText.SetActive(true);
        }
    }

    public void ClearHitList()
    {
        _collider.enabled = false;
        hitList.Clear();
        _collider.enabled = true;
    }
}
