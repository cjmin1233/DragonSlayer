using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayer : MonoBehaviour
{
    private void Update()
    {
        if (PlayerHealth.Instance is null) return;
        transform.position = new Vector3(PlayerHealth.Instance.transform.position.x, -5, PlayerHealth.Instance.transform.position.z);
        transform.forward = PlayerHealth.Instance.transform.forward;
    }
}
