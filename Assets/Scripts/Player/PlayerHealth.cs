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
    private Rigidbody _rigidBody;
    private PlayerCombat _playerCombat;
    private PlayerMove _playerMove;
    private PlayerInputControl _playerInputControl;
    private PlayerAnimationEvent _animationEvent;
    
    // private bool isDead;
    private int _animIDIsDead;
    private int _animIDIsStunned;
    private int _animIDIsGetHit;
    
    // 무적 시간
    private bool isInvincible;
    private float invincibleTimer;
    private Coroutine invincibleProcess;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _playerCombat = GetComponent<PlayerCombat>();
        _playerMove = GetComponent<PlayerMove>();
        _playerInputControl = GetComponent<PlayerInputControl>();
        _animationEvent = GetComponentInChildren<PlayerAnimationEvent>();
        
        _animationEvent.OnEndHitAction += EndHit;
        _animIDIsDead = Animator.StringToHash("IsDead");
        _animIDIsStunned = Animator.StringToHash("IsStunned");
        _animIDIsGetHit = Animator.StringToHash("IsGetHit");

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
        
        _rigidBody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        
        _animator.applyRootMotion = false;

        ToggleFreezePlayer(false);
    }

    public override void TakeDamage(DamageMessage damageMessage)
    {
        // 가격자가 본인이면 리턴
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

        if (_playerMove.IsRolling)
        {
            // 구르기로 회피
            return;
        }
        // 사망, 또는 무적시 리턴 *** >> isDead 추가할것
        if (isInvincible || _playerMove.IsRolling) return;
        
        // 데미지 처리
        if (damageMessage.isStiff)
        {
            _playerCombat.TerminateCombat();
            ToggleFreezePlayer(true);
            _animator.SetBool(_animIDIsGetHit, true);
        }
        
        base.TakeDamage(damageMessage);

        if (currentHp <= 0f) Die();
    }

    protected override IEnumerator StunProcess(float stunTime)
    {
        RemainingStunTime = stunTime;
        isStunned = true;
        _animator.SetBool(_animIDIsStunned, true);
        // _animator.Play("Dizzy", 0, 0f);
        
        _playerCombat.TerminateCombat();
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

        _rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotation;
        
        _animator.applyRootMotion = true;
        
        // isDead = true;
        _animator.SetBool(_animIDIsDead, true);

        ToggleFreezePlayer(true);
        print("Player Died!");
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
        // toggle player control and stop player.
        if (_playerInputControl is not null) _playerInputControl.enabled = !toggle;
        if (_playerMove is not null) _playerMove.enabled = !toggle;
        if (_playerCombat is not null) _playerCombat.enabled = !toggle;
        if (_rigidBody is not null && toggle) _rigidBody.velocity = Vector3.zero;
    }

    private void EndHit()
    {
        if (!isStunned) ToggleFreezePlayer(false);
        _animator.SetBool(_animIDIsGetHit, false);
    }
}
