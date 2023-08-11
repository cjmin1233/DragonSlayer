using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel; // 인벤토리 패널을 연결할 변수
    private bool isInventoryOpen = false; // 인벤토리 열림 상태를 추적

    private void Start()
    {
        inventoryPanel.SetActive(false); // 게임 시작 시 인벤토리 패널을 비활성화
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // I 키를 눌렀을 때
        {
            isInventoryOpen = !isInventoryOpen; // 인벤토리 열림 상태를 반전

            if (isInventoryOpen)
            {
                OpenInventory(); // 인벤토리 열기
            }
            else
            {
                CloseInventory(); // 인벤토리 닫기
            }
        }
    }

    private void OpenInventory()
    {
        inventoryPanel.SetActive(true); // 인벤토리 패널을 활성화하여 열린 상태로 설정
    }

    private void CloseInventory()
    {
        inventoryPanel.SetActive(false); // 인벤토리 패널을 비활성화하여 닫힌 상태로 설정
    }
}
