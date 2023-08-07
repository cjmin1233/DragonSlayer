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

    public List<Vector2Int> mapVec2;
    public List<Vector3> mapVec3 = new();
    public List<int>  EpicRooms = new(3); // 점프 골드 트랩 방의 개수를 넣을 에픽방 배열 선언
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

    private void Awake()
    {
        Instance = this;
    }
    // 2차원 좌표를 3차원 좌표로 변경
    public void DimensionTrans(List<Vector2Int> vector2d)
    {
        foreach (Vector2Int v in vector2d)
        {
            mapVec3.Add(new Vector3(v.x, 0, v.y));
        }

        mapSize = mapVec3.Count;

    }
    // 게임 오브젝트가 활성화 되어있으면 MapVector2의 OnMapAdded를 받음
    public void OnEnable()
    {
        MapVector2.OnMapAdded += HandleMapAdded;
    }
    // 받은 좌표를 3차원좌표로 변경 후 맵 생성
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
        NavMeshBake(Rooms);
    }
    private void EpicRoomCreate()
    {
        List<int> tempEpicRooms = new List<int>();
        epicSize = 0;
        //에픽리스트 0: 점프방 1: 황금방 2: 함정방 개수를 스테이지별로 랜덤하게 정하기
        for (var i = 0; i < 3; i++)
        {
            tempEpicRooms.Add(Random.Range(0, MapVector2.instance.Stage + 1));
            epicSize += tempEpicRooms[i];
        }
        EpicRooms.AddRange(tempEpicRooms);
        tempEpicRooms.Clear();

        for (var i = 0; i < EpicRooms[0]; i++)
        {
            var rand = Random.Range(0, 4); //점프방 랜덤 범위
            var randMap = Random.Range(0, mapVec3.Count);
            Rooms.Add(Instantiate(JumpRooms[rand], mapVec3[randMap], Quaternion.identity));
            mapVec3.Remove(mapVec3[randMap]);
        }
        for (var i = 0; i < EpicRooms[1]; i++)
        {
            var rand = Random.Range(0, 4); //황금방 랜덤 범위
            var randMap = Random.Range(0, mapVec3.Count);
            Rooms.Add(Instantiate(GoldRooms[rand], mapVec3[randMap], Quaternion.identity));
            mapVec3.Remove(mapVec3[randMap]);
        }
        for (var i = 0; i < EpicRooms[2]; i++)
        {
            var rand = Random.Range(0, 4); //트랩방 랜덤 범위
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
}
