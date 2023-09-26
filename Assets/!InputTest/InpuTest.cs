using System;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class InpuTest : MonoBehaviour
{
    private PlayerInput _playerInput;
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Debug.Log("AA");
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Move"].performed += OnMovement;
        _playerInput.actions["Move"].started   += OnStart;
        _playerInput.actions["Move"].canceled  += OnCancel;
    }

    
    public void OnMovement(InputAction.CallbackContext _obj)
    {
        if (_playerInput.currentControlScheme != "Gamepad") return;
        Debug.Log($"OnMovement : {_obj.ReadValue<Vector2>()} by {_obj.control}");
    }

    public void OnStart(InputAction.CallbackContext _obj) => Debug.Log($"OnStart : {_obj.ReadValue<Vector2>()} by {_obj.control}");

    public void OnCancel(InputAction.CallbackContext   _obj) => Debug.Log($"OnCancel : {_obj.ReadValue<Vector2>()} by {_obj.control}");

    private void Update()
    {
        /*
        StringBuilder _sb = new StringBuilder();
        foreach (InputDevice _playerInputDevice in _playerInput.devices)
        {
            _sb.Append(_playerInputDevice + " ");
        }
        
        Debug.Log($"Devices : {_playerInput.devices.Count} - {_sb}");
        */
    }
}
