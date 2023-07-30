using System;
using System.Collections.Generic;

[Serializable]
public class ComboData
{
    public PlayerCombat.PlayerComboType comboType;
    public List<AttackSo> Combos = new List<AttackSo>();
    public float LastComboEnd;
    public int ComboCounter;

    public void Init(List<AttackSo> comboList)
    {
        foreach (var combo in comboList)
        {
            Combos.Add(combo);
        }
    }
}
