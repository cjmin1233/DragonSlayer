using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BossPatternAction
{
    public AnimatorOverrideController animatorOv;
    public int priority;
    public float patternCooldown;
    public float patternEnableTime;
    public Transform target;

    public BossPatternAction()
    {
        this.priority = -1;
    }

    public BossPatternAction(AnimatorOverrideController animatorOv, int priority, float coolDown)
    {
        this.animatorOv = animatorOv;
        this.priority = priority;
        this.patternCooldown = coolDown;
    }
}
