using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Pattern")]
public class BossPatternScriptableObject : ScriptableObject
{
    public string patternName;
    public AnimatorOverrideController animatorOv;
    public int priority;
    public float patternCooldown;
    public GameObject patternPrefab;
    public BossPatternAction Init(Transform patternContainer)
    {
        var container = Instantiate(patternPrefab, patternContainer);
        container.name = patternName;

        BossPatternAction patternAction = container.GetComponent<BossPatternAction>();
        patternAction.Init(animatorOv, priority, patternCooldown);

        return patternAction;
    }
}
