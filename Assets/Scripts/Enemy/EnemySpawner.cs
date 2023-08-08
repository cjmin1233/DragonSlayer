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
    [SerializeField] private GameObject[] enemies;
    private MultiQueue<GameObject> enemyQueue;

    private List<Vector3> mapRecord = new();

    private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        int enumLength = Enum.GetValues(typeof(EnemyType)).Length;
        enemyQueue = new MultiQueue<GameObject>(enumLength);

        mapRecord = GameObject.Find("MapGenerator").GetComponent<MapGenerator>().mapRecord;
        player = GameObject.FindGameObjectWithTag("Player");

        SelectEnemySpawner();
    }

    public Vector3 FindPlayerPlace()
    {
        Vector3 nearestMap = Vector3.zero;
        float closestDistance = 1000;
        foreach (Vector3 map in mapRecord)
        {
            if (Vector3.Distance(player.transform.position, map) < closestDistance)
            {
                closestDistance = Vector3.Distance(player.transform.position, map);
                nearestMap = map;
            }
        }
        return nearestMap;
    }

    public void SelectEnemySpawner()
    {
        Vector3 map = FindPlayerPlace();
        
        for (int i = 0; i < 5; i++) EnemySpawn(map, i);
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
        instance.transform.position = new Vector3(map.x + Random.Range(-9, 10), 0f, map.z + Random.Range(-9, 10));
        instance.SetActive(true);
    }
}
