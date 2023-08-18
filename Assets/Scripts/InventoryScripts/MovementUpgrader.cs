using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementUpgrader : ItemObject
{
    [SerializeField] private float upgradeMoveSpeed;
    public override void Interact(GameObject target)
    {
        var playerMove = target.GetComponent<PlayerMove>();
        if (playerMove is not null) playerMove.UpgradeStatus(upgradeMoveSpeed);
        base.Interact(target);
    }
}
