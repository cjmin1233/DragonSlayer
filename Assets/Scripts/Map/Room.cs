using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RoomType
{
    Normal,
    Golden,
    Trap,
    Jump,
    Boss
}

public class Room : MonoBehaviour
{
    public static Room instance;

    public RoomType roomType;

    public List<GameObject> entrances;

    public bool isClearedRoom = false;
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
    public void Change()
    {
        // 자식 오브젝트 중 door 태그를 가진 오브젝트 검사
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Door"))
            {
                Door doorObject = child.GetComponent<Door>();

                foreach(Transform children in child)
                {
                    Image doorImage = children.GetComponent<Image>();

                    switch (doorObject.connectRoomType)
                    {
                        case RoomType.Normal:
                            doorImage.color = Color.black;
                            break;
                        case RoomType.Golden:
                            doorImage.color = Color.yellow;
                            break;
                        case RoomType.Trap:
                            doorImage.color = Color.gray;
                            break;
                    }
                }
            }
        }
    }
}
