using System;
using UnityEngine;

public class Coin : ItemObject
{
    [SerializeField, Range(10f, 100f)] private float gold;
    public override void Interact(GameObject target)
    {
        Debug.Log("Use Coin : +" + gold + "G");
        UiManager.Instance.GetItem2Inventory(itemData);
        Destroy(gameObject);
    }
}
