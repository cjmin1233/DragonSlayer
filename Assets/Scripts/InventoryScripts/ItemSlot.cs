using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryManager manager;
    public ItemUI item;

    public void Init(InventoryManager inventoryManager)
    {
        manager = inventoryManager;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.SelectedSlot = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.SelectedSlot = null;
    }
}
