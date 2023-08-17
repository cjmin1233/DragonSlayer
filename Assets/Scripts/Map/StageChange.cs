using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageChange : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (MapVector2.instance.Stage == 3)
            {
                //SceneManager.LoadScene("BossScene");
            }

            GameManager.Instance.generatedRooms.Clear();
            EnemySpawner.Instance.MapRecordClear();

            MapGenerator.Instance.epicSize = 0;
            MapVector2.instance.Stage++;
            // fade in/out
            player.transform.position = new Vector3(0, 10, 0);

            MapVector2.instance.GenerateDungeon();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(MapVector2.instance.Stage == 3)
            {
                //SceneManager.LoadScene("");
            }
 
            GameManager.Instance.generatedRooms.Clear();
            EnemySpawner.Instance.MapRecordClear();

            MapGenerator.Instance.epicSize = 0;
            MapVector2.instance.Stage++;
            // fade in/out
            player.transform.position = new Vector3(0, 10, 0);
            MapVector2.instance.GenerateDungeon();
            MinimapCameraFollow.Instance.FollowMinimap();
        }

        
    }
}
