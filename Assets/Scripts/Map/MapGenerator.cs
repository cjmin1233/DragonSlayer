using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<Vector2Int> mapVec2;
    public List<Vector3> mapVec3 = new();

    private void Start()
    {

    }

    // 2차원 좌표를 3차원 좌표로 변경
    public void DimensionTrans(List<Vector2Int> vector2d)
    {
        foreach (Vector2Int v in vector2d)
        {
            mapVec3.Add(new Vector3(v.x, 0, v.y));
        }
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
    }
}
