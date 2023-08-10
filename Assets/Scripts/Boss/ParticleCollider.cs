using UnityEngine;

public class ParticleCollider : MonoBehaviour
{
    private GameObject damager;
    private void Awake()
    {
        damager = GetComponentInParent<LivingEntity>().gameObject;
    }

    private void OnParticleCollision(GameObject other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            print("particle player hit");
            DamageMessage damageMessage = new DamageMessage(damager, 10f, 0f, true);
            playerHealth.TakeDamage(damageMessage);
        }
    }
}
