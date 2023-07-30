using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class AttackSo : ScriptableObject
{
    public AnimatorOverrideController animatorOv;
    public float damage;
    public float normalizedExitTime;
    public float normalizedComboTime;
    public bool loop;
    public float nextComboInterval;
}
