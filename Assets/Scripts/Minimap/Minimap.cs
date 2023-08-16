using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public static Minimap Instance { get; private set; }

    private Queue<GameObject> minimapTiles = new();
    [SerializeField] GameObject minimapTile;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(Instance);
    }

    public void MinimapCreate()
    {
        foreach (GeneratedRoomInfo room in GameManager.Instance.generatedRooms)
        {
            var instance = Instantiate(minimapTile, room.roomPosition + new Vector3(0, -10, 0), Quaternion.identity, transform);
            minimapTiles.Enqueue(instance);
        }
    }

    public void MinimapClear()
    {
        while(minimapTiles.Count > 0) Destroy(minimapTiles.Dequeue());
    }
}
