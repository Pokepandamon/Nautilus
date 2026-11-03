
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavVolume;

[RequireComponent(typeof(Fish))]
[RequireComponent(typeof(NavVolumeAgent))]
public class Swordfish : MonoBehaviour
{
    [SerializeField] private float _updateInterval = 2f;
    [SerializeField] private float _targetMoveThreshold = 3f;

    private Fish _fish;
    private NavVolumeAgent _agent;
    private GameObject _submarine;

    private float _timer;
    private Vector3 _lastRequestedPosition;
    private bool _hasRequestedPath;

    private void Awake()
    {
        _fish = GetComponent<Fish>();
        _agent = GetComponent<NavVolumeAgent>();
    }

    private void OnEnable()
    {
        _fish.DetectedSubmarine += OnSubmarineDetected;
        _fish.LostSubmarine += OnSubmarineLost;
    }

    private void OnDisable()
    {
        _fish.DetectedSubmarine -= OnSubmarineDetected;
        _fish.LostSubmarine -= OnSubmarineLost;
    }

    private void Update()
    {
        if (_submarine == null || _agent.NavVolumeSpace == null)
            return;

        _timer += Time.deltaTime;

        if (_timer < _updateInterval)
            return;

        _timer = 0f;

        Vector3 targetPosition = _submarine.transform.position;
        float targetMovement =
            Vector3.Distance(targetPosition, _lastRequestedPosition);

        if (!_hasRequestedPath || targetMovement >= _targetMoveThreshold)
        {
            RequestChasePath(targetPosition);
        }
    }

    private void OnSubmarineDetected(GameObject submarine)
    {
        _submarine = submarine;
        _timer = 0f;
        _hasRequestedPath = false;

        if (_agent.NavVolumeSpace != null)
            RequestChasePath(_submarine.transform.position);
    }

    private void OnSubmarineLost()
    {
        _submarine = null;
        _hasRequestedPath = false;
    }

    private void RequestChasePath(Vector3 destination)
    {
        if (_submarine == null || _agent.NavVolumeSpace == null)
            return;

        _lastRequestedPosition = destination;
        _hasRequestedPath = true;

        _fish.SetDestination(destination);
    }

}
