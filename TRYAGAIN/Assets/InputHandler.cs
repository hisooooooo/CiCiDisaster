using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    public Vector2 MoveInput { get; private set; }
    public float JumpInput { get; private set; }
    public float SlideInput { get; private set; }

    Controls _input;

    private void OnEnable()
    {
        _input = new Controls();
        _input.Player.Enable();

        _input.Player.Move.performed += SetMove;
        _input.Player.Move.canceled += SetMove;

        _input.Player.Jump.performed += SetJump;
        _input.Player.Jump.canceled += SetJump;

        _input.Player.Slide.performed += SetSlide;
        _input.Player.Slide.canceled += SetSlide;

        
    }

    private void OnDisable()
    {
       

        _input.Player.Move.performed += SetMove;
        _input.Player.Move.canceled += SetMove;

        _input.Player.Jump.performed += SetJump;
        _input.Player.Jump.canceled += SetJump;

        _input.Player.Slide.performed += SetSlide;
        _input.Player.Slide.canceled += SetSlide;

        _input.Player.Disable();
    }

    private void SetMove(InputAction.CallbackContext input)
    {
        MoveInput = input.ReadValue<Vector2>();
    }

    private void SetJump(InputAction.CallbackContext input)
    {
        JumpInput = input.ReadValue <float>();
    }
    private void SetSlide(InputAction.CallbackContext input)
    {
        SlideInput = input.ReadValue<float>();
    }
}
