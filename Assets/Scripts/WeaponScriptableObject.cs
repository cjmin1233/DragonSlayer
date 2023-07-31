using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 0)]
public class WeaponScriptableObject : ScriptableObject
{
    public WeaponType type;
    public GameObject weaponPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;

    public void Spawn(Transform parent, MonoBehaviour monoBehaviour)
    {
        this.activeMonoBehaviour = monoBehaviour;

        model = Instantiate(weaponPrefab, parent, false);
        // model = Instantiate(weaponPrefab);
        // model.transform.SetParent(parent,false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);
    }
}
