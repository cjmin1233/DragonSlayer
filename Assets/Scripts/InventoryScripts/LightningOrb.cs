using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : ItemObject
{
    [SerializeField] private GameObject effectGeneratorPrefab;
    public override void Interact(GameObject target)
    {
        if (effectGeneratorPrefab is not null)
        {
            var instance = Instantiate(effectGeneratorPrefab, target.transform, false)
                .GetComponent<EffectRandomGenerator>();
            if (instance is not null) instance.SetAttacker(target);
        }
        base.Interact(target);
    }
}
