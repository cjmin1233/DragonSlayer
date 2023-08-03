using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAnimation
{
    public AnimatorOverrideController animatorOv;
    public float animationDamage;
    public bool loop;
    public float nextComboInterval;
    public Vector3 assaultDirection;
    public AnimationCurve assaultSpeedCurve;
    public List<ParticleModifier> effects = new List<ParticleModifier>();
    public int effectIndex;
    public void EnableParticle(float speed)
    {
        if (effectIndex >= effects.Count) return;
        effects[effectIndex].ModifySimulationSpeed(speed);
        effects[effectIndex].PlayParticle();
        
        effectIndex++;
    }
}
