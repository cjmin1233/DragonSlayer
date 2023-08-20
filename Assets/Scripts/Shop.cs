using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopStand[] itemStands;
    [SerializeField] private int priceUnit;

    private void Start()
    {
        itemStands = GetComponentsInChildren<ShopStand>();
        SpawnItems();
    }

    private void SpawnItems()
    {
        print("spawn items ****************");
        for (int i = 0; i < itemStands.Length; i++)
        {
            var item = ItemSpawner.Instacne.GetRandomItem();
            itemStands[i].StandSetup(item, priceUnit);
        }
    }
}
