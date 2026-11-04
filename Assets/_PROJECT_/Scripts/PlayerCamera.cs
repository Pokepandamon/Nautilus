using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerBody;

    [Header("Mouse Look")]
    [SerializeField] private InputActionReference _look;
    [SerializeField, Min(0.01f)] private float _mouseSensitivity = 180f;
    [SerializeField] private float _minimumPitch = -80f;
    [SerializeField] private float _maximumPitch = 80f;

    private float _pitch;

    private void Start()
    {
        _pitch = NormalizeAngle(transform.localEulerAngles.x);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        //float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
        Vector2 mouseDelta = _look.action.ReadValue<Vector2>() * _mouseSensitivity * Time.deltaTime;

        // Rotate the player left and right.
        _playerBody.Rotate(Vector3.up * mouseDelta.x);

        // Rotate the camera up and down.
        _pitch = Mathf.Clamp(_pitch - mouseDelta.y, _minimumPitch, _maximumPitch);
        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private static float NormalizeAngle(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}

