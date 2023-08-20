using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class StageChange : MonoBehaviour
{
    private GameObject player;

    
    private void Start()
    {
        player = PlayerHealth.Instance.gameObject;
        // player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (MapVector2.Instance.Stage == 3)
            {
                // SceneManager.LoadScene("BossScene");
                GameManager.Instance.LoadScene(SceneType.Boss);
                return;
            }

            // GameManager.Instance.generatedRooms.Clear();
            // EnemySpawner.Instance.MapRecordClear();
            //
            // MapGenerator.Instance.epicSize = 0;
            // MapVector2.Instance.Stage++;
            // // fade in/out
            // player.transform.position = new Vector3(0, 0, 0);
            //
            // MapVector2.Instance.GenerateDungeon();
            GameManager.Instance.MoveNextStage();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(MapVector2.Instance.Stage == 3)
            {
                GameManager.Instance.LoadScene(SceneType.Boss);
                return;
            }
            
            // GameManager.Instance.generatedRooms.Clear();
            // EnemySpawner.Instance.MapRecordClear();
            //
            // MapGenerator.Instance.epicSize = 0;
            // MapVector2.Instance.Stage++;
            // // fade in/out
            // player.transform.position = Vector3.zero;
            // MapVector2.Instance.GenerateDungeon();
            // MinimapCameraFollow.Instance.FollowMinimap();
            GameManager.Instance.MoveNextStage();
        }

        
    }
}
