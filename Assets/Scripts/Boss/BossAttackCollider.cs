using System.Collections.Generic;
using UnityEngine;

public class BossAttackCollider : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float stunTime;
    [SerializeField] private bool isStiff;
    [SerializeField] private GameObject damager;
    private List<GameObject> hitList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject)) return;
        
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);
            DamageMessage damageMessage = new DamageMessage(damager, hitPoint, damage, stunTime, isStiff);
            
            playerHealth.TakeDamage(damageMessage);
            hitList.Add(other.gameObject);
        }
    }

    private void OnDisable()
    {
        hitList.Clear();
    }
}
