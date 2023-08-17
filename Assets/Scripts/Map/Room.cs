using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RoomType
{
    Normal,
    Golden,
    Shop,
    Boss,
    Entry
}

public class Room : MonoBehaviour
{
    public static Room instance;

    public RoomType roomType;
    public List<GameObject> entrances;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void Open(Transform parent)
    {
        int childCount = parent.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);

            Open(child);

            if (child.CompareTag("Entrance"))
            {
                Destroy(child.gameObject);
            }

            if (child.CompareTag("Portal"))
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
