
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using RenderSettings = UnityEngine.RenderSettings;

public class CameraMovement : MonoBehaviour
{
    // Fields
    [SerializeField] private float _distance;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _sensitivity;
    [SerializeField] private Light _light;
    [SerializeField] private float _maxLight;
    [SerializeField] private float _minLight;
    [SerializeField] private float _maxHeight;
    [SerializeField] private float _minHeight;
    
    private float _theta = 180;
    private float _phi = 0;
    private InputAction _mouseDelta;
    private bool _lightInitialized;
    private Camera _camera;
    private bool _paused;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _paused = false;
        CanvasUIManager.OnPause += OnPause;
        CanvasUIManager.OnResume += OnResume;
        _mouseDelta = InputSystem.actions.FindAction("Look");
        transform.position = _distance * new Vector3(Mathf.Cos(_theta), Mathf.Sin(_phi), Mathf.Sin(_theta));
        _lightInitialized = _light != null;
        _camera = GetComponent<Camera>();
    }
    
    private void Update()
    {
        _theta -= _mouseDelta.ReadValue<Vector2>().x * _sensitivity / 100;
        _phi = Mathf.Clamp(_phi - _mouseDelta.ReadValue<Vector2>().y * _sensitivity / 100, -Mathf.PI/2, Mathf.PI/2);
        //Debug.Log("theta: " + _theta + " | phi: " + _phi);
        if (!_paused)
        {
            transform.position = _playerTransform.position +
                                 _distance * new Vector3(Mathf.Cos(_theta), Mathf.Sin(_phi), Mathf.Sin(_theta));
            transform.LookAt(_playerTransform);
        }

        float[] currentFogColor = SubmarineMovement.Instance.GetFogColor();
        _camera.backgroundColor = new Color(currentFogColor[0], currentFogColor[1], currentFogColor[2]);
        if (_lightInitialized)
        {
            _light.intensity = Mathf.Lerp(_minLight, _maxLight, (_playerTransform.position.y - _minHeight) / (_maxHeight - _minLight));
        }
    }

    // Public Methods
    
    // Private Methods
    private void OnPause()
    {
        _paused = true;
    }

    private void OnResume()
    {
        _paused = false;
    }
    
}
