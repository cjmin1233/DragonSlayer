using UnityEngine;

public struct DamageMessage
{
    public GameObject damager;
    public float damage;
    public float stunTime;

    public bool isStiff;
    
    
    //public DamageType damageType;
    //public Vector3 hitPoint;
    //public Vector3 hitNormal;
    public DamageMessage(GameObject damager, float damage, float stunTime, bool isStiff = false)
    {
        this.damager = damager;
        this.damage = damage;
        this.stunTime = stunTime;
        this.isStiff = isStiff;
    }
}
//public enum DamageType
//{
//    Head,
//    Body
//}