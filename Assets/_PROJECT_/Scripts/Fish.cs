using System;
using NavVolume;
using UnityEngine;

[RequireComponent(typeof(NavVolumeAgent))]
public class Fish : MonoBehaviour
{
    // Fields
    [SerializeField] private float _detectionRadius;

    private NavVolumeAgent _agent;
    private Vector3 _lastDestination = Vector3.positiveInfinity;
    private bool _hasDestination;

    // Properties

    // Events / Delegates
    public event Action DestinationReached;

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _agent = GetComponent<NavVolumeAgent>();
    }

    private void Update()
    {
        if (_hasDestination && Vector3.Distance(transform.position, _lastDestination) < 1f)
        {
            _hasDestination = false;
            DestinationReached?.Invoke();
        }
    }

    // Public Methods
    public void SetDestination(Vector3 destination)
    {
        if (_lastDestination != destination)
        {
            _lastDestination = destination;
            _hasDestination = true;
            _agent.SetDestination(destination);
        }
    }

    // Private Methods
}