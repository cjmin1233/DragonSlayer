using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyType
{
    Slime,
    TurtleShell,
    Beholder,
    ChestMonster,
    Golem
}
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance {  get; private set; }

    [SerializeField] private GameObject[] enemies;
    private MultiQueue<GameObject> enemyQueue;

    private List<Vector3> mapRecord = new();

    private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        if(!Instance) Instance = this;
        else Destroy(Instance);

        int enumLength = Enum.GetValues(typeof(EnemyType)).Length;
        enemyQueue = new MultiQueue<GameObject>(enumLength);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public Vector3 FindPlayerPlace()
    {
        foreach (var generatedRoomInfo in GameManager.Instance.generatedRooms) mapRecord.Add(generatedRoomInfo.roomPosition);
        Vector3 nearestMap = Vector3.zero;
        float closestDistance = 1000;
        for (int i = 0; i < mapRecord.Count; i++)
        {
            if (Vector3.Distance(player.transform.position, mapRecord[i]) < closestDistance)
            {
                closestDistance = Vector3.Distance(player.transform.position, mapRecord[i]);
                nearestMap = mapRecord[i];
                GameManager.Instance.playerRoomIndex = i;
            }
        }
        return nearestMap;
    }

    public void SelectEnemySpawner()
    {
        Vector3 map = FindPlayerPlace();
        if (GameManager.Instance.generatedRooms[GameManager.Instance.playerRoomIndex].isRoomClear) return;

        for (int i = 0; i < 1; i++)
        {
            EnemySpawn(map, i);   
        }
    }

    private void GrowPool(int index)
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(enemies[index]);
            instanceToAdd.transform.SetParent(transform);
            Add2Pool(index, instanceToAdd);
        }
    }

    public void Add2Pool(int index, GameObject instanceToAdd)
    {
        instanceToAdd.SetActive(false);
        enemyQueue.Enqueue(index, instanceToAdd);
    }

    public GameObject GetFromPool(int index)
    {
        if (enemyQueue.Count(index) <= 0) GrowPool(index);
        return enemyQueue.Dequeue(index);
    }

    private void EnemySpawn(Vector3 map, int index)
    {
        var instance = GetFromPool(index);
        instance.transform.position = new Vector3(map.x + Random.Range(-19, 20), 0f, map.z + Random.Range(-19, 20));
        instance.SetActive(true);
        GameManager.Instance.aliveEnemies++;
    }

    public void MapRecordClear()
    {
        mapRecord.Clear();
    }
}
