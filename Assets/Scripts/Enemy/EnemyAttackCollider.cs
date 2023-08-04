using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    protected EnemyData EnemyData { set { enemyData = value; } }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 공격");
            var livingEntity = other.GetComponent<LIvingEntity>();
            if(livingEntity != null )
            {
                DamageMessage damageMessage;
                damageMessage.damager = GetComponentInParent<Enemy>().gameObject;
                damageMessage.damage = enemyData.Damage;
                damageMessage.stunTime = 3.0f;
            }
        }
    }
}
