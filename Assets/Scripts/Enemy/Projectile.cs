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
                var hitPos = other.ClosestPointOnBounds(transform.position);

                DamageMessage damageMessage = new DamageMessage(this.damager, hitPos, enemyData.Damage, 0f, true);
                
                var hitVfx = EffectManager.Instance.GetFromPool(2);
                hitVfx.transform.position = hitPos;
                hitVfx.SetActive(true);

                livingEntity.TakeDamage(damageMessage);
            }
            Destroy(gameObject);
        }
    }
}
