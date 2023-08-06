using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Pattern")]
public class BossPatternScriptableObject : ScriptableObject
{
    public AnimatorOverrideController animatorOv;
    public int priority;
    public float patternCooldown;
    public BossPatternAction Init()
    {
        BossPatternAction patternAction = new BossPatternAction(animatorOv, priority, patternCooldown);

        return patternAction;
    }
}
