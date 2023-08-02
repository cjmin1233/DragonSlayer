using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEditorInternal;
using Unity.AI.Navigation;
using UnityEditor;
using NavMeshSurface = UnityEngine.AI.NavMeshSurface;

public class RoomSpawner : MonoBehaviour {

	public int openingDirection;
	// 1 --> need bottom door
	// 2 --> need top door
	// 3 --> need left door
	// 4 --> need right door
    public static RoomSpawner instance;
	private RoomTemplates templates;
	private int rand;
	public bool spawned = false;

	public float waitTime = 4f;

	public GameObject bakePrefab;

    void Awake()
	{
		instance = this;
        RoomSpawn();
	}

	void RoomSpawn()
	{
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

	
	void Spawn(){
		if(spawned == false){
			if(openingDirection == 1){
				// Need to spawn a room with a BOTTOM door.
				rand = Random.Range(0, templates.bottomRooms.Length);
				bakePrefab = Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
			} else if(openingDirection == 2){
				// Need to spawn a room with a TOP door.
				rand = Random.Range(0, templates.topRooms.Length);
                bakePrefab = Instantiate(templates.topRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
            } else if(openingDirection == 3){
				// Need to spawn a room with a LEFT door.
				rand = Random.Range(0, templates.leftRooms.Length);
                bakePrefab = Instantiate(templates.leftRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
            } else if(openingDirection == 4){
				// Need to spawn a room with a RIGHT door.
				rand = Random.Range(0, templates.rightRooms.Length);
                bakePrefab = Instantiate(templates.rightRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
            }
            //templates.bottomRooms[rand].transform.rotation
            spawned = true;
        }
	}
	private void OnTriggerEnter(Collider other){
		if(other.CompareTag("SpawnPoint")){
			if(other.GetComponent<RoomSpawner>().spawned == false && spawned == false){
				if(transform.position != new Vector3(0f, 0f, 0f))
				{
                    Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
				
			} 
			spawned = true;
		}
		
	}
    
}
