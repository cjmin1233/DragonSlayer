using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] EnemyData enemyData;
    protected EnemyData EnemyData { set { enemyData = value; } }

    private Rigidbody rb;
    public GameObject damager;
    
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
            var livingEntity = other.GetComponent<LivingEntity>();
            if (livingEntity != null)
            {
                DamageMessage damageMessage = new DamageMessage(this.damager, enemyData.Damage, 0.5f);
                
                livingEntity.TakeDamage(damageMessage);
            }
            Destroy(gameObject);
        }
    }
}
