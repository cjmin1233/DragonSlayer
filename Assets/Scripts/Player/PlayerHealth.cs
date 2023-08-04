using System;
using UnityEngine;

public class PlayerHealth : LIvingEntity
{
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    
    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerCombat _playerCombat;
    
    // private bool isDead;
    private int _animIDIsDead;
    public event Action OnDeath;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
        
        _animIDIsDead = Animator.StringToHash("IsDead");

        OnDeath = () => { };
    }

    private void OnEnable()
    {
        PlayerInit(playerScriptableObject);
    }

    public void PlayerInit(PlayerScriptableObject playerSo)
    {
        // isDead = false;

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
        if (damageMessage.damager == gameObject) return;
        
        if (_playerCombat.IsGuarding)
        {
            Vector3 damagerDirection = damageMessage.damager.transform.position - transform.position;

            float angleToDamager = AngleBetweenVectors(transform.forward, damagerDirection);
            print(angleToDamager);
            if (angleToDamager <= 30f) print("Guard!!");
        }
        base.TakeDamage(damageMessage);

        if (currentHp <= 0f) Die();
    }
    

    private void Die()
    {
        OnDeath?.Invoke();

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        
        _animator.applyRootMotion = true;
        
        // isDead = true;
        _animator.SetBool(_animIDIsDead, true);

        var playerInput = GetComponent<PlayerInputControl>();
        var playerMove = GetComponent<PlayerMove>();
        var playerCombat = GetComponent<PlayerCombat>();
        if (playerInput is not null) playerInput.enabled = false;
        if (playerMove is not null) playerMove.enabled = false;
        if (playerCombat is not null) playerCombat.enabled = false;
    }
    public static float AngleBetweenVectors(Vector3 from, Vector3 to)
    {
        from.Normalize();
        to.Normalize();

        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f)) * Mathf.Rad2Deg;
        return angle;
    }
}
