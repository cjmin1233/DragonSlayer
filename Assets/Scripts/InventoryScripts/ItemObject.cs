using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected ItemScriptableObject itemData;
    [SerializeField] protected GameObject interactVfx;
    public virtual void Interact(GameObject target)
    {
        UiManager.Instance.GetItem2Inventory(itemData);
        if (interactVfx is not null)
            Instantiate(interactVfx, target.transform, false);
        Destroy(gameObject);
    }

    public virtual void EnterInteract(GameObject target)
    {
        // enable ui
        UiManager.Instance.ShowInteractInfo($"Use {itemData.itemName}");
    }

    public float GetItemWeight() => itemData.itemWeight;
    public ItemTier GetItemTier() => itemData.itemTier;
}
