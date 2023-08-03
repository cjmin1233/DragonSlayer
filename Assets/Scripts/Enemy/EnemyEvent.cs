using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyEvent : MonoBehaviour
{
    public UnityEvent onShoot;

    public void OnShoot() => onShoot.Invoke();
}
