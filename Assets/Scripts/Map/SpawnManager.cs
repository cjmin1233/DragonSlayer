using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public GameObject closedRoom;
    public GameObject BossRoom;
    public GameObject Key;
    public List<GameObject> rooms;

    private Transform spawnPoint;
    public bool spawned = false;

    public int[] rand = new int[4];
    public int randSum;
    public int stage = 1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        RoomSetting(stage);
        foreach(int ran in rand)
            print(rand[ran]);
        print(randSum);
    }
    public void RoomSetting(int _stage)
    {
        randSum = 0;
        switch (_stage)
        {
            case 1:
                for (var i = 0; i < rand.Length; i++)
                {
                    rand[i] = Random.Range(1, 3);
                    randSum += rand[i];
                }
                break;
            case 2:
                for (var i = 0; i < rand.Length; i++)
                {
                    rand[i] = Random.Range(2, 4);
                    randSum += rand[i];
                }
                break;
            case 3:
                for (var i = 0; i < rand.Length; i++)
                {
                    rand[i] = Random.Range(3, 6);
                    randSum += rand[i];
                }
                break;
            default:
                break;
        }
    }
}
