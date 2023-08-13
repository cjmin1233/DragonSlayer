using UnityEngine;

public class BossShockWave : MonoBehaviour
{
    [SerializeField] private float shockRadius;
    [SerializeField] private LayerMask whatIsTarget;
    private void OnEnable()
    {
        print("shock wave enabled");
        Collider[] hitsInfo = new Collider[10];
        int numColliders =
            Physics.OverlapSphereNonAlloc(transform.position, shockRadius, hitsInfo, whatIsTarget);
        for (int i = 0; i < numColliders; i++)
        {
            var livingEntity = hitsInfo[i].GetComponent<LivingEntity>();
            if (livingEntity is not null)
            {
                var damagerEntity = GetComponentInParent<LivingEntity>().gameObject;
                DamageMessage damageMessage = new DamageMessage(damagerEntity, 0f, 2f);
                livingEntity.TakeDamage(damageMessage);
            }
        }
    }
}
