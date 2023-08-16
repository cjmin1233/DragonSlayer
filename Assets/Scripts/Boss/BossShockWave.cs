using UnityEngine;

public class BossShockWave : MonoBehaviour
{
    [SerializeField] private GameObject damager;
    [SerializeField] private float shockRadius;
    [SerializeField] private float stunTime;
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
                DamageMessage damageMessage = new DamageMessage(damager,
                    hitsInfo[i].ClosestPointOnBounds(transform.position), 0f, stunTime);
                livingEntity.TakeDamage(damageMessage);
            }
        }
    }
}
