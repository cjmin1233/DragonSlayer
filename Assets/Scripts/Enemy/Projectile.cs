using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    protected EnemyData EnemyData { set { enemyData = value; } }

    private Rigidbody rb;
    
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 5f;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            Debug.Log("플레이어 명중");
            var livingEntity = other.GetComponent<LIvingEntity>();
            if (livingEntity != null)
            {
                DamageMessage damageMessage;
                damageMessage.damager = GetComponentInParent<Enemy>().gameObject;
                damageMessage.damage = enemyData.Damage;
                damageMessage.stunTime = 3.0f;
            }
            Destroy(gameObject);
        }
    }
}
