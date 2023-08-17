using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ItemObject
{
    [SerializeField] private float healAmount;
    public override void Interact(GameObject target)
    {
        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.RestoreHealth(healAmount);

        base.Interact(target);
    }
}
