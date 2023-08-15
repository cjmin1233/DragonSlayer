using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItem : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            string itemName = other.gameObject.name; // 충돌한 오브젝트의 이름 가져오기
            InvetoryManager inventoryManager = FindObjectOfType<InvetoryManager>(); // InvetoryManager 인스턴스 가져오기
            if (inventoryManager != null)
            {
                inventoryManager.SetItem(itemName); // 아이템 추가
            }
            Destroy(other.gameObject); // 오브젝트 파괴
        }
    }
}





