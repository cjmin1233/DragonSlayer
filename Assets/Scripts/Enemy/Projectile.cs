using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //[HideInInspector] public Enemy enemy;

    private Rigidbody rb;
    
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 5f;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            Debug.Log("플레이어 명중");
            Destroy(gameObject);
        }
    }
}
