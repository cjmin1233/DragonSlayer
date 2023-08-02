using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "MyData/New item")]
public class MyItem : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite itemImage;

    public GameObject obj;


    public GameObject GenerateItemObj()
    {
        return Instantiate(obj);
    }
}
