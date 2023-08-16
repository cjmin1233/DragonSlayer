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
            string itemName = other.gameObject.name; // �浹�� ������Ʈ�� �̸� ��������
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>(); // InvetoryManager �ν��Ͻ� ��������
            if (inventoryManager != null)
            {
                inventoryManager.SetItem(itemName); // ������ �߰�
            }
            Destroy(other.gameObject); // ������Ʈ �ı�
        }
    }
}





