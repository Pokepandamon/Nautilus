using System;
using NavVolume;
using UnityEngine;

[RequireComponent(typeof(NavVolumeAgent))]
public class Fish : MonoBehaviour
{
    // Fields
    [SerializeField] private float _baseDetectionRadius;
    [SerializeField] private SubUpgrades _subUpgrades;

    private NavVolumeAgent _agent;
    private Vector3 _lastDestination = Vector3.positiveInfinity;
    private bool _hasDestination = false;
    private bool _isSubmarineCurrentlyDetected = false;

    // Properties

    // Events / Delegates
    public event Action DestinationReached;
    public event Action<GameObject> DetectedSubmarine;
    public event Action LostSubmarine;

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _agent = GetComponent<NavVolumeAgent>();
    }

    private void Update()
    {
        CheckForSubmarine();

        if (_hasDestination && Vector3.Distance(transform.position, _lastDestination) < 1f)
        {
            _hasDestination = false;
            DestinationReached?.Invoke();
        }
    }

    // Public Methods
    public void SetDestination(Vector3 destination)
    {
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

    // Private Methods
    private void CheckForSubmarine()
    {
        float radius = _baseDetectionRadius * (6f - _subUpgrades.Stealth) / 6f;

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