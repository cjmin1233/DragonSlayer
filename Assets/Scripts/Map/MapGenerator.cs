//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.AI;
//using Random = UnityEngine.Random;

//public class MapGenerator : MonoBehaviour
//{
//    public static MapGenerator Instance;
//    public Room[] rooms;
//    public GameObject BossRoom;
//    public int numOfRooms = 5; // 생성할 방의 수
//    public int currRooms = 0;
//    public int Stage = 1;
//    GameObject roomPrefab;

//    private void Start()
//    {
//        Instance = this;

//        if(Stage == 1)
//            numOfRooms = 7;
//        if(Stage == 2)
//            numOfRooms = 10;
//        if(Stage == 3)
//            numOfRooms = 15;
//    }


//    public void GenerateMap(Room _room)
//    {
//        if (currRooms < numOfRooms)
//        {
//            Room roomScript = rooms[Random.Range(0, rooms.Length)];
//            if(currRooms == numOfRooms - 1)
//                roomPrefab = BossRoom;
//            else
//                roomPrefab = roomScript.roomPrefebs[Random.Range(0, roomScript.roomPrefebs.Length)];
            
//            Transform spawnPoint = roomScript.spawnPoints[Random.Range(0, roomScript.spawnPoints.Length)];
            
//            Instantiate(roomPrefab, spawnPoint.position, Quaternion.identity);

//            currRooms++;
//        }
//    }


//}
