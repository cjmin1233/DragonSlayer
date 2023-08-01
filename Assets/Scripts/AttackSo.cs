using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class AttackSo : ScriptableObject
{
    public AnimatorOverrideController animatorOv;
    public float damage;
    public bool loop;
    public float nextComboInterval;
    public Vector3 assaultDirection;
    public AnimationCurve assaultSpeedCurve;
}
