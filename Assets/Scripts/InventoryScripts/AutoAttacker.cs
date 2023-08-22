using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoAttacker : MonoBehaviour
{
    [SerializeField] private EffectProjectile[] effectProjectiles;
    [SerializeField] private float attackDuration;
    private GameObject _attacker;
    private Coroutine autoAttackRoutine;
    
    public void SetAttacker(GameObject attacker)
    {
        _attacker = attacker;
        foreach (var projectile in effectProjectiles)
        {
            projectile.SetAttacker(_attacker);
        }
    }
    private void OnEnable()
    {
        if (autoAttackRoutine is not null) StopCoroutine(autoAttackRoutine);
        autoAttackRoutine = StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackDuration);
            foreach (var projectile in effectProjectiles)
            {
                projectile.ClearHitList();
            }
        }
    }
}
