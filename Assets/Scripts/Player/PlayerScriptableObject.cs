using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Player/PlayerScriptableObject", order = 0)]
public class PlayerScriptableObject : ScriptableObject
{
    public float health;
    public float moveSpeed;
    public float sprintSpeed;
    public float rollSpeed;
    public float attackSpeed;
    public float defence;
    public float guardDuration;
    public float guardTimeOut;
}
