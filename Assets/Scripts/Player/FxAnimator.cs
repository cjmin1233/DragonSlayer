using UnityEngine;

public class FxAnimator : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void EnableFx()=> _animator.Play("Enable",0,0f);
    public void DisableFx() => _animator.Play("Disable", 0, 0f);
}
