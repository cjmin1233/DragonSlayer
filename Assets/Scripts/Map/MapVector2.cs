using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapVector2 : MonoBehaviour
{
    public static MapVector2 Instance { get; private set; }

    public List<Vector2Int> mapVector = new();
    public List<Vector2Int> candidateVector = new();

    public Vector2Int startPoint = new(0, 0);
    private const int distance = 42; // ������� �Ÿ� && �� ũ��
    public int Stage = 1;

    public delegate void VectorMapAdded(List<Vector2Int> vector);
    public static event VectorMapAdded OnMapAdded;

    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        //GenerateDungeon();  
    }

    public void GenerateDungeon()
    {
        int numOfRooms;
        mapVector.Clear();
        candidateVector.Clear();

        // ������ ���� �̱�� �� ���������� 8, 11, 16 �� ����
        if (Stage == 1)
            numOfRooms = 7;
        else if (Stage == 2)
            numOfRooms = 10;
        else
            numOfRooms = 15;


        mapVector.Add(startPoint);

        for (var i = 0; i < numOfRooms; i++)
        {
            StartFinding(mapVector[i], distance);
            if (i == numOfRooms - 1)
                OnMapAdded(mapVector);
        }
    }
    // ���������� distance��ŭ ������ ���͸� �����ö� �ʰ� �ĺ� ����Ʈ�� ���� ���� ������
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
    // �ĺ��� ����Ʈ���� �������� �� ���͸� ������ �� ����Ʈ�� �߰�
    public void Selecting()
    {
        var rand = Random.Range(0, candidateVector.Count);
        var randVector = candidateVector[rand];

        bool check = candidateVector.Any(randVector => mapVector.Contains(randVector));

        //�ߺ����� ������ mapVector�� �߰��� �ĺ��ڿ��� ������
        if (!check)
        {
            mapVector.Add(randVector);
            candidateVector.Remove(randVector);
        }
        //�ߺ����� �̾��� ��� �ĺ��ڿ��� ������ �ٽ� ����
        else
        {
            candidateVector.Remove(randVector);
            Selecting();
        }

    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
