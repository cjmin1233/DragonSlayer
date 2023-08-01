using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class AttackSo : ScriptableObject
{
    public AnimatorOverrideController animatorOv;
    public float damage;
    public bool loop;
    public float nextComboInterval;
    public Vector3 assaultDirection;
    public AnimationCurve assaultSpeedCurve;

    public List<GameObject> vfxPrefabs = new List<GameObject>();
    // public List<ParticleModifier> particles = new List<ParticleModifier>();
    public int particleIndex = 0;

    private List<GameObject> particleInstances = new List<GameObject>();
    public void Init(Transform vfxParent)
    {
        particleInstances.Clear();
        foreach (var prefab in vfxPrefabs)
        {
            var vfx = Instantiate(prefab, vfxParent, false);
            // var vfxModifier = vfx.GetComponent<ParticleModifier>();
            // particles.Add(vfxModifier);
            particleInstances.Add(vfx);
        }
    }

    public void EnableParticle(float speed)
    {
        if (particleIndex >= particleInstances.Count) return;
        ParticleModifier particleModifier = particleInstances[particleIndex].GetComponent<ParticleModifier>();
        particleModifier.ModifySimulationSpeed(speed);
        particleModifier.PlayParticle();
        particleIndex++;
    }
}
