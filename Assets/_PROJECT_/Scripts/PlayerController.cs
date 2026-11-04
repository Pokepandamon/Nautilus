using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5f; 

    private Rigidbody _playerRigidbody;
    private InputAction _moveAction;
    private Vector2 _movement;

    void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }
    
    void Update()
    {
        _movement = _moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 movement3D = new Vector3(_movement.x, 0f, _movement.y);
        _playerRigidbody.linearVelocity = movement3D * _speed;
    }
}