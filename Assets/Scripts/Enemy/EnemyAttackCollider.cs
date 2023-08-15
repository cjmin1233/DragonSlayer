using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    protected EnemyData EnemyData { set { enemyData = value; } }

    private List<GameObject> hitList = new List<GameObject>();
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (hitList.Contains(other.gameObject)) return;
            
            var livingEntity = other.GetComponent<LivingEntity>();
            if (livingEntity is not null)
            {
                var isStiff = Random.Range(0, 10) > 6 ? true : false;
                var stunTIme = isStiff ? Random.Range(0, 10) > 6 ? 1f : 0f : 0f;
                DamageMessage damageMessage =
                    new DamageMessage(GetComponentInParent<Enemy>().gameObject, enemyData.Damage, stunTIme, isStiff);

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
