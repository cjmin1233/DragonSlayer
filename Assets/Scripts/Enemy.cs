using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    public EnemyData EnemyData { set { enemyData = value; } }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(enemyData.EnemyName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
