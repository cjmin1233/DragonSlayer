using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Effect : MonoBehaviour
{
    [SerializeField] private EffectType effectType;
    private ParticleSystem _particleSystem;
    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void OnParticleSystemStopped()
    {
        EffectManager.Instance.Add2Pool((int)effectType, gameObject);
    }
}
