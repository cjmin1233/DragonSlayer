using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform hitPoint;
    private WeaponType weaponType;
    private float weaponDamage;
    private float stunTime;
    private bool stiff;
    private BoxCollider boxCollider;

    private List<GameObject> hitInstanceIdList = new List<GameObject>();
    public void WeaponInit(WeaponScriptableObject weaponScriptableObject)
    {
        weaponType = weaponScriptableObject.type;
        weaponDamage = weaponScriptableObject.damage;
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    public void WeaponSetup(ComboAnimation comboAnimation)
    {
        weaponDamage = comboAnimation.AnimationDamage;
        stunTime = comboAnimation.StunTime;
        stiff = comboAnimation.IsStiff;
    }

    public void EnableWeapon()
    {
        hitInstanceIdList.Clear();
        boxCollider.enabled = true;
    }

    public void DisableWeapon()
    {
        boxCollider.enabled = false;
    }
    private void OnTriggerStay(Collider other)
    {
        var instance = other.gameObject;
        if (hitInstanceIdList.Contains(instance)) return;
        hitInstanceIdList.Add(instance);

        // 데미지 처리
        var livingEntity = other.GetComponent<LIvingEntity>();
        if(livingEntity != null)
        {
            DamageMessage damageMessage =
                new DamageMessage(GetComponentInParent<PlayerCombat>().gameObject, weaponDamage, stunTime, stiff);

            livingEntity.TakeDamage(damageMessage);
        }
        // var collisionPointOnBound = other.ClosestPointOnBounds(hitPoint.position);
        // print("collision point : " + collisionPointOnBound);
    }
}
