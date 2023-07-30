using UnityEngine;

[RequireComponent(typeof(PlayerInputControl), typeof(PlayerMove))]
public class PlayerCombat : MonoBehaviour
{
    public enum PlayerComboType
    {
        Nm,
        Sp
    }

    public bool IsComboActive
    {
        get
        {
            return curComboData is not null;
        }
    }

    [SerializeField] private ComboData[] playerComboData;
    private ComboData curComboData;
    private AnimatorStateInfo GetCurStateInfo(int layerIndex) => _animator.GetCurrentAnimatorStateInfo(layerIndex);

    private Animator _animator;
    private PlayerInputControl _playerInput;
    private PlayerMove _playerMove;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInputControl>();
        _playerMove = GetComponent<PlayerMove>();
    }
    private void Update()
    {
        if (_playerInput.attackNormal) StartCombo(PlayerComboType.Nm);
        if (_playerInput.attackSpecial) StartCombo(PlayerComboType.Sp);
        ExitCombo();
    }

    private void StartCombo(PlayerComboType comboType)
    {
        // 공중이거나 구르기 중 공격 못함
        if (!_playerMove.Grounded || _playerMove.IsRolling) return;
        
        if (curComboData is null) curComboData = playerComboData[(int)comboType];
        // 다른 콤보중인 경우
        else if (curComboData.comboType != comboType) return;
        
        int curComboCounter = curComboData.comboCounter;
        
        if (curComboCounter < curComboData.combos.Count
            && Time.time > curComboData.nextComboStartTime)
        {
            // 첫 콤보 또는 콤보중
            if (curComboCounter == 0 ||
                (curComboCounter > 0 && GetCurStateInfo(0).IsTag("Attack")
                                     && GetCurStateInfo(0).normalizedTime > curComboData
                                         .combos[curComboCounter - 1].normalizedComboTime))
            {
                _animator.runtimeAnimatorController = curComboData.combos[curComboCounter].animatorOv;
                _animator.Play("Attack",0,0);

                // 반복 콤보가 아니면 다음 콤보 진행
                if (!curComboData.combos[curComboCounter].loop) curComboData.comboCounter++;

                // 콤보 끝 도달
                if (curComboData.comboCounter >= curComboData.combos.Count) EndCombo(comboType);
            }
        }
    }

    private void EndCombo(PlayerComboType comboType)
    {
        curComboData = playerComboData[(int)comboType];
        int curComboCounter = curComboData.comboCounter;

        // 다음 콤보 딜레이 적용
        if (curComboCounter > 0 && curComboData.combos[curComboCounter - 1].nextComboInterval > 0f)
        {
            curComboData.nextComboStartTime = Time.time + curComboData.combos[curComboCounter - 1].nextComboInterval;
        }

        // 콤보 초기화
        curComboData.comboCounter = 0;
        curComboData = null;
    }

    private void ExitCombo()
    {
        if (curComboData is null) return;
        int curComboCounter = curComboData.comboCounter;

        // 공격 상태 벗어남
        if (!GetCurStateInfo(0).IsTag("Attack")) EndCombo(curComboData.comboType);
        // 콤보 시간 종료
        else if (curComboCounter > 0
                 && GetCurStateInfo(0).normalizedTime > curComboData.combos[curComboCounter - 1].normalizedExitTime)
        {
            EndCombo(curComboData.comboType);
        }
    }
}
