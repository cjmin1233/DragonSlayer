using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public enum DoorType
{
    Up,
    Down,
    Right,
    Left
}

public class Door : MonoBehaviour
{
    public Image doorImage;
    private Color basic;
    //public bool isCleared = true; 게임매니저에 있는 isCleared 받아옴
    public DoorType doorType;
    public RoomType connectRoomType;
    private const float distance = 8.5f;

    RaycastHit[] hits;
    private const float LayDistance = 8.5f;

    private void Start()
    {
        basic = doorImage.material.color;
    }
    public void MoveToRoom(Vector3 direction)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        var tranPosition = playerTransform.position + direction + (Vector3.up * 0.5f);
        playerTransform.position = tranPosition; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) // && isCleared
        {
            Vector3 normalVector;

            switch (doorType)
            {
                case DoorType.Up:
                    normalVector = new Vector3(0, 0, distance);
                    MoveToRoom(normalVector);
                    Debug.Log("up");
                    break;
                case DoorType.Down:
                    normalVector = new Vector3(0, 0, -distance);
                    MoveToRoom(normalVector);
                    Debug.Log("down");
                    break;
                case DoorType.Right:
                    normalVector = new Vector3(distance, 0, 0);
                    MoveToRoom(normalVector);
                    Debug.Log("right");
                    break;
                case DoorType.Left:
                    normalVector = new Vector3(-distance, 0, 0);
                    MoveToRoom(normalVector);
                    Debug.Log("left");
                    break;
                default:
                    break;
            }
        }
    }
    public void ChangeRoomImage(RoomType rt)
    {
        switch (rt)
        {
            case RoomType.Normal:
                doorImage.material.color = basic;
                break;
            case RoomType.Golden:
                doorImage.material.color = Color.yellow;
                break;
            case RoomType.Trap:
                doorImage.material.color = Color.gray;
                break;
            case RoomType.Jump:
                doorImage.material.color = Color.gray;
                break;
            case RoomType.Boss:
                doorImage.material.color = Color.red;
                break;
        }
    }
    public void DoorGenerate(DoorType dt)
    {
        switch (dt)
        {
            case DoorType.Up:
                break;
            case DoorType.Down:
                break;
            case DoorType.Right:
                break;
            case DoorType.Left:
                break; 
        }
    }
}
