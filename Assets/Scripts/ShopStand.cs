using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopStand : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform standPoint;
    private GameObject _item;
    private int price;
    public void Interact(GameObject target)
    {
        // 플레이어 골드 소모
        _item.GetComponent<Collider>().enabled = true;
        gameObject.SetActive(false);
    }

    public void EnterInteract(GameObject target) => UiManager.Instance.ShowInteractInfo($"Purchase {price}G");
    public void StandSetup(GameObject item, int priceUnit)
    {
        _item = item;
        _item.SetActive(true);
        _item.transform.position = standPoint.position;
        _item.GetComponent<Collider>().enabled = false;
        price = priceUnit * ((int)item.GetComponent<ItemObject>().GetItemTier() + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.Add2InteractList(this);
    }

    private void OnTriggerExit(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth is not null) playerHealth.RemoveInteractable(this);
    }
}
