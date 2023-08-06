using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapVector2 : MonoBehaviour
{
    public static MapVector2 instance;

    public List<Vector2Int> mapVector = new();
    public List<Vector2Int> candidateVector = new();

    public Vector2Int startPoint = new(0, 0);
    private int distance = 7; // 방사이의 거리 && 방 크기
    public int Stage = 1;

    public delegate void VectorMapAdded(List<Vector2Int> vector);
    public static event VectorMapAdded OnMapAdded;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int numOfRooms;
        mapVector.Clear();
        candidateVector.Clear();

        // 시작점 제외 뽑기라 각 스테이지당 8, 11, 16 방 구성
        if (Stage == 1)
            numOfRooms = 7;
        else if (Stage == 2)
            numOfRooms = 10;
        else
            numOfRooms = 15;

        mapVector.Add(startPoint);

        for(var i = 0; i < numOfRooms; i++)
        {
            StartFinding(mapVector[i], distance);
            if (i == numOfRooms - 1)
                OnMapAdded(mapVector);
                
        }
    }

    // 시작지점과 distance만큼 떨어진 벡터를 가져올때 맵과 후보 리스트에 없는 값을 가져옴
    public void StartFinding(Vector2Int startVector, int distance)
    {
        List<Vector2Int> temp = new();
        
        temp.Add(new Vector2Int(startVector.x + distance, startVector.y));
        temp.Add(new Vector2Int(startVector.x + -distance, startVector.y));
        temp.Add(new Vector2Int(startVector.x , startVector.y + distance));
        temp.Add(new Vector2Int(startVector.x , startVector.y - distance));

        foreach(var i in temp)
        {
            if (!candidateVector.Contains(i) && !mapVector.Contains(i))
            {
                candidateVector.Add(i);
            }
        }
        Selecting(); 
    }
    // 후보자 리스트에서 랜덤으로 한 벡터를 고르고 맵 리스트에 추가
    public void Selecting()
    {
        var rand = Random.Range(0, candidateVector.Count);
        var randVector = candidateVector[rand];

        bool check = candidateVector.Any(randVector => mapVector.Contains(randVector));

        //중복값이 없으면 mapVector에 추가후 후보자에서 제거함
        if (!check)
        {
            mapVector.Add(randVector);
            candidateVector.Remove(randVector);
        }
        //중복값을 뽑았을 경우 후보자에서 삭제후 다시 뽑음
        else
        {
            candidateVector.Remove(randVector);
            Selecting();
        }

    }
}
