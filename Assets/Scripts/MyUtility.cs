using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class MyUtility
{
    public static float AngleBetweenVectors(Vector3 from, Vector3 to)
    {
        from.Normalize();
        to.Normalize();

        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f)) * Mathf.Rad2Deg;
        return angle;
    }
    public static Vector3 GetRandomPointOnNavmesh(Vector3 origin, float radius = 1f, int areaMask = NavMesh.AllAreas)
    {
        Vector3 randomPoint = Vector3.zero;
        if (NavMesh.SamplePosition(origin + Random.insideUnitSphere * radius, out NavMeshHit hit, radius, areaMask))
        {
            randomPoint = hit.position;
        }
        randomPoint.y = 0f;
        return randomPoint;
    }

    public static Vector3 GetRandomPointBet2Circles(Vector3 origin, float minRadius, float maxRadius)
    {
        Vector3 randomPoint = Random.onUnitSphere;
        randomPoint.y = 0f;
        randomPoint.Normalize();
        randomPoint *= Random.Range(minRadius, maxRadius);
        return origin + randomPoint;
    }
}
