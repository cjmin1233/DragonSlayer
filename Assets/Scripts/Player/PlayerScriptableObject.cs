using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Player/PlayerScriptableObject", order = 0)]
public class PlayerScriptableObject : ScriptableObject
{
    public float health;
    public float vitality;
    public float vitalityRestoreRate;
    public float moveSpeed;
    public float sprintSpeed;
    public float rollSpeed;
    public float rollVitality;
    public float playerPower;
    public float attackSpeed;
    public float defence;
    public float guardDuration;
    public float guardTimeOut;
    public float parryingInvincibleDuration;
    public float parryingAngle;
}
