using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayCast : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject);
                hit.transform.GetComponentInChildren<DamageTextTest>().TakenDamage();
            }
        }
    }
}
