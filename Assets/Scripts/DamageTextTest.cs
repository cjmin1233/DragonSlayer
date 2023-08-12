using UnityEngine;
using UnityEngine.EventSystems;

public class DamageTextTest : MonoBehaviour
{
    public int damage = 1111;
    public GameObject hudDamageText;
    public Transform hudPos;

    public void TakenDamage()
    {
        Debug.Log("데미지를 받았습니다.");
        GameObject hudText = Instantiate(hudDamageText);
        hudText.transform.position = hudPos.position;
        DamageText damageText = hudText.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.ShowDamage(damage); // DamageText 스크립트의 ShowDamage 호출
        }
    }
}