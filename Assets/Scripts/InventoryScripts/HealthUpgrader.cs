using UnityEngine;

public class HealthUpgrader : ItemObject
{
    [SerializeField] private float upgradeMaxHealth;
    public override void Interact(GameObject target)
    {
        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.UpgradeMaxHealth(upgradeMaxHealth);
        base.Interact(target);
    }
}
