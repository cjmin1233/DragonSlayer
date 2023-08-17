using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : ItemObject
{
    [SerializeField] private float upgradePower;
    [SerializeField] private float upgradeSpeed;
    public override void Interact(GameObject target)
    {
        var playerCombat = target.GetComponent<PlayerCombat>();
        if (playerCombat is not null) playerCombat.UpgradeStatus(upgradePower, upgradeSpeed);
        
        base.Interact(target);
    }
}
