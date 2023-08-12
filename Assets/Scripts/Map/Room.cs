using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Normal,
    Golden=2,
    Trap=3,
    Jump=4,
    Boss=5
}

public class Room : MonoBehaviour
{
    public static Room instance;

    public RoomType roomType;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public void ClearRoom()
    {
        Debug.Log("cleared");

        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            ClearRoom();

            if (child.CompareTag("Entrance"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
