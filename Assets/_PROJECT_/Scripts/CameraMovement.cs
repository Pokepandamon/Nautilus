
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    // Fields
    [SerializeField] private float _distance;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _sensitivity;
    private float _theta = 0;
    private float _phi = 0;
    private InputAction _mouseDelta;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _mouseDelta = InputSystem.actions.FindAction("Look");
        transform.position = _distance * new Vector3(Mathf.Cos(_theta), Mathf.Sin(_phi), Mathf.Sin(_theta));
    }
    
    private void Update()
    {
        _theta -= _mouseDelta.ReadValue<Vector2>().x * _sensitivity / 100;
        _phi = Mathf.Clamp(_phi - _mouseDelta.ReadValue<Vector2>().y * _sensitivity / 100, -Mathf.PI/2, Mathf.PI/2);
        Debug.Log("theta: " + _theta + " | phi: " + _phi);
        transform.position = _playerTransform.position + _distance * new Vector3(Mathf.Cos(_theta), Mathf.Sin(_phi), Mathf.Sin(_theta));
        transform.LookAt(_playerTransform);
    }
    
    // Public Methods
    
    // Private Methods
    
}
