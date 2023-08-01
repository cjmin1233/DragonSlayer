using UnityEngine;

public class ParticleModifier : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystem[] _childParticleSystems;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _childParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void ModifySimulationSpeed(float speed)
    {
        var mainModule = _particleSystem.main;
        mainModule.simulationSpeed = speed;
        foreach (var childPs in _childParticleSystems)
        {
            mainModule = childPs.main;
            mainModule.simulationSpeed = speed;
        }
    }

    public void PlayParticle()
    {
        if (_particleSystem is not null) _particleSystem.Play();
    }
}
