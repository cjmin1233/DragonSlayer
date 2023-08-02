using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface[] surfaces = null;

    private void Start()
    {
        Invoke(nameof(bakeGround), 7f);
    }
    public void bakeGround()
    {
        if(surfaces != null)
        {
            foreach (NavMeshSurface surface in surfaces)
            {
                surface.BuildNavMesh();
            }
        }
    }
}