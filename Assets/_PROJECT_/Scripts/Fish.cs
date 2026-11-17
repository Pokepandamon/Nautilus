using System;
using NavVolume;
using UnityEngine;

// TODO: add patrolling, maybe chasing too, depends if its useful to other fish or if they're implementing their own logic
public enum FishMode
{
    Custom,
    Roaming
}

[RequireComponent(typeof(NavVolumeAgent))]
public class Fish : MonoBehaviour
{
    // Fields
    public FishMode Mode = FishMode.Custom;
    public bool IsCatchable = true;
    public bool AutoCollectInNet = true; // some fish (clams) do their own checks

    [SerializeField] private FishData _fishData;
    [SerializeField] private float[] _detectionRadius = { 0, 0, 0, 0, 0, 0, 0 };
    [SerializeField] private float[] _harpoonDetectionRadius = { 0, 0, 0, 0, 0, 0, 0 };
    [SerializeField] private float _maxRoamDistance = 10f;
    [SerializeField] private SubUpgrades _subUpgrades;
    [SerializeField] private DebugFlags _debugFlags;

    private NavVolumeAgent _agent;
    private Rigidbody _rb;
    private bool _wasKinematic;
    private Vector3 _lastDestination = Vector3.positiveInfinity;
    private Vector3 _attemptingRoamingDestination = Vector3.positiveInfinity;
    private bool _hasDestination = false;
    private Vector3 _initialPosition;

    // Properties
    public FishData FishData => _fishData;
    public bool IsHooked { get; private set; }
    public bool IsSubmarineDetected { get; private set; } = false;
    public bool IsHarpoonDetected { get; private set; } = false;

    // Events / Delegates
    public event Action DestinationReached;
    public event Action<GameObject> DetectedSubmarine;
    public event Action LostSubmarine;
    public event Action<GameObject> DetectedHarpoon;
    public event Action LostHarpoon;
    public event Action Hooked;

    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _agent = GetComponent<NavVolumeAgent>();
        _rb = GetComponent<Rigidbody>(); // may be null, that's fine
        _initialPosition = transform.position;
    }

    private void Start()
    {
        if (_agent.NavVolumeSpace == null || !_agent.NavVolumeSpace.IsReady)
        {
            Debug.LogError("Fish requires a NavVolumeSpace. Disabling...");
            enabled = false;
        }
    }

    private void Update()
    {
        if (IsHooked) return; // frozen: no detection, no destination events

        CheckForSubmarine();
        CheckForHarpoon();

        if (_hasDestination && Vector3.Distance(transform.position, _lastDestination) < 1f)
        {
            _hasDestination = false;
            DestinationReached?.Invoke(); // TODO: only invoke if patrolling mode
        }

        if (Mode == FishMode.Roaming)
        {
            int tryCount = 0;

            while (!_hasDestination)
            {
                Vector3 destination = GetAlmostRandomDestination();

                // retry if raycast goes through ground
                if (Physics.Raycast(transform.position, destination - transform.position, out var hitInfo,
                        Vector3.Distance(transform.position, destination)))
                {
                    tryCount++;

                    if (tryCount >= 5)
                    {
                        destination = Vector3.Lerp(transform.position, hitInfo.point, 0.8f);
                    }
                    else
                    {
                        continue;
                    }
                }

                bool success = SetDestination(destination);
                _attemptingRoamingDestination = destination;

                if (!success)
                {
                    Debug.Log("failed to set destination while roaming");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (AutoCollectInNet && IsCatchable && other.CompareTag("Net"))
        {
            Collect();
        }
    }

    private void OnDrawGizmos()
    {
        if (Mode == FishMode.Roaming && !IsHooked && _attemptingRoamingDestination != Vector3.positiveInfinity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _attemptingRoamingDestination);
            Gizmos.DrawWireSphere(_attemptingRoamingDestination, 0.5f);
        }

        if (_debugFlags.ShowDetectionRadiusSpheres)
        {
            float radius = _detectionRadius[_subUpgrades.Stealth];
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    // Public Methods
    public void Collect()
    {
        if (_fishData != null)
        {
            InventoryManager.instance.AddFish(_fishData);
        }

        Destroy(gameObject);
    }

    public bool SetDestination(Vector3 destination)
    {
        if (IsHooked) return false;

        NavVolumeSpace space = _agent.NavVolumeSpace;

        if (space == null || !space.IsReady)
        {
            return false;
        }

        if (!space.IsNavigable(destination)
            && !space.TrySnapToNavigable(destination, 10f, out destination))
        {
            return false;
        }

        if (_lastDestination != destination)
        {
            _lastDestination = destination;
            _hasDestination = true;
            _agent.SetDestination(destination);
        }

        return true;
    }

    public void Hook()
    {
        if (IsHooked) return;
        IsHooked = true;

        _hasDestination = false;
        _lastDestination = Vector3.positiveInfinity; // so the first order after release isn't ignored

        _agent.enabled = false; // stop navigation

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero; // zero BEFORE going kinematic
            _rb.angularVelocity = Vector3.zero;
            _wasKinematic = _rb.isKinematic;
            _rb.isKinematic = true;
        }

        Hooked?.Invoke(); // lets other scripts (AI, animation) freeze themselves
    }

    public void Release()
    {
        if (!IsHooked) return;
        IsHooked = false;

        if (_rb != null) _rb.isKinematic = _wasKinematic;

        IsSubmarineDetected = false; // re-detect cleanly after release
        _agent.enabled = true;
    }

    public float GetDetectionRadius()
    {
        return _detectionRadius[_subUpgrades.Stealth];
    }

    public float GetHarpoonDetectionRadius()
    {
        return _harpoonDetectionRadius[_subUpgrades.Stealth];
    }

    public bool GetHasDestination()
    {
        return _hasDestination;
    }

    public Vector3 GetLastDestination()
    {
        return _lastDestination;
    }

    // Private Methods
    private void CheckForSubmarine()
    {
        float radius = GetDetectionRadius();


        if (Vector3.Magnitude(SubmarineMovement.Instance.transform.position - transform.position) <= radius)
        {
            if (!IsSubmarineDetected)
            {
                IsSubmarineDetected = true;
                DetectedSubmarine?.Invoke(SubmarineMovement.Instance.gameObject);
            }
        }
        else if (IsSubmarineDetected)
        {
            IsSubmarineDetected = false;
            LostSubmarine?.Invoke();
        }
    }

    private void CheckForHarpoon()
    {
        if (SubmarineMovement.Instance.GetHarpoon() == null)
        {
            if (IsHarpoonDetected)
            {
                IsHarpoonDetected = false;
                LostHarpoon?.Invoke();
            }

            return;
        }

        if (Vector3.Magnitude(SubmarineMovement.Instance.GetHarpoon().transform.position - transform.position) <=
            GetHarpoonDetectionRadius())
        {
            if (!IsHarpoonDetected)
            {
                IsHarpoonDetected = true;
                DetectedHarpoon?.Invoke(SubmarineMovement.Instance.GetHarpoon());
            }
        }
        else if (IsHarpoonDetected)
        {
            IsHarpoonDetected = false;
            LostHarpoon?.Invoke();
        }
    }

    // random but sends you towards initial position over time
    private Vector3 GetAlmostRandomDestination()
    {
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * _maxRoamDistance;

        // bias towards initial position
        Vector3 toInitial = (_initialPosition - transform.position).normalized;
        randomDir += toInitial * UnityEngine.Random.Range(0f, _maxRoamDistance);

        return transform.position + randomDir;
    }
}