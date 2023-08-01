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
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Player")
        {
            Destroy(gameObject);
        }
    }
}
