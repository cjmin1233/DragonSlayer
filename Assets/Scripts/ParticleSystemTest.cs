using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemTest : MonoBehaviour
{
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        float simulationSpeed = _particleSystem.main.simulationSpeed;

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var _particle in particles)
        {
            ParticleSystem.MainModule mainModule = _particle.main;
            mainModule.simulationSpeed = simulationSpeed;
        }
    }
}
