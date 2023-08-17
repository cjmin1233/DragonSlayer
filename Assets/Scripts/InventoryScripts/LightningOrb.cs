using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : ItemObject
{
    [SerializeField] private GameObject autoAttackerPrefab;
    public override void Interact(GameObject target)
    {
        if(autoAttackerPrefab is not null) Instantiate(autoAttackerPrefab, target.transform, false);
        base.Interact(target);
    }
}
