using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour
{
    [SerializeField] private StandardSlider playerHealthBar;
    [SerializeField] private StandardSlider playerVitalityBar;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private SmallPopup interactInfo;

    public void UpdateUi()
    {
        playerHealthBar.UpdateValue(PlayerHealth.Instance.CurHP, PlayerHealth.Instance.MaxHP);
        playerVitalityBar.UpdateValue(PlayerHealth.Instance.CurVitality, PlayerHealth.Instance.MaxVitality);
    }

    public void ToggleInventory() => inventoryManager.gameObject.SetActive(!inventoryManager.gameObject.activeSelf);
    public void GetItem2Inventory(ItemScriptableObject itemData) => inventoryManager.SetItem(itemData);

    public void ShowInteractInfo(string description) => interactInfo.EnableInfo(description);
    public void HideInteractInfo() => interactInfo.DisableInfo();
}
