using UnityEngine;

public class FishPatrol : Fish
{
    // Fields
    [SerializeField] private Transform[] _points;

    private int _currentPoint = 0;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    protected override void Update()
    {
        base.Update();
        
        Transform destination = _points[_currentPoint];

        if (destination != null)
        {
            SetDestination(destination.position);
        }
    }

    // Public Methods

    // Private Methods
    protected override void OnReachDestination()
    {
        GoNextPoint();
    }

    private void GoNextPoint()
    {
        if (_points.Length == 0)
        {
            return;
        }

        _currentPoint = (_currentPoint + 1) % _points.Length;
    }
}