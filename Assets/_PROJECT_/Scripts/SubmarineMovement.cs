
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SubmarineMovement : MonoBehaviour
{
    // Fields

    [SerializeField] private float _turnSpeed;
    [SerializeField] private GameObject _netPrefab;
    [SerializeField] private Transform _netSpawn;
    [SerializeField] private GameObject _harpoonPrefab;
    [SerializeField] private Transform _harpoonSpawn;
    [SerializeField] private SubUpgrades _subUpgrades;
    [SerializeField] private float[] _speedUpgrades;
    
    // 0: Kelp
    // 1: Sandy Dunes
    // 2: Barnacle Gardens
    [SerializeField] private float[] _fogValues;
    [SerializeField] private Color[] _fogColors;
    [SerializeField] private float _fogAdjustmentTime;


    private Rigidbody _rigidbody;
    private InputAction _linearMovementAction;
    private InputAction _rotationalMovementAction;
    private InputAction _netAction;
    private InputAction _harpoonAction;

    private Vector2 _linearMovement;
    private float _rotationalMovement;

    private GameObject _currentHarpoon;
    private GameObject _currentHarpoonSource;

    public static SubmarineMovement Instance;

    private SubmarineHealth _submarineHealth;

    private float _currentFogDensity;
    private float _targetFogDensity;
    private float _currentFogDensityDelta;
    private float[] _currentFogColor = new float[3];
    private Color _targetFogColor;
    private float[] _currentFogColorDelta = new float[3];
    
    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _rigidbody = GetComponent<Rigidbody>();

        _linearMovementAction =
            InputSystem.actions.FindAction("ForwardBackUpDown");

        _rotationalMovementAction =
            InputSystem.actions.FindAction("Move");

        _netAction =
            InputSystem.actions.FindAction("Attack");

        _harpoonAction =
            InputSystem.actions.FindAction("RightClick");

        // Fix: Re-enable turning after returning from the Village.
        if (_rotationalMovementAction != null)
        {
            _rotationalMovementAction.Enable();
        }
        else
        {
            Debug.LogError("SubmarineMovement: Move input action not found!");
        }

        _submarineHealth = GetComponent<SubmarineHealth>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        _currentFogDensity = _fogValues[0];
        _targetFogDensity = _fogValues[0];
        _currentFogColor[0] = _fogColors[0].r;
        _currentFogColor[1] = _fogColors[0].g;
        _currentFogColor[2] = _fogColors[0].b;
        _targetFogColor = _fogColors[0];
    }

    private void Update()
    {
        _linearMovement = _linearMovementAction.ReadValue<Vector2>();

        if (_rotationalMovementAction != null)
        {
            _rotationalMovement =
                _rotationalMovementAction.ReadValue<Vector2>().x;
        }

        if (_netAction.WasPressedThisFrame())
        {
            Instantiate(_netPrefab, _netSpawn);
        }

        if (_harpoonAction.WasPressedThisFrame() && _currentHarpoon == null)
        {
            _currentHarpoon = Instantiate(_harpoonPrefab, _harpoonSpawn);

            _currentHarpoon.GetComponent<Harpoon>().SetSub(gameObject);

            _currentHarpoon.transform.SetParent(null);

            _currentHarpoonSource =
                _currentHarpoon.GetComponent<Harpoon>().GetRopeSource();

            _currentHarpoonSource.transform.SetParent(_harpoonSpawn);
        }

        _currentFogDensity = _targetFogDensity - _currentFogDensity > 0 ? Mathf.Clamp(_currentFogDensity + _currentFogDensityDelta * Time.deltaTime, _currentFogDensity, _targetFogDensity) : Mathf.Clamp(_currentFogDensity - _currentFogDensityDelta * Time.deltaTime, _targetFogDensity, _currentFogDensity);
        _currentFogColor[0] = _targetFogColor.r - _currentFogColor[0] > 0
            ? Mathf.Clamp(_currentFogColor[0] + _currentFogColorDelta[0] * Time.deltaTime, _currentFogColor[0], _targetFogColor.r)
            : Mathf.Clamp(_currentFogColor[0] - _currentFogColorDelta[0] * Time.deltaTime, _targetFogColor.r, _currentFogColor[0]);
        _currentFogColor[1] = _targetFogColor.g - _currentFogColor[1] > 0
            ? Mathf.Clamp(_currentFogColor[1] + _currentFogColorDelta[1] * Time.deltaTime, _currentFogColor[1], _targetFogColor.g)
            : Mathf.Clamp(_currentFogColor[1] - _currentFogColorDelta[1] * Time.deltaTime, _targetFogColor.g, _currentFogColor[1]);
        _currentFogColor[2] = _targetFogColor.b - _currentFogColor[2] > 0
                ? Mathf.Clamp(_currentFogColor[2] + _currentFogColorDelta[2] * Time.deltaTime, _currentFogColor[2], _targetFogColor.b)
                : Mathf.Clamp(_currentFogColor[2] - _currentFogColorDelta[2] * Time.deltaTime, _targetFogColor.b, _currentFogColor[2]);
        RenderSettings.fogDensity = _currentFogDensity;
        Color newFogColor = new Color(_currentFogColor[0], _currentFogColor[1], _currentFogColor[2]);
        RenderSettings.fogColor = newFogColor;
        
        // Debug.Log(_currentFogColor[0] + ", " + _currentFogColor[1] + ", "+ _currentFogColor[2]);
        // Debug.Log(RenderSettings.fogDensity + " : " + _currentFogDensity);

    }

    private void FixedUpdate()
    {
        _rigidbody.AddRelativeForce(
            _linearMovement.y * _speedUpgrades[_subUpgrades.Speed],
            _linearMovement.x * _speedUpgrades[_subUpgrades.Speed],
            0f
        );

        _rigidbody.AddTorque(
            0f,
            _rotationalMovement * _turnSpeed,
            0f
        );
    }

    private void OnCollisionEnter(Collision other)
    {
            Debug.Log("Ow");
            if(_submarineHealth != null){
                _submarineHealth.TakeDamage(1);
            }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KelpForest"))
        {
            Debug.Log("Entered the Kelp Forest");
            _targetFogDensity = _fogValues[0];
            _currentFogDensityDelta = Mathf.Abs(_targetFogDensity - _currentFogDensity) / _fogAdjustmentTime;
            _targetFogColor = _fogColors[0];
            _currentFogColorDelta[0] = Mathf.Abs(_targetFogColor.r - _currentFogColor[0]) / _fogAdjustmentTime;
            _currentFogColorDelta[1] = Mathf.Abs(_targetFogColor.g - _currentFogColor[1]) / _fogAdjustmentTime;
            _currentFogColorDelta[2] = Mathf.Abs(_targetFogColor.b - _currentFogColor[2]) / _fogAdjustmentTime;
        }else if (other.CompareTag("SandyRocks"))
        {
            _targetFogDensity = _fogValues[1];
            _currentFogDensityDelta = Mathf.Abs(_targetFogDensity - _currentFogDensity) / _fogAdjustmentTime;
            _targetFogColor = _fogColors[1];
            _currentFogColorDelta[0] = Mathf.Abs(_targetFogColor.r - _currentFogColor[0]) / _fogAdjustmentTime;
            _currentFogColorDelta[1] = Mathf.Abs(_targetFogColor.g - _currentFogColor[1]) / _fogAdjustmentTime;
            _currentFogColorDelta[2] = Mathf.Abs(_targetFogColor.b - _currentFogColor[2]) / _fogAdjustmentTime;
            Debug.Log("Entered Sandy Rocks");
        }else if (other.CompareTag("BarnacleGarden"))
        {
            _targetFogDensity = _fogValues[2];
            _currentFogDensityDelta = Mathf.Abs(_targetFogDensity - _currentFogDensity) / _fogAdjustmentTime;
            _targetFogColor = _fogColors[2];
            _currentFogColorDelta[0] = Mathf.Abs(_targetFogColor.r - _currentFogColor[0]) / _fogAdjustmentTime;
            _currentFogColorDelta[1] = Mathf.Abs(_targetFogColor.g - _currentFogColor[1]) / _fogAdjustmentTime;
            _currentFogColorDelta[2] = Mathf.Abs(_targetFogColor.b - _currentFogColor[2]) / _fogAdjustmentTime;
            Debug.Log("Entered the Barnacle Garden");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Public Methods

    public Transform GetHarpoonSpawn()
    {
        return _harpoonSpawn;
    }

    public GameObject GetHarpoon()
    {
        return _currentHarpoon;
    }

    public float[] GetFogColor()
    {
        return _currentFogColor;
    }

    // Private Methods
}
