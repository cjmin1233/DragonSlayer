using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float WeaponDamage
    {
        get => weaponDamage;
    }
    [SerializeField] private Transform hitPoint;
    [SerializeField] private EffectType hitVfxType;
    private WeaponType weaponType;
    private float weaponDamage;
    private float stunTime;
    private bool stiff;
    private BoxCollider boxCollider;

    private List<GameObject> hitList = new List<GameObject>();
    private PlayerCombat _playerCombat;

    private float curDamage;
    public void WeaponInit(WeaponScriptableObject weaponScriptableObject, PlayerCombat playerCombat)
    {
        weaponType = weaponScriptableObject.type;
        weaponDamage = weaponScriptableObject.weaponDamage;
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        _playerCombat = playerCombat;
    }

    public void WeaponSetup(ComboAnimation comboAnimation)
    {
        curDamage = _playerCombat.CalculateDamage();
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
            DamageMessage damageMessage = new DamageMessage(_playerCombat.gameObject, hitPos, curDamage, stunTime, stiff);
            print("weapon damage : " + damageMessage.damage);
            livingEntity.TakeDamage(damageMessage);
            SpawnEffects(damageMessage);
        }
    }

    private void SpawnEffects(DamageMessage damageMessage)
    {
        var hitVfx = EffectManager.Instance.GetFromPool((int)hitVfxType);
        if (hitVfx is not null)
        {
            hitVfx.transform.position = damageMessage.hitPoint;
            hitVfx.SetActive(true);
        }

        var damageText = EffectManager.Instance.GetFromPool((int)EffectType.DamageText);
        if (damageText is not null)
        {
            damageText.transform.position = damageMessage.hitPoint;
            damageText.GetComponent<DamageText>().SetDamageValue(damageMessage.damage);
            damageText.transform.rotation = Quaternion.LookRotation(damageMessage.damager.transform.forward);
            damageText.SetActive(true);
        }
    }
}
