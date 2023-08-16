using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] protected ItemScriptableObject itemData;
    public virtual void Interact(GameObject target)
    {
        UiManager.Instance.GetItem2Inventory(itemData);
        Destroy(gameObject);
    }
}
