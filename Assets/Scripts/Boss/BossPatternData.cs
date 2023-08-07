using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossPatternData
{
    [SerializeField] private BossPatternType patternType;
    public BossPatternScriptableObject[] originPatterns;
    public List<BossPatternAction> patterns;
    public int curPatternIndex;

    public void InitPatternData(Transform patternContainer)
    {
        patterns = new List<BossPatternAction>();
        foreach (var originPattern in originPatterns)
        {
            var bossPatternAction = originPattern.Init(patternContainer);
            if (bossPatternAction is null)
            {
                Debug.LogError("Boss pattern action is null");
                continue;
            }
            else patterns.Add(bossPatternAction);
        }

        Debug.Log(patterns.Count);
    }

    public BossPatternAction SelectPatternAction()
    {
        int highestPriority = -1;
        int selectedIndex = -1;
        for (int i = 0; i < patterns.Count; i++)
        {
            if (Time.time >= patterns[i].patternEnableTime && patterns[i].priority > highestPriority)
            {
                selectedIndex = i;
                highestPriority = patterns[selectedIndex].priority;
            }
        }

        if (selectedIndex >= 0) return patterns[selectedIndex];
        else return null;
    }
}
