using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 0)]
public class WeaponScriptableObject : ScriptableObject
{
    public WeaponType type;
    public GameObject weaponPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;
    public float damage;

    private GameObject model;
    private Weapon weapon;
    public Weapon Spawn(Transform parent)
    {
        model = Instantiate(weaponPrefab, parent, false);
        // model = Instantiate(weaponPrefab);
        // model.transform.SetParent(parent,false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        weapon = model.GetComponent<Weapon>();
        weapon.WeaponInit(this);
        return weapon;
    }
}
