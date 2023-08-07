using System;
using System.Collections;
using System.Collections.Generic;
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
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject[] enemies;
    private MultiQueue<GameObject> enemyQueue;

    // Start is called before the first frame update
    void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(this);

        int enumLength = Enum.GetValues(typeof(EnemyType)).Length;
        enemyQueue = new MultiQueue<GameObject>(enumLength);

        StartCoroutine(EnemySpawn());
    }

    private IEnumerator EnemySpawn()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                EnemySpawn(i);
            }
            yield return new WaitForSeconds(10f);
        }
        
    }

    private void GrowPool(int index)
    {
        for(int i = 0; i < 10; i++)
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
        if(enemyQueue.Count(index) <= 0) GrowPool(index);
        return enemyQueue.Dequeue(index);
    }

    private void EnemySpawn(int index)
    {
        var instance = GetFromPool(index);
        instance.transform.position = new Vector3(Random.Range(-9, 10), 0f, Random.Range(-9, 10));
        instance.SetActive(true);
    }
}
