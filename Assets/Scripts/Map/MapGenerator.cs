using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public List<GameObject> doors = new();
    public List<Vector3> mapVec3 = new();
    public List<Vector3> mapRecord = new();
    public int epicSize = 0;
    private int mapSize = 0;
    private Vector3 bossVector;
    public GameObject EntryRoom;
    public GameObject[] NormalRooms;
    public GameObject BossRoom;
    public GameObject ShopRoom;
    public GameObject GoldRoom;
    public List<GameObject> portals = new();
    //public GameObject Room;
    public List<GameObject> Rooms;
    public List<GameObject> listRooms = new();

    private void Awake()
    {
        if(!Instance) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }
    private void Start()
    {
        foreach(var portal in portals)
        {
            portal.SetActive(false);
        }
    }

    private void HandleMapAdded(List<Vector2Int> vector)
    {
        DimensionTrans(vector);
        RoomGenerator();
    }

    public void DimensionTrans(List<Vector2Int> vector2d)
    {
        foreach (Vector2Int v in vector2d)
        {
            Vector3 vectorTemp = new Vector3(v.x, 0f, v.y);
            mapVec3.Add(vectorTemp);
            GeneratedRoomInfo generatedRoomInfo = new GeneratedRoomInfo(vectorTemp);
            GameManager.Instance.generatedRooms.Add(generatedRoomInfo);
        }
        mapSize = mapVec3.Count;
    }
    public void OnEnable()
    {
        MapVector2.OnMapAdded += HandleMapAdded;
    }

    private void RoomGenerator()
    {
        DungeonReset();
        FindBoss();
        Rooms.Add(Instantiate(EntryRoom, mapVec3[0], Quaternion.identity));
        mapVec3.Remove(mapVec3[0]);
        EpicRoomCreate();
        NormalRoomCreate();
        NavMeshBake(Rooms);
        FindingRoom();
        FindingDoor();
        foreach(var listRoom in listRooms)
        {
            var a = listRoom.GetComponent<Room>().roomType;
            if (a != RoomType.Normal)
                Room.instance.Open(listRoom.transform);
        }
        
    }
    private void EpicRoomCreate()
    {
        for (var i = 0; i < 1; i++)
        {
            var rand = Random.Range(0,1); //������ ���� ����
            var randMap = Random.Range(0, mapVec3.Count);
            if(rand < MapVector2.instance.Stage)
            {
                Rooms.Add(Instantiate(ShopRoom, mapVec3[randMap], Quaternion.identity));
                epicSize++;
                mapVec3.Remove(mapVec3[randMap]);
            }
        }
        for (var i = 0; i < 1; i++)
        {
            var rand = Random.Range(0, 1); //Ȳ�ݹ� ���� ����
            var randMap = Random.Range(0, mapVec3.Count);
            if (rand < MapVector2.instance.Stage)
            {
                Rooms.Add(Instantiate(GoldRoom, mapVec3[randMap], Quaternion.identity));
                epicSize++;
                mapVec3.Remove(mapVec3[randMap]);
            }
        }

    }
    private void NormalRoomCreate()
    {
        for(var i = 0; i < mapSize - epicSize - 1; i++)
        {
            var randNor = Random.Range(0, NormalRooms.Length);
            if(mapVec3.Count == 0)
            {
                Rooms.Add(Instantiate(BossRoom, bossVector, Quaternion.identity));
                mapVec3.Clear();
                return;
            }
            var randMap = Random.Range(0, mapVec3.Count);
            Rooms.Add(Instantiate(NormalRooms[randNor], mapVec3[randMap], Quaternion.identity));
            mapVec3.Remove(mapVec3[randMap]);
        }

      
    }
    private void NavMeshBake(List<GameObject> bakeroom)
    {
        foreach (var room in bakeroom)
        {
            Transform[] children = room.GetComponentsInChildren<Transform>();
            
            foreach (var child in children)
            {
                MeshCollider meshCollider = child.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    NavMeshSurface navMeshSurface = child.GetComponent<NavMeshSurface>();
                    if (navMeshSurface != null)
                    {
                        NavMeshData newNavMeshData = new NavMeshData();
                        navMeshSurface.navMeshData = newNavMeshData;
                        navMeshSurface.BuildNavMesh();
                    }
                }
            }
        }       
    }
    public void DungeonReset()
    {
        NavMesh.RemoveAllNavMeshData();

        Rooms.Clear();
        listRooms.Clear();
        foreach (var door in doors) door.SetActive(false);
        doors.Clear();

        bossVector = Vector3.zero;

        GameObject[] roomTag = GameObject.FindGameObjectsWithTag("Rooms");

        foreach(var room in roomTag)
        {
            Destroy(room);
        }
    }
    public void ResetDoors()
    {
        foreach(var door in doors)
        {
            door.SetActive(false);
        }
    }
    public void FindBoss()
    {
        var minDist = 0f;
        foreach(Vector3 distance in mapVec3)
        {
            if(Vector3.Distance(mapVec3[0], distance) > minDist)
            {
                bossVector = distance;
                minDist = Vector3.Distance(mapVec3[0], distance);
            }
        }

        mapVec3.Remove(bossVector);
    }
    public void FindingDoor()
    {
        foreach (var door in GameObject.FindGameObjectsWithTag("Door")) doors.Add(door);

        foreach (GameObject door in doors)
        {
            GameObject closestDoor = FindClosestDoor(door);

            if (closestDoor != null)
            {
                Door closestDoorObject = closestDoor.GetComponent<Door>();
                Door doorObject = door.GetComponent<Door>();

                if (closestDoorObject != null && doorObject != null)
                {
                    doorObject.connectRoomType = closestDoorObject.currentRoomType;
                }
            }
            closestDoor = null;
        }
        
        foreach (var door in doors)
        {
            door.GetComponent<Door>().ShootRay();
            door.GetComponent<Door>().ChangeImage();
        }
    }
    private GameObject FindClosestDoor(GameObject currentDoor)
    {
        GameObject closestDoor = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject door in doors)
        {
            if (door != currentDoor)
            {
                float distance = Vector3.Distance(currentDoor.transform.position, door.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestDoor = door;
                }
            }
        }

        return closestDoor;
    }
    public void FindingRoom()
    {
        var distance = 5f;

        for(var i = 0; i < GameManager.Instance.generatedRooms.Count; i++)
        {
            for(var j = 0; j < Rooms.Count; j++)
            {
                if (Vector3.Distance(Rooms[j].transform.position, GameManager.Instance.generatedRooms[i].roomPosition) < distance)
                {
                    listRooms.Add(Rooms[j]);
                }
            }
        }
    }
    public void ClearRoom(int playerRoomIndex)
    {
        listRooms[playerRoomIndex].GetComponent<Room>().Open(listRooms[playerRoomIndex].transform);
        //listRooms[playerRoomIndex].GetComponent<Room>().PortalOn();
    }
}
