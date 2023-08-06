using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public List<Vector2Int> mapVec2;
    public List<Vector3> mapVec3 = new();
    public int[] EpicRooms = new int[3]; // 점프 골드 트랩 방의 개수를 넣을 에픽방 배열 선언
    private int epicSize = 0;
    private int roomSize = 0;
    public GameObject EntryRoom;
    public GameObject[] NormalRooms;
    public GameObject[] BossRooms;
    public GameObject[] JumpRooms;
    public GameObject[] GoldRooms;
    public GameObject[] TrapRooms;
    public GameObject Room;
  
    // 2차원 좌표를 3차원 좌표로 변경
    public void DimensionTrans(List<Vector2Int> vector2d)
    {
        foreach (Vector2Int v in vector2d)
        {
            mapVec3.Add(new Vector3(v.x, 0, v.y));
        }
        roomSize = mapVec3.Count;
    }
    // 게임 오브젝트가 활성화 되어있으면 MapVector2의 OnMapAdded를 받음
    private void OnEnable()
    {
        MapVector2.OnMapAdded += HandleMapAdded;
    }
    // 받은 좌표를 3차원좌표로 변경후 메서드 삭제
    private void HandleMapAdded(List<Vector2Int> vector)
    {
        DimensionTrans(vector);
        MapVector2.OnMapAdded -= HandleMapAdded;
        RoomGenerator();
    }

    private void RoomGenerator()
    {
        Instantiate(EntryRoom, mapVec3[0], Quaternion.identity);
        EpicRoomCreate();
        NormalRoomCreate();
        NavMeshBake(Room);
    }
    private void EpicRoomCreate()
    {
        //에픽리스트 0: 점프방 1: 황금방 2: 함정방 개수를 스테이지별로 랜덤하게 정하기
        for (var i = 0; i < EpicRooms.Length; i++)
        {
            EpicRooms[i] = Random.Range(0, MapVector2.instance.Stage + 1);
            epicSize += EpicRooms[i];
        }
        // 점프방 개수만큼 방 생성 -> mapVec3[0] 은 Entry방이므로 그 뒤로 생성
        for(var i = 0; i < EpicRooms[0]; i++)
        {
            var rand = Random.Range(0, 4); //에픽방 랜덤 범위
            Room = Instantiate(JumpRooms[rand], mapVec3[i+1], Quaternion.identity);
            NavMeshBake(Room);
        }
        // 황금방 개수만큼 방 생성 -> mapVec3[1 + 점프방 수] 그 뒤로 생성
        for(var i = 0; i < EpicRooms[1]; i++)
        {
            var rand = Random.Range(0, 4); //에픽방 랜덤 범위
            Room = Instantiate(GoldRooms[rand], mapVec3[i + 1 + EpicRooms[0]], Quaternion.identity);
            NavMeshBake(Room);
        }
        // 함정방 개수만큼 방 생성 -> mapVec3[1 + 점프방 + 황금방 수] 그 뒤로 생성
        for (var i = 0; i < EpicRooms[2]; i++)
        {
            var rand = Random.Range(0, 4); //에픽방 랜덤 범위
            Room = Instantiate(TrapRooms[rand], mapVec3[i + 1 + EpicRooms[0] + EpicRooms[1]], Quaternion.identity);
            NavMeshBake(Room);
        }
    }
    private void NormalRoomCreate()
    {
        for(var i = epicSize + 1 ; i < roomSize; i++)
        {
            var randNor = Random.Range(0, NormalRooms.Length);
            Room = Instantiate(NormalRooms[randNor], mapVec3[i], Quaternion.identity);
            NavMeshBake(Room);
        }
    }
    private void NavMeshBake(GameObject room)
    {
        
        if(room.TryGetComponent<MeshCollider>(out var meshCollider))
        {
            
            if(!room.TryGetComponent<NavMeshSurface>(out var navMeshSurface))
                navMeshSurface = room.AddComponent<NavMeshSurface>();

            navMeshSurface.collectObjects = CollectObjects.Children;
            navMeshSurface.BuildNavMesh();
        }
        
    }
}
