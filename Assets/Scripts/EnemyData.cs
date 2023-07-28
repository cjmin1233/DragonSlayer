using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy Data", menuName = "Scriptable Object/Enemy Data", order = int.MaxValue)]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string enemyName;
    public string EnemyName { get { return enemyName; } }

    [SerializeField] private EnemyType enemyType;
    public EnemyType EnemyType { get {  return enemyType; } }

    [SerializeField] private int enemyHp;
    public int EnemyHp { get {  return enemyHp; } }

    [SerializeField] private float enemyMoveSpeed;
    public float EnemyMoveSpeed { get { return enemyMoveSpeed; } }
}
