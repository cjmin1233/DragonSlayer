using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject itemUIPrefab;
    // public RectTransform inventoryPanel;
    public RectTransform dragLayer;

    public Tooltip tooltip;

    private ItemUI _draggingItem = null;
    public ItemUI DraggingItem
    {
        get => _draggingItem;
        set => _draggingItem = value;
    }

    private ItemSlot _selectedSlot = null;
    public ItemSlot SelectedSlot
    {
        get => _selectedSlot;
        set => _selectedSlot = value;
    }

    [SerializeField] private List<ItemSlot> itemSlots = new List<ItemSlot>();

    private void Start()
    {
        for (var i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].Init(this);
        }
    }

    public void SetItem(string itemName)
    {
        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot.item == null)
            {
                GameObject tempItemUI = Instantiate(itemUIPrefab, itemSlot.transform);
                ItemUI temp = tempItemUI.GetComponent<ItemUI>();
                ItemScriptableObject tempItemData = Resources.Load<ItemScriptableObject>("Items/" + itemName);
                temp.Init(tempItemData, this, itemSlot);
                break;
            }
        }
    }

    public void SetItem(ItemScriptableObject itemData)
    {
        foreach (var itemSlot in itemSlots)
        {
            if (itemSlot.item is null)
            {
                ItemUI itemUI = Instantiate(itemUIPrefab, itemSlot.transform).GetComponent<ItemUI>();
                itemUI.Init(itemData, this, itemSlot);
                break;
            }
        }
    }
}