using UnityEngine;

[RequireComponent(typeof(Fish))]
public class PatrollingFish : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform[] _points;

    private Fish _fish;
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
        if (_points == null || _points.Length == 0)
        {
            return;
        }

        _currentPoint = (_currentPoint + 1) % _points.Length;
    }

    private void OnDetectedSubmarine(GameObject submarine)
    {
        // TODO: run away
        Debug.Log("fish heard submarine");
    }
    
    private void OnLostSubmarine()
    {
        // TODO: return to patrolling
        Debug.Log("fish lost submarine");
    }
}