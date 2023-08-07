using System.Collections.Generic;
using UnityEngine;

public class ComboAnimation
{
    public AnimatorOverrideController AnimatorOv;
    public float AnimationDamage;
    public bool Loop;
    public bool IsStiff;
    public float StunTime;
    public float NextComboInterval;
    public Vector3 AssaultDirection;
    public AnimationCurve AssaultSpeedCurve;
    public List<ParticleModifier> Effects;
    public int EffectIndex;

    public ComboAnimation(AnimatorOverrideController animatorOv,
        float animationDamage, bool loop, float nextComboInterval,
        Vector3 assaultDirection, AnimationCurve assaultSpeedCurve)
    {
        this.AnimatorOv = animatorOv;
        this.AnimationDamage = animationDamage;
        this.Loop = loop;
        this.IsStiff = false;
        this.NextComboInterval = nextComboInterval;
        this.AssaultDirection = assaultDirection;
        this.AssaultSpeedCurve = assaultSpeedCurve;
        this.Effects = new List<ParticleModifier>();
    }
    public ComboAnimation(AnimatorOverrideController animatorOv,
        float animationDamage, bool loop, bool isStiff, float nextComboInterval,
        Vector3 assaultDirection, AnimationCurve assaultSpeedCurve)
    {
        this.AnimatorOv = animatorOv;
        this.AnimationDamage = animationDamage;
        this.Loop = loop;
        this.StunTime = 0f;
        this.IsStiff = isStiff;
        this.NextComboInterval = nextComboInterval;
        this.AssaultDirection = assaultDirection;
        this.AssaultSpeedCurve = assaultSpeedCurve;
        this.Effects = new List<ParticleModifier>();
    }

    public ComboAnimation(AnimatorOverrideController animatorOv,
        float animationDamage, bool loop, float stunTime, bool isStiff, float nextComboInterval,
        Vector3 assaultDirection, AnimationCurve assaultSpeedCurve)
    {
        this.AnimatorOv = animatorOv;
        this.AnimationDamage = animationDamage;
        this.Loop = loop;
        this.StunTime = stunTime;
        this.IsStiff = isStiff;
        this.NextComboInterval = nextComboInterval;
        this.AssaultDirection = assaultDirection;
        this.AssaultSpeedCurve = assaultSpeedCurve;
        this.Effects = new List<ParticleModifier>();
    }
    
    public void EnableParticle(float speed)
    {
        if (EffectIndex >= Effects.Count) return;
        Effects[EffectIndex].ModifySimulationSpeed(speed);
        Effects[EffectIndex].PlayParticle();
        
        EffectIndex++;
    }
}
