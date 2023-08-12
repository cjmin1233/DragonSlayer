using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Pattern")]
public class BossPatternScriptableObject : ScriptableObject
{
    public string patternName;
    // public AnimatorOverrideController[] animatorOv;
    public AnimationClip[] animationClips;
    public int priority;
    public float patternCooldown;
    public float fieldOfView;
    public float viewDistance;
    public GameObject patternPrefab;
    public BossPatternAction Init(Transform patternContainer)
    {
        var container = Instantiate(patternPrefab, patternContainer);
        container.name = patternName;

        BossPatternAction patternAction = container.GetComponent<BossPatternAction>();
        patternAction.Init(animationClips, priority, patternCooldown, fieldOfView, viewDistance);

        
        return patternAction;
    }
}
