using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class AddRoom : MonoBehaviour {

	private RoomTemplates templates;

	void Awake(){

		templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
		templates.rooms.Add(this.gameObject);
		this.gameObject.GetComponentInChildren<NavMeshSurface>();
    }
}
