using UnityEngine;

[RequireComponent(typeof(Fish))]
public class PatrollingFish : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _fleeDistance = 6f;
    [SerializeField] private float _fleeRepathInterval = 0.5f;

    private Fish _fish;
    private Transform _submarine;
    private float _nextFleeRepathTime;
    private int _currentPoint = 0;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _fish = GetComponent<Fish>();
    }

    private void OnEnable()
    {
        _fish.DestinationReached += GoNextPoint;
        _fish.DetectedSubmarine += OnDetectedSubmarine;
        _fish.LostSubmarine += OnLostSubmarine;
    }

    private void OnDisable()
    {
        _fish.DestinationReached -= GoNextPoint;
        _fish.DetectedSubmarine -= OnDetectedSubmarine;
        _fish.LostSubmarine -= OnLostSubmarine;
    }

    private void Update()
    {
        if (_submarine != null)
        {
            if (Time.time >= _nextFleeRepathTime)
            {
                _nextFleeRepathTime = Time.time + _fleeRepathInterval;
                Flee();
            }

            return;
        }

        if (_points == null || _points.Length == 0)
        {
            return;
        }

        Transform destination = _points[_currentPoint];

        if (destination != null)
        {
            _fish.SetDestination(destination.position);
        }
    }

    // Public Methods

    // Private Methods
    private void GoNextPoint()
    {
        if (_submarine != null || _points == null || _points.Length == 0)
        {
            return;
        }

        _currentPoint = (_currentPoint + 1) % _points.Length;
    }

    private void OnDetectedSubmarine(GameObject submarine)
    {
        _submarine = submarine.transform;
        _nextFleeRepathTime = 0f;
    }

    private void OnLostSubmarine()
    {
        _submarine = null;
    }

    private void Flee()
    {
        Vector3 direction = (transform.position - _submarine.position).normalized;
        Vector3 destination = transform.position + direction * _fleeDistance;
        _fish.SetDestination(destination);
    }
}