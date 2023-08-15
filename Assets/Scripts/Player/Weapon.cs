using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private GameObject damager;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private EffectType hitVfxType;
    private WeaponType weaponType;
    private float weaponDamage;
    private float stunTime;
    private bool stiff;
    private BoxCollider boxCollider;

    private List<GameObject> hitList = new List<GameObject>();
    public void WeaponInit(WeaponScriptableObject weaponScriptableObject, PlayerCombat playerCombat)
    {
        weaponType = weaponScriptableObject.type;
        weaponDamage = weaponScriptableObject.damage;
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        damager = playerCombat.gameObject;
    }

    public void WeaponSetup(ComboAnimation comboAnimation)
    {
        weaponDamage = comboAnimation.AnimationDamage;
        stunTime = comboAnimation.StunTime;
        stiff = comboAnimation.IsStiff;
    }

    public void EnableWeapon()
    {
        hitList.Clear();
        boxCollider.enabled = true;
    }

    public void DisableWeapon()
    {
        boxCollider.enabled = false;
    }
    private void OnTriggerStay(Collider other)
    {
        var instance = other.gameObject;
        if (hitList.Contains(instance)) return;
        hitList.Add(instance);

        // 데미지 처리
        var livingEntity = other.GetComponent<LivingEntity>();
        if(livingEntity != null)
        {
            Vector3 hitPos = other.ClosestPointOnBounds(this.hitPoint.position);
            DamageMessage damageMessage = new DamageMessage(damager, hitPos, weaponDamage, stunTime, stiff);

            livingEntity.TakeDamage(damageMessage);
            var hitVfx = EffectManager.Instance.GetFromPool((int)hitVfxType);
            hitVfx.transform.position = hitPos;
            hitVfx.SetActive(true);
        }
        // var collisionPointOnBound = other.ClosestPointOnBounds(hitPoint.position);
        // print("collision point : " + collisionPointOnBound);
    }
}
