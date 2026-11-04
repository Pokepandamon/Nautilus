using System;
using NavVolume;
using UnityEngine;

[RequireComponent(typeof(NavVolumeAgent))]
public class Fish : MonoBehaviour
{
    // Fields
    [SerializeField] private FishData _fishData;
    [SerializeField] private float _baseDetectionRadius;
    [SerializeField] private SubUpgrades _subUpgrades;
    [SerializeField] private DebugFlags _debugFlags;

    private NavVolumeAgent _agent;
    private Rigidbody _rb;
    private bool _wasKinematic;
    private Vector3 _lastDestination = Vector3.positiveInfinity;
    private bool _hasDestination = false;
    private bool _isSubmarineCurrentlyDetected = false;

    // Properties
    public FishData FishData => _fishData;
    public bool IsHooked { get; private set; }

    // Events / Delegates
    public event Action DestinationReached;
    public event Action<GameObject> DetectedSubmarine;
    public event Action LostSubmarine;
    public event Action Hooked;

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _agent = GetComponent<NavVolumeAgent>();
        _rb = GetComponent<Rigidbody>(); // may be null, that's fine
    }

    private void Update()
    {
        if (IsHooked) return; // frozen: no detection, no destination events

        CheckForSubmarine();

        if (_hasDestination && Vector3.Distance(transform.position, _lastDestination) < 1f)
        {
            _hasDestination = false;
            DestinationReached?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (_debugFlags.ShowDetectionRadiusSpheres)
        {
            float radius = _baseDetectionRadius * (6f - _subUpgrades.Stealth) / 6f;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    // Public Methods
    public void SetDestination(Vector3 destination)
    {
        if (IsHooked) return;

        NavVolumeSpace space = _agent.NavVolumeSpace;

        if (space == null || !space.IsReady)
        {
            return;
        }

        if (!space.IsNavigable(destination)
            && !space.TrySnapToNavigable(destination, 10f, out destination))
        {
            return;
        }

        if (_lastDestination != destination)
        {
            _lastDestination = destination;
            _hasDestination = true;
            _agent.SetDestination(destination);
        }
    }

    public void Hook()
    {
        if (IsHooked) return;
        IsHooked = true;

        _hasDestination = false;
        _lastDestination = Vector3.positiveInfinity; // so the first order after release isn't ignored

        _agent.enabled = false; // stop navigation

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;   // zero BEFORE going kinematic
            _rb.angularVelocity = Vector3.zero;
            _wasKinematic = _rb.isKinematic;
            _rb.isKinematic = true;
        }

        Hooked?.Invoke(); // lets other scripts (AI, animation) freeze themselves
    }

    public void Release()
    {
        if (!IsHooked) return;
        IsHooked = false;

        if (_rb != null) _rb.isKinematic = _wasKinematic;

        _isSubmarineCurrentlyDetected = false; // re-detect cleanly after release
        _agent.enabled = true;
    }

    public float GetDetectionRadius()
    {
        return _baseDetectionRadius * (6f - _subUpgrades.Stealth) / 6f;
    }

    public bool GetHasDestination()
    {
        return _hasDestination;
    }

    public Vector3 GetLastDestination()
    {
        return _lastDestination;
    }

    // Private Methods
    private void CheckForSubmarine()
    {
        float radius = GetDetectionRadius();

        // TODO: look into using layers and LayerMask.GetMask("Submarine")
        // might speed up check?
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Submarine"))
            {
                if (!_isSubmarineCurrentlyDetected)
                {
                    _isSubmarineCurrentlyDetected = true;
                    DetectedSubmarine?.Invoke(collider.gameObject);
                }

                return;
            }
        }

        if (_isSubmarineCurrentlyDetected)
        {
            _isSubmarineCurrentlyDetected = false;
            LostSubmarine?.Invoke();
        }
    }
}
