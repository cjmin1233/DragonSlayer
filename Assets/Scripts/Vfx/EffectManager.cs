using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    SwordHit,
}
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [SerializeField] private GameObject[] effects;
    private MultiQueue<GameObject> effectQueue;
    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        int enumLength = Enum.GetValues(typeof(EffectType)).Length;
        effectQueue = new MultiQueue<GameObject>(enumLength);

        for (int i = 0; i < enumLength; i++)
        {
            GrowPool(i);
        }
    }

    private void GrowPool(int index)
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(effects[index], transform, true);
            Add2Pool(index, instanceToAdd);
        }
    }

    public void Add2Pool(int index, GameObject instanceToAdd)
    {
        instanceToAdd.SetActive(false);
        effectQueue.Enqueue(index, instanceToAdd);
    }

    public GameObject GetFromPool(int index)
    {
        if (effectQueue.Count(index) <= 0) GrowPool(index);
        return effectQueue.Dequeue(index);
    }
}
