using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float stunTime;
    [SerializeField] private bool isStiff;
    [SerializeField] private GameObject damager;

    private void OnParticleCollision(GameObject other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            Vector3 hitPoint = other.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            DamageMessage damageMessage = new DamageMessage(damager, hitPoint, damage, stunTime, isStiff);
            playerHealth.TakeDamage(damageMessage);
        }
    }
}
