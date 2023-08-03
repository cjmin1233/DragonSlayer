using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboData
{
    [SerializeField] private PlayerComboType comboType;
    public ComboScriptableObject[] originCombos;
    public List<ComboAnimation> combos;
    public float nextComboStartTime;
    public int comboCounter = 0;
    public void InitComboData(Transform vfxParent)
    {
        combos = new List<ComboAnimation>();
        foreach (var originCombo in originCombos)
        {
            var comboAnimation = originCombo.Init(vfxParent);
            combos.Add(comboAnimation);
        }
    }
}
