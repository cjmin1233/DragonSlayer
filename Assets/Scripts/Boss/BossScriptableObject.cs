using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Boss")]
public class BossScriptableObject : ScriptableObject
{
    public float maxHp;
    public float patrolSpeed;
    public float traceSpeed;
    public float flyPatrolSpeed;
    public float flyTraceSpeed;
    public float findTargetRadius;
    public float takeOffSpeed;
    public float restTime;
    
    public int phase;
}
