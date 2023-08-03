using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AddRoom : MonoBehaviour {


    void Start()
	{
        var spawnManager = GameObject.FindGameObjectWithTag("Rooms").GetComponent<SpawnManager>();
        spawnManager.rooms.Add(this.gameObject);
    }
}
