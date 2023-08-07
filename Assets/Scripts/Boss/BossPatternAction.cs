using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

public class BossPatternAction : MonoBehaviour
{
    public AnimatorOverrideController animatorOv;
    public int priority;
    public float patternCooldown;
    public float patternEnableTime;
    public Transform target;

    public void Init(AnimatorOverrideController animatorOvParam, int priorityParam, float patternCooldownParam)
    {
        this.animatorOv = animatorOvParam;
        this.priority = priorityParam;
        this.patternCooldown = patternCooldownParam;
    }

    protected virtual IEnumerator PatternRoutine()
    {
        yield break;
    }
}
