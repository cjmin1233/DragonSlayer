using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : LivingEntity
{
    public static PlayerHealth Instance { get; private set; }
    
    public float MaxHP
    {
        get => maxHp;
    }

    public float CurHP
    {
        get => currentHp;
    }

    public float MaxVitality { get; private set; }
    public float CurVitality { get; private set; }

    private float vitalityRestoreRate;
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
    
    // 오브젝트 interaction
    private List<IInteractable> contactObjects = new List<IInteractable>();
    private void Awake()
    {
        if (Instance is null) Instance = this;
        else if (!Instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        
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

    private void Start()
    {
        _playerInputControl.OnInteractAction += UseItem;
    }

    private void Update()
    {
        RestoreVitality(vitalityRestoreRate * Time.deltaTime);
    }

    public void PlayerInit(PlayerScriptableObject playerSo)
    {
        isDead = false;

        maxHp = playerSo.health;
        currentHp = maxHp;

        MaxVitality = playerSo.vitality;
        CurVitality = MaxVitality;

        vitalityRestoreRate = playerSo.vitalityRestoreRate;

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
        // 사망, 구르기 또는 무적시 리턴
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
        // print("player hit on point : " + damageMessage.hitPoint);
        base.TakeDamage(damageMessage);

        if (currentHp <= 0f) Die();
    }
    protected override IEnumerator StunProcess(float stunTime)
    {
        _playerCombat.TerminateCombat();
        _animator.SetBool(_animIDIsStunned, true);
        ToggleFreezePlayer(true);
        yield return base.StunProcess(stunTime);
        _animator.SetBool(_animIDIsStunned, false);
        ToggleFreezePlayer(false);
        print("overrided stun process in playerHealth");
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

        GameManager.Instance.OnPlayerDeath();
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

        if (toggle) _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
        else _rigidBody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
    }

    private void EndHit()
    {
        if (!isStunned) ToggleFreezePlayer(false);
        _animator.SetBool(_animIDIsGetHit, false);
    }

    public bool IsVitalityEnough(float amount) => amount <= CurVitality ? true : false;

    public void SpendVitality(float amount)
    {
        CurVitality = Mathf.Clamp(CurVitality - amount, 0f, MaxVitality);
    }

    public void RestoreVitality(float amount) => CurVitality = Mathf.Clamp(CurVitality + amount, 0f, MaxVitality);

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable is not null)
        {
            contactObjects.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable is not null && contactObjects.Contains(interactable))
        {
            contactObjects.Remove(interactable);
        }
    }

    private void UseItem()
    {
        if (contactObjects.Count <= 0) return;
        print("use item!");
        var selectedItem = contactObjects[0];
        contactObjects.RemoveAt(0);
        selectedItem.Interact(gameObject);
    }

    public void RestoreHealth(float amount)
    {
        currentHp = Mathf.Clamp(currentHp + amount, 0f, maxHp);
    }
}
