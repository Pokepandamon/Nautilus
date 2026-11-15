using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Fish))]
public class Clam : MonoBehaviour
{
    private enum ShellState
    {
        Opening,
        Closing,
        Open,
        Closed
    }

    // Fields
    [SerializeField] private GameObject _topShell;
    [SerializeField] private GameObject _bottomShell;
    [SerializeField] private GameObject _clamParent;
    [SerializeField] private Transform _hinge;
    [SerializeField] private Transform _netChecker;
    [SerializeField] private LayerMask _shellMask;
    [SerializeField] private float _closingTime;
    private Fish _fish;
    private Vector3 _initialPosition;
    private ShellState _shellState = ShellState.Opening;
    private float _shellAngle = 0;
    private float _lastShellUpdate;
    private float _shellSpeedModifier = 1;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)

    private void Awake()
    {
        _fish = GetComponent<Fish>();
        _fish.AutoCollectInNet = false; // clam handles its own collection logic
        _initialPosition = transform.position;
    }

    private void Start()
    {
        StartCoroutine(ShellChange());
    }

    private void OnEnable()
    {
        _fish.DetectedHarpoon += OnDetectedHarpoon;
        _fish.LostHarpoon += OnLostHarpoon;
    }

    private void OnDisable()
    {
        _fish.DetectedHarpoon -= OnDetectedHarpoon;
        _fish.LostHarpoon -= OnLostHarpoon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_fish.IsCatchable && other.gameObject.CompareTag("Net"))
        {
            //Debug.Log(other.gameObject.GetComponentInParent<Transform>().position);
            Physics.queriesHitBackfaces = true;
            //Debug.DrawLine(_netChecker.position, _subNetSpawn.position, Color.red, 10f);
            if (!Physics.Linecast(_netChecker.position, SubmarineMovement.Instance.GetHarpoonSpawn().position,
                    _shellMask))
            {
                //Debug.Log("Nothing in between ");
                _fish.Collect();

                Destroy(_clamParent);
            }
        }
    }

    private void Update()
    {
        if (_fish.IsSubmarineDetected || _fish.IsHarpoonDetected)
        {
            if (_shellState != ShellState.Closed)
            {
                _shellState = ShellState.Closing;
            }
        }
        else
        {
            if (_shellState != ShellState.Open)
            {
                _shellState = ShellState.Opening;
            }
        }
    }

    private void LateUpdate()
    {
        // I don't know why but it keeps wanting to move when the sub gets too close
        transform.position = _initialPosition;
    }

    // Public Methods

    // Private Methods
    private void OnDetectedHarpoon(GameObject harpoon)
    {
        _shellSpeedModifier = 2;
    }

    private void OnLostHarpoon()
    {
        _shellSpeedModifier = 1;
    }

    /*private IEnumerator OpenShell()
    {
        for (int i = 0; i < 100; i++)
        {
            _topShell.transform.RotateAround(_hinge.position, Vector3.forward, 90f/100f);
            yield return new WaitForSeconds(0.01f);
        }
        _shellChanging = false;
    }

    private IEnumerator CloseShell()
    {
        for (int i = 0; i < 100; i++)
        {
            _topShell.transform.RotateAround(_hinge.position, Vector3.forward, -90f/100f);
            yield return new WaitForSeconds(0.01f);
        }
        _shellChanging = false;
    }*/

    private IEnumerator ShellChange()
    {
        while (isActiveAndEnabled)
        {
            if (_shellState == ShellState.Closing)
            {
                if (_shellAngle <= 0)
                {
                    _shellState = ShellState.Closed;
                }
                else
                {
                    float angleDelta =
                        Mathf.Clamp(
                            90f * ((Time.realtimeSinceStartup - _lastShellUpdate) /
                                   (_closingTime / _shellSpeedModifier)), 0f, _shellAngle);
                    _topShell.transform.RotateAround(_hinge.position, Vector3.forward, -angleDelta);
                    _shellAngle -= angleDelta;
                }
            }

            if (_shellState == ShellState.Opening)
            {
                if (_shellAngle >= 90)
                {
                    _shellState = ShellState.Open;
                }
                else
                {
                    float angleDelta =
                        Mathf.Clamp(
                            90f * ((Time.realtimeSinceStartup - _lastShellUpdate) /
                                   (_closingTime / _shellSpeedModifier)), 0f, 90f - _shellAngle);
                    _topShell.transform.RotateAround(_hinge.position, Vector3.forward, angleDelta);
                    _shellAngle += angleDelta;
                }
            }

            _lastShellUpdate = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(0.02f);
        }
    }
}