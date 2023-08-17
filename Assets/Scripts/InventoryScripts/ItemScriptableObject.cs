using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/New item")]
public class ItemScriptableObject : ScriptableObject
{
    public string itemName;
    public string description;
    public ItemTier itemTier;
    public float itemWeight;
    public Sprite itemImage;

    public GameObject itemPrefab;

    public GameObject GenerateItemObj()
    {
        return Instantiate(itemPrefab);
    }
}
