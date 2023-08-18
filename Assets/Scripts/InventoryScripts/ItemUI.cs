using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private ItemScriptableObject itemData;
    private Image itemImage;
    private RectTransform rectTransform;

    private InventoryManager inventoryManager;
    private ItemSlot itemSlot;

    public void Init(ItemScriptableObject data, InventoryManager manager, ItemSlot slot)
    {
        rectTransform = GetComponent<RectTransform>();
        
        itemData = data;
        itemImage = GetComponent<Image>();
        itemImage.sprite = itemData.itemImage;

        inventoryManager = manager;
        itemSlot = slot;
        itemSlot.item = this;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryManager.tooltip.SetTooltip(rectTransform.position, itemData.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryManager.tooltip.Disable();
    }

    //-------------------------------------------------------------------


    public void OnPointerDown(PointerEventData eventData)
    {
        inventoryManager.DraggingItem = this;
        itemImage.raycastTarget = false;
        
        rectTransform.SetParent(inventoryManager.dragLayer);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inventoryManager.DraggingItem = null;

        if (inventoryManager.SelectedSlot == null)
        {
            rectTransform.SetParent(itemSlot.transform);
        }
        else
        {
            if (inventoryManager.SelectedSlot.item == null)
            {
                itemSlot.item = null;
                rectTransform.SetParent(inventoryManager.SelectedSlot.transform);
                itemSlot = inventoryManager.SelectedSlot;
            }
            else //Item Swap
            {
                inventoryManager.SelectedSlot.item.ChangeSlot(itemSlot);
                ChangeSlot(inventoryManager.SelectedSlot);
            }
            
        }

        rectTransform.localPosition = Vector3.zero;
        itemImage.raycastTarget = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    private void ChangeSlot(ItemSlot slot)
    {
        slot.item = this;
        rectTransform.SetParent(slot.transform);
        itemSlot = slot;
        rectTransform.localPosition = Vector3.zero;
    }
}
