using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeholderProjectile : Enemy
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform firePoint;

    protected override void Awake()
    {
        base.Awake();

        enemyEvent.onShoot.AddListener(Shoot);
    }

    private void Shoot()
    {
        var instance = Instantiate(projectile, firePoint.position, firePoint.rotation);
        //instance = GetComponent<Projectile>();
    }
}
