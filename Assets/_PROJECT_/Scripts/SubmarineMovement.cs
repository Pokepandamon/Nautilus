
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SubmarineMovement : MonoBehaviour
{
    // Fields
    [SerializeField] private float _speed;
    private Rigidbody _rigidbody;
    private InputAction _linearMovementAction;
    private InputAction _rotationalMovementAction;
    private Vector2 _linearMovement;
    private float _rotationalMovement;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _linearMovementAction = InputSystem.actions.FindAction("ForwardBackUpDown");
        _rotationalMovementAction = InputSystem.actions.FindAction("Move");
    }
    
    private void Update()
    {
        _linearMovement = _linearMovementAction.ReadValue<Vector2>();
        _rotationalMovement = _rotationalMovementAction.ReadValue<Vector2>().x;
    }
    
    private void FixedUpdate()
    {
        _rigidbody.AddRelativeForce(_linearMovement.y * _speed, _linearMovement.x * _speed, 0f);
        _rigidbody.AddTorque(0f, _rotationalMovement * _speed, 0f);
    }
    
    // Public Methods
    
    // Private Methods
    
}
