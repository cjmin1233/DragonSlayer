using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControl : MonoBehaviour
{
    private MyDefaultInputAction inputAction;

    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool jump
    {
        get
        {
            return inputAction.PlayerInput.Jump.triggered;
        }
        set
        {
            jump = value;
        }
    }
    public bool sprint { get; private set; }

    private void Start()
    {
        inputAction = new MyDefaultInputAction();
        inputAction.PlayerInput.Enable();

        inputAction.PlayerInput.Move.performed += MoveControl;
        inputAction.PlayerInput.Move.canceled += context => moveInput = Vector2.zero;

        inputAction.PlayerInput.Look.performed += LookControl;
        inputAction.PlayerInput.Look.canceled += context=>lookInput=Vector2.zero;
        
        inputAction.PlayerInput.Sprint.performed += context => sprint = true;
        inputAction.PlayerInput.Sprint.canceled += context => sprint = false;
    }

    private void LookControl(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void MoveControl(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude > 1) moveInput = moveInput.normalized;
    }

}
