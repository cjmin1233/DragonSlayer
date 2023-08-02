using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour {

	public GameObject[] bottomRooms;
	public GameObject[] topRooms;
	public GameObject[] leftRooms;
	public GameObject[] rightRooms;

	public GameObject closedRoom;

	public List<GameObject> rooms;

    public int lastValue;
    public DateTime lastUpdateTime;

    public float waitTime;
	private bool spawnedBoss;
	public GameObject boss;

    private void Awake()
    {
        lastValue = 0;
        lastUpdateTime = DateTime.Now;
    }

    private void Start()
    {
        BossSpawn();
        CheckIfValueStaysSameForDuration(TimeSpan.FromSeconds(waitTime));
    }

    public void BossSpawn(){

        if (waitTime <= 0 && spawnedBoss == false)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i == rooms.Count - 1)
                {
                    Instantiate(boss, rooms[i].transform.position, Quaternion.identity);
                    spawnedBoss = true;
                }
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }

    }
    public void CheckIfValueStaysSameForDuration(TimeSpan duration)
    {
        TimeSpan timeSinceLastUpdate = DateTime.Now - lastUpdateTime;

        if (timeSinceLastUpdate >= duration)
        {
            Console.WriteLine("Value stayed the same for " + duration.TotalSeconds + " seconds!");
        }
    }
}
