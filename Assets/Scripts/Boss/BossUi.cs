using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUi : MonoBehaviour
{
    private StandardSlider healthBar;
    private Boss _boss;

    private void Awake()
    {
        healthBar = GetComponentInChildren<StandardSlider>();
        _boss = GetComponentInParent<Boss>();
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.UpdateValue(_boss.CurHP, _boss.MaxHP);
    }
}
