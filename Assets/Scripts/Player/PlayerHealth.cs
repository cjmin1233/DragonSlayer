using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : LIvingEntity
{
    public static float AngleBetweenVectors(Vector3 from, Vector3 to)
    {
        from.Normalize();
        to.Normalize();

        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f)) * Mathf.Rad2Deg;
        return angle;
    }
    public event Action OnDeath;
    
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    
    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerCombat _playerCombat;
    
    // private bool isDead;
    private int _animIDIsDead;
    private int _animIDIsStunned;
    
    // 무적 시간
    private bool isInvincible;
    private float invincibleTimer;
    private Coroutine invincibleProcess;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
        
        _animIDIsDead = Animator.StringToHash("IsDead");
        _animIDIsStunned = Animator.StringToHash("IsStunned");

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

        ToggleFreezePlayer(false);
    }

    public override void TakeDamage(DamageMessage damageMessage)
    {
        // 무적 시간 있을 시 리턴 ***
        if (damageMessage.damager == gameObject) return;
        
        if (_playerCombat.IsGuarding)
        {
            Vector3 damagerDirection = damageMessage.damager.transform.position - transform.position;

            float angleToDamager = AngleBetweenVectors(transform.forward, damagerDirection);
            print(angleToDamager);
            if (angleToDamager <= 30f)
            {
                _playerCombat.Parrying();
            }
        }

        if (isInvincible) return;
        base.TakeDamage(damageMessage);

        if (currentHp <= 0f) Die();
    }

    protected override IEnumerator StunProcess(float stunTime)
    {
        RemainingStunTime = stunTime;
        isStunned = true;
        _animator.SetBool(_animIDIsStunned, true);
        _animator.Play("Dizzy", 0, 0f);
        
        _playerCombat.TerminateCombo();
        ToggleFreezePlayer(true);
        
        while (isStunned)
        {
            RemainingStunTime -= Time.deltaTime;
            if (RemainingStunTime <= 0f) isStunned = false;
            yield return null;
        }
        _animator.SetBool(_animIDIsStunned, false);
        ToggleFreezePlayer(false);
    }

    private void Die()
    {
        OnDeath?.Invoke();

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotation;
        
        _animator.applyRootMotion = true;
        
        // isDead = true;
        _animator.SetBool(_animIDIsDead, true);

        ToggleFreezePlayer(true);
    }

    public void MakeInvincible(float invincibleTime)
    {
        if (invincibleTimer >= invincibleTime) return;
        if (invincibleProcess is not null) StopCoroutine(invincibleProcess);
        invincibleProcess = StartCoroutine(InvincibleTimer(invincibleTime));
    }

    private IEnumerator InvincibleTimer(float invincibleTime)
    {
        invincibleTimer = invincibleTime;
        isInvincible = true;
        while (invincibleTimer >= 0f)
        {
            invincibleTimer -= Time.deltaTime;
            yield return null;
        }

        isInvincible = false;
    }

    private void ToggleFreezePlayer(bool toggle)
    {
        // toggle player control.
        var playerInput = GetComponent<PlayerInputControl>();
        var playerMove = GetComponent<PlayerMove>();
        var playerCombat = GetComponent<PlayerCombat>();
        if (playerInput is not null) playerInput.enabled = !toggle;
        if (playerMove is not null) playerMove.enabled = !toggle;
        if (playerCombat is not null) playerCombat.enabled = !toggle;
    }
}
