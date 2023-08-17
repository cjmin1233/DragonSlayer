using UnityEngine;
using TMPro;

public class SmallPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI infoText;
    private Animator _animator;
    private int _animIdTrigger;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animIdTrigger = Animator.StringToHash("Trigger");
    }

    public void EnableInfo(string description)
    {
        infoText.text = description;
        _animator.SetBool(_animIdTrigger, true);
    }

    public void DisableInfo() => _animator.SetBool(_animIdTrigger, false);
}
