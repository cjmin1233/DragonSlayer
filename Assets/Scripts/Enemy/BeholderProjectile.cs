using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeholderProjectile : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;

    private void Shoot()
    {
        var instance = Instantiate(projectile, firePoint.position, firePoint.rotation);
    }
}
