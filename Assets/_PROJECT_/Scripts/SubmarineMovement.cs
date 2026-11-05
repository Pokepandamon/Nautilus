using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SubmarineMovement : MonoBehaviour
{
    // Fields
    //[SerializeField] private float _speed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private GameObject _netPrefab;
    [SerializeField] private Transform _netSpawn;
    [SerializeField] private GameObject _harpoonPrefab;
    [SerializeField] private Transform _harpoonSpawn;
    [SerializeField] private SubUpgrades _subUpgrades;
    [SerializeField] private float[] _speedUpgrades;
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
    public int Health = 3;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _rigidbody = GetComponent<Rigidbody>();
        _linearMovementAction = InputSystem.actions.FindAction("ForwardBackUpDown");
        _rotationalMovementAction = InputSystem.actions.FindAction("Move");
        _netAction = InputSystem.actions.FindAction("Attack");
        _harpoonAction = InputSystem.actions.FindAction("RightClick");
    }
    
    private void Update()
    {
        _linearMovement = _linearMovementAction.ReadValue<Vector2>();
        _rotationalMovement = _rotationalMovementAction.ReadValue<Vector2>().x;
        if (_netAction.WasPressedThisFrame())
        {
            Instantiate(_netPrefab, _netSpawn);
        }

        if (_harpoonAction.WasPressedThisFrame() && _currentHarpoon == null)
        {
           _currentHarpoon = Instantiate(_harpoonPrefab, _harpoonSpawn);
           _currentHarpoon.GetComponent<Harpoon>().SetSub(gameObject);
           _currentHarpoon.transform.SetParent(null);
           _currentHarpoonSource = _currentHarpoon.GetComponent<Harpoon>().GetRopeSource();
           _currentHarpoonSource.transform.SetParent(_harpoonSpawn);
        }
    }
    
    private void FixedUpdate()
    {
        _rigidbody.AddRelativeForce(_linearMovement.y * _speedUpgrades[_subUpgrades.Speed], _linearMovement.x * _speedUpgrades[_subUpgrades.Speed], 0f);
        _rigidbody.AddTorque(0f, _rotationalMovement * _turnSpeed, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hurts"))
        {
            Debug.Log("Ow");
            Health -= 1;
            Destroy(other.gameObject);
        }
    }

    // Public Methods
    public Transform GetHarpoonSpawn()
    {
        return _harpoonSpawn;
    }
    
    // Private Methods
    
}
