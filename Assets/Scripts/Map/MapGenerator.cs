using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public GameObject[] doors;
    public List<Vector2Int> mapVec2;
    public List<Vector3> mapVec3 = new();
    public List<Vector3> mapRecord = new();
    public List<Vector3> roomVec3 = new();
    public List<int>  EpicRooms = new(3); 
    private int epicSize = 0;
    private int mapSize = 0;
    private Vector3 bossVector;
    public GameObject EntryRoom;
    public GameObject[] NormalRooms;
    public GameObject[] BossRooms;
    public GameObject[] JumpRooms;
    public GameObject[] GoldRooms;
    public GameObject[] TrapRooms;
    public GameObject Room;
    public List<GameObject> Rooms;
    public List<GameObject> listRooms = new();
    [SerializeField] GameObject minimapTile;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
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
        roomVec3.AddRange(mapVec3);
        mapSize = mapVec3.Count;

    }
    public void OnEnable()
    {
        MapVector2.OnMapAdded += HandleMapAdded;
    }
    private void HandleMapAdded(List<Vector2Int> vector)
    {
        DimensionTrans(vector);
        RoomGenerator();
    }
    private void RoomGenerator()
    {
        DungeonReset();
        FindBoss();
        Rooms.Add(Instantiate(EntryRoom, mapVec3[0], Quaternion.identity));
        mapVec3.Remove(mapVec3[0]);
        EpicRoomCreate();
        NormalRoomCreate();
        FindingDoor();
        NavMeshBake(Rooms);
        FindingRoom();
    }
    private void EpicRoomCreate()
    {
        List<int> tempEpicRooms = new List<int>();
        epicSize = 0;
        
        for (var i = 0; i < 3; i++)
        {
            tempEpicRooms.Add(Random.Range(0, MapVector2.instance.Stage + 1));
            epicSize += tempEpicRooms[i];
        }
        EpicRooms.AddRange(tempEpicRooms);
        tempEpicRooms.Clear();

        for (var i = 0; i < EpicRooms[0]; i++)
        {
            var rand = Random.Range(0, 4); //������ ���� ����
            var randMap = Random.Range(0, mapVec3.Count);
            Rooms.Add(Instantiate(JumpRooms[rand], mapVec3[randMap], Quaternion.identity));
            mapVec3.Remove(mapVec3[randMap]);
        }
        for (var i = 0; i < EpicRooms[1]; i++)
        {
            var rand = Random.Range(0, 4); //Ȳ�ݹ� ���� ����
            var randMap = Random.Range(0, mapVec3.Count);
            Rooms.Add(Instantiate(GoldRooms[rand], mapVec3[randMap], Quaternion.identity));
            mapVec3.Remove(mapVec3[randMap]);
        }
        for (var i = 0; i < EpicRooms[2]; i++)
        {
            var rand = Random.Range(0, 4); //Ʈ���� ���� ����
            var randMap = Random.Range(0, mapVec3.Count);
            Rooms.Add(Instantiate(TrapRooms[rand], mapVec3[randMap], Quaternion.identity));
            mapVec3.Remove(mapVec3[randMap]);
        }
    }
    private void NormalRoomCreate()
    {
        for(var i = 0; i < mapSize - epicSize - 1; i++)
        {
            var randNor = Random.Range(0, NormalRooms.Length);
            if(mapVec3.Count == 0)
            {
                Rooms.Add(Instantiate(BossRooms[0], bossVector, Quaternion.identity));
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
        EpicRooms.Clear();
        bossVector = Vector3.zero;

        GameObject[] roomTag = GameObject.FindGameObjectsWithTag("Rooms");

        foreach(var room in roomTag)
        {
            Destroy(room);
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
        doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (var door in doors)
        {
            door.GetComponent<Door>().ShootRay();
        }        
    }
    public void FindingRoom()
    {
        var distance = 5f;

        for(var i = 0; i < roomVec3.Count; i++)
        {
            for(var j = 0; j < Rooms.Count; j++)
            {
                if (Vector3.Distance(Rooms[j].transform.position, roomVec3[i]) < distance)
                {
                    listRooms.Add(Rooms[j]);
                }
            }
        }
    }
    public void OpenDoor(int playerRoomIndex)
    {
        listRooms[playerRoomIndex].GetComponent<Room>().Open(listRooms[playerRoomIndex].transform);
    }
}
