using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboData
{
    [SerializeField] private PlayerComboType comboType;
    public ComboScriptableObject[] originCombos;
    public List<ComboAnimation> Combos;
    public float nextComboStartTime;
    public int comboCounter;
    public void InitComboData(Transform vfxParent)
    {
        Combos = new List<ComboAnimation>();
        foreach (var originCombo in originCombos)
        {
            var comboAnimation = originCombo.Init(vfxParent);
            Combos.Add(comboAnimation);
        }
    }
}
