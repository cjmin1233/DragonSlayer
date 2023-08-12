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
    public RoomType roomType;

    private void Start()
    {
        
    }
    public void ClearRoom()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Entrance"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
