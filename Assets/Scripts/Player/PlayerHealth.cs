using System;
using UnityEngine;

public class PlayerHealth : LIvingEntity
{
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    
    private Animator _animator;
    private Rigidbody _rigidbody;
    
    private bool isDead;
    private int _animIDIsDead;
    public event Action OnDeath;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _animIDIsDead = Animator.StringToHash("IsDead");

        OnDeath = () => { };
    }

    private void OnEnable()
    {
        PlayerInit(playerScriptableObject);
    }

    public void PlayerInit(PlayerScriptableObject playerSo)
    {
        isDead = false;

        maxHp = playerSo.health;
        currentHp = maxHp;
        
        _rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        
        _animator.applyRootMotion = false;
        
        var playerInput = GetComponent<PlayerInputControl>();
        var playerMove = GetComponent<PlayerMove>();
        var playerCombat = GetComponent<PlayerCombat>();
        if (playerInput is not null) playerInput.enabled = true;
        if (playerMove is not null) playerMove.enabled = true;
        if (playerCombat is not null) playerCombat.enabled = true;
    }

    public override void TakeDamage(DamageMessage damageMessage)
    {
        base.TakeDamage(damageMessage);

        if (currentHp <= 0f) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        
        _animator.applyRootMotion = true;
        
        isDead = true;
        _animator.SetBool(_animIDIsDead, true);

        var playerInput = GetComponent<PlayerInputControl>();
        var playerMove = GetComponent<PlayerMove>();
        var playerCombat = GetComponent<PlayerCombat>();
        if (playerInput is not null) playerInput.enabled = false;
        if (playerMove is not null) playerMove.enabled = false;
        if (playerCombat is not null) playerCombat.enabled = false;
    }
}
