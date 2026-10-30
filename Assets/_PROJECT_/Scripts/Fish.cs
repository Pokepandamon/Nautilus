using NavVolume;
using UnityEngine;

[RequireComponent(typeof(NavVolumeAgent))]
public class Fish : MonoBehaviour
{
    // Fields
    [SerializeField] private float _detectionRadius;

    protected NavVolumeAgent _agent;
    private Vector3 _lastDestination = Vector3.positiveInfinity;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    protected virtual void Awake()
    {
        _agent = GetComponent<NavVolumeAgent>();
    }

    protected virtual void Update()
    {
        if (Vector3.Distance(transform.position, _lastDestination) < 1f)
        {
            OnReachDestination();
        }
    }

    // Public Methods

    // Private Methods
    protected void SetDestination(Vector3 destination)
    {
        if (_lastDestination != destination)
        {
            _lastDestination = destination;
            _agent.SetDestination(destination);
        }
    }

    protected virtual void OnReachDestination()
    {
        // to be overriden by subclasses
    }
}