using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
public enum DoorType
{
    Up,
    Down,
    Right,
    Left
}

public class Room : MonoBehaviour
{
    public GameObject door;
    public bool isCleared = true;
    public DoorType doorType;
    public float distance = 30f;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void ClearRoom()
    {
        Destroy(door);
    }

    public void MoveToRoom(Vector3 direction)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        var tranPosition = playerTransform.position + (direction * distance) + Vector3.up * 3f;
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
                    normalVector = new Vector3(0, 0, 3f);
                    MoveToRoom(normalVector);
                    Debug.Log("up");
                    break;
                case DoorType.Down:
                    normalVector = new Vector3(0, 0, -3f);
                    MoveToRoom(normalVector);
                    Debug.Log("down");
                    break;
                case DoorType.Right:
                    normalVector = new Vector3(3, 0, 0);
                    MoveToRoom(normalVector);
                    Debug.Log("right");
                    break;
                case DoorType.Left:
                    normalVector = new Vector3(-3f, 0, 0);
                    MoveToRoom(normalVector);
                    Debug.Log("left");
                    break;
                default:
                    break;
            }
        }
    }
}
