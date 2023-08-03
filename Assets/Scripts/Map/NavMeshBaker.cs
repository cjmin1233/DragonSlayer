using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface[] surfaces = null;

    private void Start()
    {
        
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