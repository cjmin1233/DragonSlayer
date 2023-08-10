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
    public static Door instance;
    public LayerMask whatIsTarget;
    public Image doorImage;
    private Color basic;
    //public bool isCleared = true; 게임매니저에 있는 isCleared 받아옴
    public DoorType doorType;
    public RoomType connectRoomType;
    private const float distance = 8.5f;
    private Vector3 doorDirection;

    private const float RayDistance = 10f;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        doorImage = GetComponent<Image>();
        //basic = doorImage.material.color;
        if(doorType == DoorType.Right)
            doorDirection = Vector3.right;
        else if(doorType == DoorType.Left)
            doorDirection = Vector3.left;
        else if(doorType == DoorType.Up)
            doorDirection = Vector3.forward;
        else
            doorDirection = Vector3.back;
    }

    private void Update()
    {
        Debug.DrawRay(Vector3.up + transform.position + doorDirection, doorDirection * RayDistance, Color.red);
    }
    public void MoveToRoom(Vector3 direction)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        var tranPosition = playerTransform.position + direction + (Vector3.up * 0.5f);
        playerTransform.position = tranPosition; 
    }
    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player") && GameManager.Instance.IsRoomCleared()) // && isCleared
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
            EnemySpawner.Instance.SelectEnemySpawner();
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
    public void ShootRay()
    {
        Collider[] hitInfo = new Collider[10];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 5f, hitInfo, whatIsTarget, QueryTriggerInteraction.Ignore);

        if (numColliders <= 1)
        {
            gameObject.SetActive(false);
        }
    }
}
