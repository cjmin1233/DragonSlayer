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
            Debug.Log("�÷��̾� ����");
            var livingEntity = other.GetComponent<LIvingEntity>();
            if(livingEntity != null )
            {
                DamageMessage damageMessage =
                    new DamageMessage(GetComponentInParent<Enemy>().gameObject, enemyData.Damage, 3f);

                livingEntity.TakeDamage(damageMessage);
            }
        }
    }
}
