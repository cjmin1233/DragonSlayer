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
    public void ClearRoom(int playerRoomIndex)
    {
        MapGenerator.Instance.OpenDoor(playerRoomIndex);
        ChangeImage();
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
    public void ChangeImage()
    {
        // 자식 오브젝트 중 door 태그를 가진 오브젝트 검사
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Door"))
            {
                // Door 스크립트 컴포넌트 가져오기
                Door doorScript = child.GetComponent<Door>();
                if (doorScript != null)
                {
                    Image doorImage = doorScript.GetComponent<Image>();

                    // 인접한 door 오브젝트의 roomType에 따라 색상 변경
                    if (doorScript.connectRoomType == roomType)
                    {
                        doorScript.ChangeDoorColor(Color.green); // 원하는 색상으로 변경
                    }
                }
            }
        }
    }
}
