using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    public Vector2 MoveInput { get; private set; }
    public bool JumpInput { get; private set; }
    public float SlideInput { get; private set; }

    public Vector2 RawDirectionInput { get; private set; }
    public Vector2 NormalizedDirectionInput { get; private set; }

    public float MouseWheelInput { get; private set; }

    public float AirDashInput { get; private set; }
    public float GrappleInput { get; private set; }

    Controls _input;
    Camera cam;

    private void Start()
    {

        cam = Camera.main;
    }
    private void Update()
    {
        
    }

    private void OnEnable()
    {
        _input = new Controls();
        _input.Player.Enable();

        _input.Player.Move.performed += SetMove;
        _input.Player.Move.canceled += SetMove;

     

        _input.Player.Slide.performed += SetSlide;
        _input.Player.Slide.canceled += SetSlide;

        _input.Player.AirDash.performed += SetAirDash;
        _input.Player.AirDash.canceled += SetAirDash;

        _input.Player.AirDashDirection.performed += SetRawAirDashDirection;

        _input.Player.Grapple.started += SetGrapple;
        _input.Player.Grapple.performed += SetGrapple;
        _input.Player.Grapple.canceled += SetGrapple;

        _input.Player.Zoom.performed += SetZoom;
        _input.Player.Zoom.canceled += SetZoom;

    }

    private void OnDisable()
    {
       

        _input.Player.Move.performed += SetMove;
        _input.Player.Move.canceled += SetMove;

   

        _input.Player.Slide.performed += SetSlide;
        _input.Player.Slide.canceled += SetSlide;


        _input.Player.AirDash.performed += SetAirDash;
        _input.Player.AirDash.canceled += SetAirDash;

        _input.Player.AirDashDirection.performed += SetRawAirDashDirection;

        _input.Player.Grapple.started += SetGrapple;
        _input.Player.Grapple.performed += SetGrapple;
        _input.Player.Grapple.canceled += SetGrapple;

        _input.Player.Zoom.performed += SetZoom;
        _input.Player.Zoom.canceled += SetZoom;


        _input.Player.Disable();
    }

    private void SetMove(InputAction.CallbackContext input)
    {
        MoveInput = input.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            JumpInput = true;
        }
        else JumpInput = false;
     
    }
    private void SetSlide(InputAction.CallbackContext input)
    {
        SlideInput = input.ReadValue<float>();
    }
    private void SetAirDash(InputAction.CallbackContext input)
    {
        AirDashInput = input.ReadValue<float>();
    }
    private void SetRawAirDashDirection(InputAction.CallbackContext input)
    {
        RawDirectionInput= input.ReadValue<Vector2>();

        RawDirectionInput = cam.ScreenToWorldPoint((Vector3)RawDirectionInput) - transform.position;

        NormalizedDirectionInput = (RawDirectionInput.normalized);

    }

    private void SetGrapple(InputAction.CallbackContext input)
    {
        if (input.started)
        {
            GrappleInput = 1;
        }
        if (input.performed)
        {
            GrappleInput = 2;
        }
        if (input.canceled)
        {
            GrappleInput = 3;
        }
      
        
    }

    private void SetZoom(InputAction.CallbackContext input)
    {
        Vector2 rawZoom = input.ReadValue<Vector2>();
        rawZoom = rawZoom.normalized;
        MouseWheelInput = rawZoom.y;
    }
}
