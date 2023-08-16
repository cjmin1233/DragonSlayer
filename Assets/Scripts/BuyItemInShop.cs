using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItemInShop : MonoBehaviour
{
    public GameObject[] itemPrefabs; // 아이템 프리팹 배열
    public Transform[] spawnPoints; // 소환 지점 배열

    private bool isInsideShop = false;

    private void Start()
    {
        SpawnItems();
    }

    private void Update()
    {
        if (isInsideShop && Input.GetKeyDown(KeyCode.B))
        {
            BuyItem();
        }
    }

    void SpawnItems()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomItemIndex = Random.Range(0, itemPrefabs.Length); // 랜덤 아이템 인덱스
            Vector3 spawnPosition = spawnPoints[i].position; // 소환 지점의 위치

            GameObject newItem = Instantiate(itemPrefabs[randomItemIndex], spawnPosition, Quaternion.identity); // 아이템 프리팹 소환
            newItem.tag = "Item"; // 아이템 태그 설정 (필요한 경우)
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shop"))
        {
            isInsideShop = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shop"))
        {
            isInsideShop = false;
        }
    }

    private void BuyItem()
    {
        // 아이템 구매 로직을 여기에 구현
        Debug.Log("아이템을 구매했습니다.");
    }
}
