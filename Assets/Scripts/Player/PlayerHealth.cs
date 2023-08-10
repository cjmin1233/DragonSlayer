using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    public event Action OnDeath;
    
    [SerializeField] private PlayerScriptableObject playerScriptableObject;
    
    private Animator _animator;
    private Rigidbody _rigidBody;
    private PlayerCombat _playerCombat;
    private PlayerMove _playerMove;
    private PlayerInputControl _playerInputControl;
    private PlayerAnimationEvent _animationEvent;
    
    private int _animIDIsDead;
    private int _animIDIsStunned;
    private int _animIDIsGetHit;
    
    private bool isDead;
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
        isDead = false;

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
            // 패링 성공시 리턴
            float parryingResult = _playerCombat.CheckParrying(damageMessage.damager);

            if (parryingResult > 0f && !isInvincible) MakeInvincible(parryingResult);
        }

        if (_playerMove.IsRolling)
        {
            // 구르기로 회피
            return;
        }
        // 사망, 또는 무적시 리턴 *** >> isDead 추가할것
        if (isInvincible || _playerMove.IsRolling || isDead) return;
        
        // 데미지 처리
        if (damageMessage.isStiff)
        {
            // 경직 적용
            _playerCombat.TerminateCombat();
            ToggleFreezePlayer(true);
            _animator.SetBool(_animIDIsGetHit, true);
            _animator.Play("GetHit", 0, 0f);
        }
        print("플레이어 피격");
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
        
        isDead = true;
        _animator.SetBool(_animIDIsDead, true);

        ToggleFreezePlayer(true);
        print("Player Died!");
    }

    public void MakeInvincible(float invincibleTime)
    {
        if (invincibleTimer >= invincibleTime) return;
        if (invincibleProcess is not null) StopCoroutine(invincibleProcess);
        invincibleProcess = StartCoroutine(InvincibleTimer(invincibleTime));
        print("플레이어 무적 부여");
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
