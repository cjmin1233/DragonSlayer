using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour
{
    public static MinimapCameraFollow Instance {  get; private set; }

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    public void FollowMinimap()
    {
        transform.position = EnemySpawner.Instance.FindPlayerPlace();
    }
}
