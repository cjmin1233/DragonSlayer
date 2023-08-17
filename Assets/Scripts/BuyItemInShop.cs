using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItemInShop : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public Transform[] spawnPoints;
    public int[] itemPrices;

    public int playerCoins = 500; // 초기 플레이어 코인

    private int[] shopPrices; // 각 상점에 해당하는 아이템 가격 배열

    private void Start()
    {
        shopPrices = new int[spawnPoints.Length]; // shopPrices 배열 초기화
        SpawnItems();
    }

    private void SpawnItems()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int randomItemIndex = Random.Range(0, itemPrefabs.Length);
            GameObject newItem = Instantiate(itemPrefabs[randomItemIndex], spawnPoints[i].position + (Vector3.up * 1), Quaternion.identity);
            newItem.transform.SetParent(spawnPoints[i]); // 아이템을 spawnPoints[i]의 하위로 설정

            // shopPrices 배열에 해당 아이템 가격 저장
            shopPrices[i] = itemPrices[randomItemIndex];
        }

        Debug.Log("shopPrices:" + shopPrices.Length);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Shop") && Input.GetKeyDown(KeyCode.B)) // 상점 칸에 충돌했을 경우
        {
            int shopIndex = -1; // 상점 칸 인덱스를 저장할 변수 초기화

            // 상점 칸의 Transform과 일치하는 인덱스 찾기
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] == collision.transform)
                {
                    shopIndex = i; // 상점 칸 인덱스 설정
                    break; // 일치하는 인덱스를 찾으면 반복문 종료
                }
            }

            if (shopIndex != -1)
            {
                BuyItem(shopIndex);
            }
        }
    }

    private void BuyItem(int shopIndex)
    {
        if (shopIndex >= 0 && shopIndex < spawnPoints.Length)
        {
            int itemPrice = shopPrices[shopIndex];

            if (playerCoins >= itemPrice)
            {
                if (spawnPoints[shopIndex].childCount > 1)
                {
                    Transform secondItemTransform = spawnPoints[shopIndex].GetChild(1); // 두 번째 아이템 가져오기
                    string itemName = secondItemTransform.name;

                    // 아이템 구매 로직
                    Debug.Log("아이템을 구매했습니다: " + itemName);
                    playerCoins -= itemPrice; // 코인 차감

                    // 아이템 박스를 파괴
                    Transform shopTransform = spawnPoints[shopIndex];
                    if (shopTransform.childCount > 0)
                    {
                        Destroy(shopTransform.GetChild(0).gameObject);
                    }
                }               
            }
            else
            {
                Debug.Log("코인이 부족합니다.");
            }
        }
    }
}