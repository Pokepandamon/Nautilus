using NavVolume;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float _defensiveDistance = 3.0f;
    [SerializeField] private float _speed;
    [SerializeField] private float _timeTillTire;
    [SerializeField] private float _speedRecoveryRate;
    [SerializeField] private Stage1BossSchool _school;
    [SerializeField] private float _detectionDistance;
    [SerializeField] private float _communicationDistance;
    [SerializeField] private float _looseDistance;
    [SerializeField] private float _minimumSpeedModifier;
    [SerializeField] private float _disengageDistance;
    [SerializeField] private float _shotCooldown;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawn;
    
    private Fish _fish;
    private Transform _submarine;
    private float _nextFleeRepathTime;
    private Vector3 _currentPatrolTarget;
    private float _fleeTime;
    private NavVolumeAgent _navVolumeAgent;
    float _speedModifier = 1.0f;
    private float _currentShotCooldown;
    private float _effectiveCommunicationDistance;
    private float _effectiveDetectionDistance;
    private float _effectiveDisengageDistance;
    
    public Stage1BossStates CurrentState = Stage1BossStates.Patrolling;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _fish = GetComponent<Fish>();
        _navVolumeAgent = GetComponent<NavVolumeAgent>();
        _navVolumeAgent._speed = _speed;
        _speedModifier = 1.0f;
        _currentShotCooldown = _shotCooldown;
    }

    private void Start()
    {
        _submarine = SubmarineMovement.Instance.transform;
    }

    private void FixedUpdate()
    {
        ChangeState();
        _navVolumeAgent._speed = _speed * _speedModifier;
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
            if (subDistance <= _detectionDistance)
            {
                bool foundAlly = false;
                foreach (Stage1Boss member in _school.GetSchoolStage1Bosses())
                {
                    if (Vector3.Distance(transform.position, member.transform.position) <= _communicationDistance)
                    {
                        member.CurrentState = Stage1BossStates.Pursuing;
                        foundAlly = true;
                    }
                }
                
                if (!foundAlly)
                {
                    CurrentState = Stage1BossStates.Fleeing;
                }else{
                    CurrentState = Stage1BossStates.Pursuing;
                }
            }
        }else if (CurrentState == Stage1BossStates.Pursuing)
        {
            if (subDistance > _looseDistance || Vector3.Distance(transform.position, _currentPatrolTarget) > _disengageDistance)
            {
                CurrentState = Stage1BossStates.Patrolling;
            }
        }else if (CurrentState == Stage1BossStates.Fleeing)
        {
            if (subDistance <= _looseDistance)
            {
                // 10 is a placeholder for how many seconds we want to have them flee for before tiring
                if (_fleeTime > _timeTillTire)
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
            if (subDistance <= _looseDistance)
            {
                // 10 is a placeholder for the threat distance when they turn around and engage defences and atatack
                if (subDistance <= _defensiveDistance)
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
            if (subDistance > _looseDistance || Vector3.Distance(transform.position, _currentPatrolTarget) > _disengageDistance)
            {
                CurrentState = Stage1BossStates.Patrolling;
            }
        }
    }

    public float DistanceToSub()
    {
        return Vector3.Magnitude(transform.position - _submarine.position);
    }

    private void PatrollingUpdate()
    {
        _speedModifier = Mathf.Clamp(_speedModifier + Time.deltaTime * _speedRecoveryRate / 2, 0, 1f);
        _fish.SetDestination(_currentPatrolTarget);
        _fleeTime = Mathf.Clamp(_fleeTime - Time.deltaTime / 2, 0f, _fleeTime);
        //Debug.Log(_fish.GetLastDestination());
    }

    private void PursuingUpdate()
    {
        Vector3 direction = (transform.position - _submarine.position).normalized;
        Vector3 destination = _submarine.position + direction * _defensiveDistance;
        _fish.SetDestination(destination);
        transform.LookAt(_submarine);
        _currentShotCooldown = Mathf.Clamp(_currentShotCooldown - Time.deltaTime, 0f, _currentShotCooldown);
        if (_currentShotCooldown == 0f)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawn);
            projectile.transform.SetParent(null);
            projectile.transform.position = _projectileSpawn.position;
            _currentShotCooldown = _shotCooldown;
        }
    }

    private void FleeingUpdate()
    {
        Vector3 direction = (transform.position - _submarine.position).normalized;
        Vector3 destination = transform.position + direction * _fleeDistance;
        _fish.SetDestination(destination);
        _fleeTime += Time.deltaTime;
    }

    private void TiringUpdate()
    {
        _speedModifier = Mathf.Clamp(_speedModifier - Time.deltaTime * _speedRecoveryRate, _minimumSpeedModifier, 1f);
        Vector3 direction = (transform.position - _submarine.position).normalized;
        Vector3 destination = transform.position + direction * _fleeDistance;
        _fish.SetDestination(destination);
    }

    private void DefensiveUpdate()
    {
        Vector3 direction = (transform.position - _submarine.position).normalized;
        Vector3 destination = _submarine.position + direction * _defensiveDistance;
        _fish.SetDestination(destination);
        transform.LookAt(_submarine);
        _currentShotCooldown = Mathf.Clamp(_currentShotCooldown - Time.deltaTime, 0f, _currentShotCooldown);
        if (_currentShotCooldown == 0f)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _projectileSpawn);
            projectile.transform.SetParent(null);
            projectile.transform.position = _projectileSpawn.position;
            _currentShotCooldown = _shotCooldown;
        }
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