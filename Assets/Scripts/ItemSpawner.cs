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
    public bool trigger;

    private List<ItemObject> itemList;
    private void Awake()
    {
        if (Instacne is null) Instacne = this;
        else if (!Instacne.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        itemList = new List<ItemObject>();
    }

    private void Start()
    {
        var items = GetComponentsInChildren<ItemObject>();
        foreach (var item in items)
        {
            itemList.Add(item);
            item.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (trigger)
        {
            trigger = false;
            GetRandomItem();
        }
    }

    public GameObject GetRandomItem()
    {
        float totalWeight = 0f;
        foreach (var item in itemList)
        {
            totalWeight += item.GetItemWeight();
        }

        var randomValue = Random.Range(0f, totalWeight);
        var curWeight = 0f;

        ItemObject selectedItemObject = null;
        foreach (var item in itemList)
        {
            curWeight += item.GetItemWeight();
            if (randomValue < curWeight)
            {
                selectedItemObject = item;
                break;
            }
        }

        if (selectedItemObject is null) return null;

        if (selectedItemObject.GetItemTier().Equals(ItemTier.Epic)) itemList.Remove(selectedItemObject);

        return Instantiate(selectedItemObject.gameObject);
    }
}
