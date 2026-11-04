using UnityEngine;
using NavVolume;

[RequireComponent(typeof(Fish), typeof(NavVolumeAgent))]
public class Narwhal : MonoBehaviour
{
    [SerializeField] private float _updateInterval = 2f;
    [SerializeField] private float _targetMoveThreshold = 3f;
    [SerializeField] private GameObject _nosePrefab;
    [SerializeField] private Transform _noseMount;
    [SerializeField] private float _noseSpeed = 10f;
    [SerializeField] private float _shootCooldown = 4f;

    private Fish _fish;
    private NavVolumeAgent _agent;
    private GameObject _submarine;

    private float _timer;
    private Vector3 _lastRequestedPosition;
    private bool _hasRequestedPath;
    private float _lastShotTime;
    private float _noseLifetime = 5f;

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

        if (Time.time - _lastShotTime >= _shootCooldown)
        {
            ShootNose();
            _lastShotTime = Time.time;
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

    private void ShootNose()
    {
        GameObject noseInstance = Instantiate(_nosePrefab, _noseMount);
        Rigidbody noseRigidbody = noseInstance.GetComponent<Rigidbody>();

        noseRigidbody.linearVelocity = _noseMount.up * _noseSpeed;

        Destroy(noseInstance, _noseLifetime);
    }
}