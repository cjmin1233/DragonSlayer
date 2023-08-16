using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayer : MonoBehaviour
{
    private void Update()
    {
        transform.position = new Vector3(PlayerHealth.Instance.transform.position.x, -10, PlayerHealth.Instance.transform.position.z);
        transform.forward = PlayerHealth.Instance.transform.forward;
    }
}
