using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal Attack")]
public class ComboScriptableObject : ScriptableObject
{
    public AnimatorOverrideController animatorOv;
    public float damage; 
    public bool loop;
    public float stunTime;
    public bool isStiff;
    public float nextComboInterval;
    public Vector3 assaultDirection;
    public AnimationCurve assaultSpeedCurve;
    public GameObject[] vfxPrefabs;
    // public int particleIndex = 0;

    // private List<GameObject> particleInstances = new List<GameObject>();
    public ComboAnimation Init(Transform vfxParent)
    {
        // particleInstances.Clear();
        ComboAnimation comboAnimation = new ComboAnimation(animatorOv, damage, loop, stunTime, isStiff,
            nextComboInterval, assaultDirection, assaultSpeedCurve);

        foreach (var vfxPrefab in vfxPrefabs)
        {
            var vfxModifier = Instantiate(vfxPrefab, vfxParent, false).GetComponent<ParticleModifier>();
            if (vfxModifier is not null) comboAnimation.Effects.Add(vfxModifier);
            else Debug.LogError("vfx modifier is not valid");
        }

        return comboAnimation;
    }
}
