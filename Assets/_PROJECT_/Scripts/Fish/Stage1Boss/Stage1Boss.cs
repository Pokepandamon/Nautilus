using UnityEngine;

[RequireComponent(typeof(Fish))]
public class Stage1Boss : MonoBehaviour
{
    public enum Stage1BossStates
    {
        Patrolling,
        Pursuing,
        Fleeing,
        Tiring,
        Defensive
    }
    
    // Fields
    [SerializeField] private float _fleeDistance = 6f;
    [SerializeField] private float _fleeRepathInterval = 0.5f;
    [SerializeField] private GameObject[] _school;

    private Fish _fish;
    private Transform _submarine;
    private float _nextFleeRepathTime;
    private Stage1Boss[] _schoolBoss;
    private Vector3 _currentPatrolTarget;
    private float _fleeTime;
    
    public Stage1BossStates CurrentState = Stage1BossStates.Patrolling;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _fish = GetComponent<Fish>();
    }

    private void FixedUpdate()
    {
        // ChangeState();
        if (CurrentState == Stage1BossStates.Patrolling)
        {
            PatrollingUpdate();
        }else if (CurrentState == Stage1BossStates.Pursuing)
        {
            PursuingUpdate();
        }else if (CurrentState == Stage1BossStates.Fleeing)
        {
            FleeingUpdate();
        }else if (CurrentState == Stage1BossStates.Tiring)
        {
            TiringUpdate();
        }
        else
        {
            DefensiveUpdate();
        }
    }

    // Public Methods
    public void SetPatrolTarget(Vector3 target)
    {
        _currentPatrolTarget = target;
    }

    // Private Methods
    private void ChangeState()
    {
        float subDistance = DistanceToSub();
        if (CurrentState == Stage1BossStates.Patrolling)
        {
            if (subDistance <= _fish.GetDetectionRadius())
            {
                bool foundFleeing = false;
                foreach (Stage1Boss member in _schoolBoss)
                {
                    if (member.CurrentState == Stage1BossStates.Fleeing)
                    {
                        CurrentState = Stage1BossStates.Pursuing;
                        foundFleeing = true;
                    }
                }
                
                if (!foundFleeing)
                {
                    CurrentState = Stage1BossStates.Fleeing;
                }
            }
        }else if (CurrentState == Stage1BossStates.Pursuing)
        {
            if (subDistance > _fish.GetDetectionRadius())
            {
                CurrentState = Stage1BossStates.Patrolling;
            }
        }else if (CurrentState == Stage1BossStates.Fleeing)
        {
            if (subDistance <= _fish.GetDetectionRadius())
            {
                // 10 is a placeholder for how many seconds we want to have them flee for before tiring
                if (_fleeTime > 10f)
                {
                    CurrentState = Stage1BossStates.Tiring;
                }
            }
            else
            {
                CurrentState = Stage1BossStates.Patrolling;
            }
        }else if (CurrentState == Stage1BossStates.Tiring)
        {
            if (subDistance <= _fish.GetDetectionRadius())
            {
                // 10 is a placeholder for the threat distance when they turn around and engage defences and atatack
                if (subDistance <= 10f)
                {
                    CurrentState = Stage1BossStates.Defensive;
                }
                else
                {
                    CurrentState = Stage1BossStates.Tiring;
                }
            }
            else
            {
                CurrentState = Stage1BossStates.Patrolling;
            }
        }
        else if(CurrentState == Stage1BossStates.Defensive)
        {
            if (subDistance > _fish.GetDetectionRadius())
            {
                CurrentState = Stage1BossStates.Patrolling;
            }
        }
    }

    private float DistanceToSub()
    {
        return Vector3.Magnitude(transform.position - SubmarineMovement.Instance.gameObject.transform.position);
    }

    private void PatrollingUpdate()
    {
        _fish.SetDestination(_currentPatrolTarget);
        //Debug.Log(_fish.GetLastDestination());
    }

    private void PursuingUpdate()
    {
        
    }

    private void FleeingUpdate()
    {
        
    }

    private void TiringUpdate()
    {
        
    }

    private void DefensiveUpdate()
    {
        
    }
    
    /*private void GoNextPoint()
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
    }*/
}