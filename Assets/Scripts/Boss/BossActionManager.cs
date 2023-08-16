using System.Collections;
using UnityEngine;

public class BossActionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] shelters;
    [SerializeField] private DamagableGround hellFireField;
    public static BossActionManager Instance { get; private set; }

    private Coroutine enableShelterRoutine;
    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this)) Destroy(gameObject);
    }

    public void EnableShelters()
    {
        if (enableShelterRoutine is not null) StopCoroutine(enableShelterRoutine);
        enableShelterRoutine = StartCoroutine(EnableShelterRoutine());
    }
    public IEnumerator EnableShelterRoutine()
    {
        if (shelters.Length <= 0) yield break;
        foreach (var shelter in shelters)
        {
            shelter.SetActive(true);
        }

        yield return new WaitForSeconds(3f);
        foreach (var shelter in shelters)
        {
            shelter.SetActive(false);
        }

    }

    public void EnableHellFireField()
    {
        hellFireField.gameObject.SetActive(false);
        hellFireField.gameObject.SetActive(true);
    }
}
