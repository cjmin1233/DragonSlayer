using System;
using System.Collections.Generic;

[Serializable]
public class ComboData
{
    public PlayerCombat.PlayerComboType comboType;
    public List<AttackSo> combos = new List<AttackSo>();
    public float nextComboStartTime;
    public int comboCounter = 0;
}
