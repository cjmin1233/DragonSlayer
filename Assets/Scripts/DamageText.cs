using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DamageText : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    private TextMeshPro text; // TextMeshProUGUI 컴포넌트 사용
    private Color alpha;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        alpha = text.color;
    }

    private void Start()
    {
        moveSpeed = 2.0f;
        alphaSpeed = 2.0f;
        destroyTime = 2.0f;

        Invoke("DestroyObject", destroyTime);
    }

    private void Update()
    {
        // 텍스트를 위로 이동
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        // 텍스트 알파값 조절
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }

    public void ShowDamage(int damage)
    {
        text.text = damage.ToString(); // 받은 데미지 값을 텍스트로 설정
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}