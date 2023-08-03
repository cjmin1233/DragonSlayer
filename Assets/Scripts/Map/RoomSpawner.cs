using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class RoomSpawner : MonoBehaviour {

	
    public static RoomSpawner instance;
    public int openingDirection;
	private int waitTime = 4;
	public bool spawned = false;

    private void Start()
    {
        var roomCount = SpawnManager.instance.randSum;
        Destroy(gameObject, waitTime);
		if(roomCount < 0 && spawned == false)
		{
			RoomGene();
		}
    }
	private void RoomGene()
	{
        SpawnManager.instance.randSum--;

        if (openingDirection == 1)
        {
            Instantiate(SpawnManager.instance.bottomRooms[SpawnManager.instance.rand[0]], transform.position, SpawnManager.instance.bottomRooms[SpawnManager.instance.rand[0]].transform.rotation);
            print("1");
        }
        else if (openingDirection == 2)
        {
            Instantiate(SpawnManager.instance.topRooms[SpawnManager.instance.rand[1]], transform.position, SpawnManager.instance.bottomRooms[SpawnManager.instance.rand[1]].transform.rotation);
        }
        else if (openingDirection == 3)
        {
            Instantiate(SpawnManager.instance.leftRooms[SpawnManager.instance.rand[2]], transform.position, SpawnManager.instance.bottomRooms[SpawnManager.instance.rand[2]].transform.rotation);
        }
        else if (openingDirection == 4)
        {
            Instantiate(SpawnManager.instance.rightRooms[SpawnManager.instance.rand[3]], transform.position, SpawnManager.instance.bottomRooms[SpawnManager.instance.rand[3]].transform.rotation);
        }
        spawned = true;
    }
    private void OnTriggerEnter(Collider other){
		if(other.CompareTag("SpawnPoint")){
			if(other.GetComponent<SpawnManager>().spawned == false && SpawnManager.instance.spawned == false){
				if(transform.position != new Vector3(0f, 0f, 0f))
				{
                    Instantiate(SpawnManager.instance.closedRoom, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
				
			} 
			SpawnManager.instance.spawned = true;
		}
		
	}
    
}
