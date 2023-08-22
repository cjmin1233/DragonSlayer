using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EffectRandomGenerator : MonoBehaviour
{
    [SerializeField] private float spawnRadius;
    [SerializeField] private float spawnDuration;
    [SerializeField] private EffectType effectType;
    private GameObject _attacker;
    private Coroutine autoAttackRoutine;
    public void SetAttacker(GameObject attacker) => _attacker = attacker;
    private void OnEnable()
    {
        if (autoAttackRoutine is not null) StopCoroutine(autoAttackRoutine);
        autoAttackRoutine = StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        while (true)
        {
            var projectile = EffectManager.Instance.GetFromPool((int)effectType).GetComponent<EffectProjectile>();
            if (projectile is not null)
            {
                projectile.SetAttacker(_attacker);
                projectile.transform.position =
                    MyUtility.GetRandomPointBet2Circles(transform.position, 0f, spawnRadius);
                projectile.gameObject.SetActive(true);
            }
            // attackObject.transform.position = MyUtility.GetRandomPointBet2Circles(transform.position, 0f, spawnRadius);
            // attackObject.SetActive(true);
            // if(_particleSystem is not null) _particleSystem.Play();
            yield return new WaitForSeconds(spawnDuration);
        }
    }
}
