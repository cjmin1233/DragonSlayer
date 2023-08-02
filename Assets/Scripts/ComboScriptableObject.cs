using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class ComboScriptableObject : ScriptableObject
{
    public AnimatorOverrideController animatorOv;
    public float damage; 
    public bool loop;
    public float nextComboInterval;
    public Vector3 assaultDirection;
    public AnimationCurve assaultSpeedCurve;
    public GameObject[] vfxPrefabs;
    // public int particleIndex = 0;

    // private List<GameObject> particleInstances = new List<GameObject>();
    public ComboAnimation Init(Transform vfxParent)
    {
        // particleInstances.Clear();
        ComboAnimation comboAnimation = new ComboAnimation();
        comboAnimation.animatorOv = animatorOv;
        comboAnimation.animationDamage = damage;
        comboAnimation.loop = loop;
        comboAnimation.nextComboInterval = nextComboInterval;
        comboAnimation.assaultDirection = assaultDirection;
        comboAnimation.assaultSpeedCurve = assaultSpeedCurve;
        foreach (var vfxPrefab in vfxPrefabs)
        {
            var vfxModifier = Instantiate(vfxPrefab, vfxParent, false).GetComponent<ParticleModifier>();
            if (vfxModifier is not null) comboAnimation.effects.Add(vfxModifier);
            else Debug.LogError("vfx modifier is not valid");
            // comboAnimation.effects.Add(vfx.GetComponent<ParticleModifier>());
        }

        return comboAnimation;
    }
}
