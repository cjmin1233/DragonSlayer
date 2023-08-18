using System.Collections;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float alphaSpeed;
    [SerializeField] private float lifeTime;
    private float damageAmount;
    
    private TextMeshPro damageText; // TextMeshProUGUI 컴포넌트 사용
    private Color alpha;
    private Coroutine damageTextRoutine;

    public void SetDamageValue(float damage) => damageAmount = damage;
    
    private void Awake()
    {
        damageText = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        if (damageTextRoutine is not null) StopCoroutine(damageTextRoutine);
        damageTextRoutine = StartCoroutine(DamageTextRoutine());
    }

    private IEnumerator DamageTextRoutine()
    {
        damageText.text = damageAmount.ToString("F0");
        alpha = damageText.color;
        alpha.a = 1f;
        float timer = lifeTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            // 텍스트를 위로 이동
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

            // 텍스트 알파값 조절
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            damageText.color = alpha;
            yield return null;
        }

        EffectManager.Instance.Add2Pool((int)EffectType.DamageText, gameObject);
    }
}