using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossPatternData
{
    [SerializeField] private BossPatternType patternType;
    public BossPatternScriptableObject[] originPatterns;
    public List<BossPatternAction> Patterns;
    public int curPatternIndex;

    public void InitPatternData()
    {
        Patterns = new List<BossPatternAction>();
        foreach (var originPattern in originPatterns)
        {
            var bossPatternAction = originPattern.Init();
            Patterns.Add(bossPatternAction);
        }

        Debug.Log(Patterns.Count);
    }

    public BossPatternAction SelectPatternAction()
    {
        BossPatternAction patternAction = new BossPatternAction();
        
        foreach (var pattern in Patterns)
        {
            if (Time.time >= pattern.patternEnableTime && pattern.priority > patternAction.priority)
            {
                patternAction = pattern;
            }
        }

        if (patternAction.priority > 0) return patternAction;
        else return null;
    }
}
