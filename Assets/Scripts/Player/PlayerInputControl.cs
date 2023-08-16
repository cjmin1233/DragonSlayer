using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControl : MonoBehaviour
{
    private MyDefaultInputAction inputAction;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool Jump
    {
        get
        {
            return inputAction.PlayerInput.Jump.triggered;
        }
    }
    public bool Sprint { get; private set; }
    public bool Roll
    {
        get
        {
            return inputAction.PlayerInput.Roll.triggered;
        }
    }
    public bool AttackNormal { get; private set; }
    public bool AttackSpecial { get; private set; }
    public bool Guard { get; private set; }

    public event Action OnInteractAction;
    private void Start()
    {
        inputAction = new MyDefaultInputAction();
        inputAction.PlayerInput.Enable();

        inputAction.PlayerInput.Move.performed += MoveControl;
        inputAction.PlayerInput.Move.canceled += context => MoveInput = Vector2.zero;

        inputAction.PlayerInput.Look.performed += LookControl;
        inputAction.PlayerInput.Look.canceled += context=>LookInput=Vector2.zero;
        
        inputAction.PlayerInput.Sprint.performed += context => Sprint = true;
        inputAction.PlayerInput.Sprint.canceled += context => Sprint = false;

        inputAction.PlayerInput.AttackNm.performed += context => AttackNormal = true;
        inputAction.PlayerInput.AttackNm.canceled += context => AttackNormal = false;
        inputAction.PlayerInput.AttackSp.performed += context => AttackSpecial = true;
        inputAction.PlayerInput.AttackSp.canceled += context => AttackSpecial = false;

        inputAction.PlayerInput.Guard.performed += context => Guard = true;
        inputAction.PlayerInput.Guard.canceled += context => Guard = false;

        inputAction.PlayerInput.Interact.started += InteractControl;
    }

    private void LookControl(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    private void MoveControl(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        if (MoveInput.sqrMagnitude > 1) MoveInput = MoveInput.normalized;
    }
    private void InteractControl(InputAction.CallbackContext obj) => OnInteractAction?.Invoke();
}
