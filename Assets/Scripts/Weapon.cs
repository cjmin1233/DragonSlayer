using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private WeaponType weaponType;
    private float weaponDamage;
    private BoxCollider boxCollider;

    private List<GameObject> hitInstanceIdList = new List<GameObject>();
    public void WeaponInit(WeaponScriptableObject weaponScriptableObject)
    {
        weaponType = weaponScriptableObject.type;
        weaponDamage = weaponScriptableObject.damage;
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
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
        print("weapon hit!");
        var livingEntity = other.GetComponent<LIvingEntity>();
        if(livingEntity != null)
        {
            DamageMessage damageMessage;
            damageMessage.damager = gameObject;
            damageMessage.damage = weaponDamage;

            livingEntity.TakeDamage(damageMessage);
        }
    }
}
