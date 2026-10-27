using System;
using System.Numerics;
using NavVolume;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FishPatrol : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform[] points;

    private NavVolumeAgent _agent;
    private int _currentPoint = 0;
    private Vector3 _lastDestination = Vector3.positiveInfinity;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _agent = GetComponent<NavVolumeAgent>();
    }

    private void Update()
    {
        Transform destination = points[_currentPoint];
        if (destination == null)
        {
            Debug.Log("dest is null");
            return;
        }

        if (_lastDestination != destination.position)
        {
            _lastDestination = destination.position;
            _agent.SetDestination(_lastDestination);
            Debug.Log("set dest");
        }

        if (Vector3.Distance(transform.position, destination.position) < 0.5f)
        {
            GoNextPoint();
        }
    }

    // Public Methods

    // Private Methods
    private void GoNextPoint()
    {
        if (points.Length == 0)
        {
            return;
        }

        _currentPoint = (_currentPoint + 1) % points.Length;
    }
}