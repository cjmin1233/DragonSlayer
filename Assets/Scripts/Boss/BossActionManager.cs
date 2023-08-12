using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] shelters;
    public static BossActionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this)) Destroy(gameObject);
    }

    public IEnumerator EnableShelter()
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
}
