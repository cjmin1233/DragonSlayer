using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    protected EnemyData EnemyData { set { enemyData = value; } }

    private GameObject damager;
    private List<GameObject> hitList = new List<GameObject>();
    private GameObject GetDamager() => damager = GetComponentInParent<Enemy>().gameObject;
    private void Awake()
    {
        GetDamager();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (hitList.Contains(other.gameObject)) return;
            
            var livingEntity = other.GetComponent<LivingEntity>();
            if (livingEntity is not null)
            {
                if (damager is null) GetDamager();
                var isStiff = Random.Range(0, 10) > 6 ? true : false;
                var stunTIme = isStiff ? Random.Range(0, 10) > 6 ? 1f : 0f : 0f;
                DamageMessage damageMessage =
                    new DamageMessage(damager,
                        other.ClosestPointOnBounds(transform.position), enemyData.Damage, stunTIme, isStiff);
                
                livingEntity.TakeDamage(damageMessage);
                hitList.Add(other.gameObject);
            }
        }
    }

    private void OnDisable()
    {
        hitList.Clear();
    }
}
