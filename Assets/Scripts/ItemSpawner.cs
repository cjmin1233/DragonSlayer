using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ItemTier
{
    Common,
    Rare,
    Epic
}
public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instacne { get; private set; }

    // private List<ItemObject> itemList;
    private ItemObject[] _items;
    private void Awake()
    {
        if (Instacne is null) Instacne = this;
        else if (!Instacne.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // itemList = new List<ItemObject>();
    }

    private void Start()
    {
        // var items = GetComponentsInChildren<ItemObject>();
        // foreach (var item in items)
        // {
        //     itemList.Add(item);
        //     item.gameObject.SetActive(false);
        // }

        _items = GetComponentsInChildren<ItemObject>();
        for (int i = 0; i < _items.Length; i++)
        {
            _items[i].SetItemEnabled(true);
            _items[i].SetItemIndex(i);
            _items[i].gameObject.SetActive(false);
        }
    }
    
    public GameObject GetRandomItem()
    {
        float totalWeight = 0f;
        foreach (var item in _items)
        {
            if (!item.ItemEnabled) continue;
            totalWeight += item.GetItemWeight();
        }
        // foreach (var item in itemList)
        // {
        //     totalWeight += item.GetItemWeight();
        // }

        var randomValue = Random.Range(0f, totalWeight);
        var curWeight = 0f;

        ItemObject selectedItemObject = null;
        foreach (var item in _items)
        {
            if (!item.ItemEnabled) continue;
            curWeight += item.GetItemWeight();
            if (randomValue < curWeight)
            {
                selectedItemObject = item;
                break;
            }
        }

        if (selectedItemObject is null) return null;

        // if (selectedItemObject.GetItemTier().Equals(ItemTier.Epic)) itemList.Remove(selectedItemObject);

        return Instantiate(selectedItemObject.gameObject);
    }

    public void DisableItem(int itemIndex)
    {
        var item = _items[itemIndex];
        if (item.ItemEnabled)
        {
            print($"{item.name} disabled");
            item.SetItemEnabled(false);
        }
    }
}
