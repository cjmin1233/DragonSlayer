using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ItemObject
{
    [SerializeField] private float healAmount;
    [SerializeField] private GameObject healVfx;
    public override void Interact(GameObject target)
    {
        var playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth is not null)
        {
            playerHealth.RestoreHealth(healAmount);
            Instantiate(healVfx, GetComponent<Collider>().bounds.center, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
