using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardUpgrader : ItemObject
{
    public override void Interact(GameObject target)
    {
        var playerCombat = target.GetComponent<PlayerCombat>();
        if(playerCombat is not null)playerCombat.UpgradeGuard();
        base.Interact(target);
    }
}
