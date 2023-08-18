using System;
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
        if (itemData.pickUp && UiManager.Instance is not null) UiManager.Instance.GetItem2Inventory(itemData);
        if (interactVfx is not null)
            Instantiate(interactVfx, GetComponent<Collider>().bounds.center, Quaternion.identity, target.transform);
        Destroy(gameObject);
    }

    public virtual void EnterInteract(GameObject target)
    {
        // enable ui
        UiManager.Instance.ShowInteractInfo($"Use {itemData.itemName}");
    }

    public float GetItemWeight() => itemData.itemWeight;
    public ItemTier GetItemTier() => itemData.itemTier;

    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.Add2InteractList(this);
    }

    private void OnTriggerExit(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.RemoveInteractable(this);
    }
}
