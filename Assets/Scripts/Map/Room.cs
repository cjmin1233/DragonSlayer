using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RoomType
{
    Normal,
    Golden,
    Shop,
    Boss
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
        }
    }
    public void PortalOn()
    {
        foreach(Transform child in transform)
        {
            Door doorPortal = child.GetComponent<Door>();

            if(doorPortal != null)
            {
                switch(doorPortal.connectRoomType)
                {
                    case RoomType.Normal:
                        MapGenerator.Instance.portals[0].SetActive(true);
                        break;
                    case RoomType.Golden:
                        MapGenerator.Instance.portals[1].SetActive(true);
                        break;
                    case RoomType.Shop:
                        MapGenerator.Instance.portals[2].SetActive(true);
                        break;
                    case RoomType.Boss:
                        MapGenerator.Instance.portals[3].SetActive(true);
                        break;
                }
            }
        }
    }
}
