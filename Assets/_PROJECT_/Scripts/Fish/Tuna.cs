
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavVolume;

[RequireComponent(typeof(Fish))]
public class Tuna : MonoBehaviour
{
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _updateInterval = 1.5f;

    private Fish _fish;
    private NavVolumeAgent _agent;
    private GameObject _submarine;
    private float _timer;

    private void Awake()
    {
        _fish = GetComponent<Fish>();
        _agent = GetComponent<NavVolumeAgent>();
    }

    private void OnEnable()
    {
        _fish.DetectedSubmarine += SwimAway;
        _fish.LostSubmarine += StopSwimming;
    }

    private void OnDisable()
    {
        _fish.DetectedSubmarine -= SwimAway;
        _fish.LostSubmarine -= StopSwimming;
    }

    private void Update()
    {
        if (_submarine == null) return;

        _timer += Time.deltaTime;

        if (_timer >= _updateInterval)
        {
            _timer = 0f;
            SetEscapeDestination();
        }
    }

    private void SwimAway(GameObject submarine)
    {
        _submarine = submarine;
        _timer = 0f;
        SetEscapeDestination();
    }

    private void SetEscapeDestination()
    {
        if (_submarine == null || _agent.NavVolumeSpace == null)
            return;

        Vector3 direction = transform.position - _submarine.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        // Try several escape directions, starting directly away.
        for (int i = 0; i < 8; i++)
        {
            float angle = i == 0 ? 0f : (i % 2 == 1 ? 45f : -45f) * ((i + 1) / 2);

            Vector3 candidateDirection =
                Quaternion.Euler(0f, angle, 0f) * direction;

            Vector3 destination =
                transform.position + candidateDirection * _distance;

            destination.y = transform.position.y;

            if (_agent.NavVolumeSpace.IsSegmentNavigable(
                transform.position, destination))
            {
                _fish.SetDestination(destination);
                Debug.Log("Tuna escape destination: " + destination);
                return;
            }
        }

        Debug.LogWarning("Tuna couldn't find a valid escape direction.");
    }

    private void StopSwimming()
    {
        _submarine = null;
    }
    
}
