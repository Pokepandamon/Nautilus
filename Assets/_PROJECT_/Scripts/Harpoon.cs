using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    // Fields
    [SerializeField] private SubUpgrades _subUpgrades;

    // The part of the harpoon that actually moves
    [SerializeField] private GameObject _harpoon;

    // How long the harpoon is out for
    [SerializeField] private float _harpoonTime;

    // Harpoon Distances
    [SerializeField] private float[] _maxLengthUpgrades;
    [SerializeField] private float[] _impulseUpgrades;
    [SerializeField] private float _retractSpeed;

    [SerializeField] private LineRenderer _ropeVisual;

    [SerializeField] private GameObject _ropeSource;
    [SerializeField] private GameObject _harpoonButt;

    [SerializeField] private float _harpoonDamping;

    private GameObject _sub;
    private Rigidbody _subRigidbody;

    // Sub doesn't call SetSub fast enough for Awake
    private bool _appliedSubMomentum = false;

    private Rigidbody _harpoonRigidbody;
    private ConfigurableJoint _ropeJoint;
    private float _ropeLength;

    private float _initialTime;
    private bool _reachedEnd;

    // NEW: no longer serialized, found/added automatically in Awake
    private HarpoonTip _tip;
    private Fish _hookedFish;

    // Monobehaviour Methods
    private void Awake()
    {

        _tip = _harpoon.GetComponent<HarpoonTip>();
        if (_tip == null)
            _tip = _harpoon.AddComponent<HarpoonTip>();
        _tip.HitFish += OnHitFish;

        _initialTime = Time.fixedTime;
        _harpoonRigidbody = _harpoon.GetComponent<Rigidbody>();
        _harpoonRigidbody.linearDamping = _harpoonDamping;

        if (_ropeJoint != null)
            return;

        _ropeLength = _maxLengthUpgrades[_subUpgrades.Harpoon];
        _reachedEnd = false;

        _ropeJoint = _harpoon.AddComponent<ConfigurableJoint>();
        _ropeJoint.autoConfigureConnectedAnchor = false;

        _ropeJoint.connectedBody = null;
        _ropeJoint.anchor = _harpoon.transform.InverseTransformPoint(_harpoonButt.transform.position);

        // With no connected Rigidbody, this is a world-space position
        _ropeJoint.connectedAnchor = _ropeSource.transform.position;

        _ropeJoint.xMotion = ConfigurableJointMotion.Limited;
        _ropeJoint.yMotion = ConfigurableJointMotion.Limited;
        _ropeJoint.zMotion = ConfigurableJointMotion.Limited;

        _ropeJoint.angularXMotion = ConfigurableJointMotion.Free;
        _ropeJoint.angularYMotion = ConfigurableJointMotion.Free;
        _ropeJoint.angularZMotion = ConfigurableJointMotion.Free;

        // Hard rope limit, with no spring or bounce
        _ropeJoint.linearLimitSpring = new SoftJointLimitSpring
        {
            spring = 0f,
            damper = 0f
        };

        SetRopeLength();
        _harpoonRigidbody.AddRelativeForce(_impulseUpgrades[_subUpgrades.Harpoon] * Vector3.right, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        // Add the sub momentum to the harpoon so we don't ever outpace it
        if (!_appliedSubMomentum && _subRigidbody != null)
        {
            _harpoonRigidbody.linearVelocity += Vector3.Project(_subRigidbody.linearVelocity, _harpoonRigidbody.linearVelocity);
            _appliedSubMomentum = true;
        }

        Vector3 sourcePoint = _ropeSource.transform.position;
        _ropeJoint.connectedAnchor = sourcePoint;

        Vector3 harpoonPoint = _harpoon.transform.TransformPoint(_ropeJoint.anchor);

        // Small tolerance because physics constraints are approximate
        if (!_reachedEnd &&
            Vector3.Distance(harpoonPoint, sourcePoint) >= _maxLengthUpgrades[_subUpgrades.Harpoon] - 0.02f)
        {
            _reachedEnd = true;
        }

        if (_reachedEnd)
        {
            _ropeLength = Mathf.MoveTowards(_ropeLength, 0f, _retractSpeed * Time.fixedDeltaTime);
            if (_ropeLength <= 0)
            {
                CollectFish(); 
                Destroy(_ropeSource);
                Destroy(gameObject);
                return;        
            }
            SetRopeLength();
        }
    }

    private void LateUpdate()
    {
        _ropeVisual.SetPosition(0, _ropeSource.transform.position);
        _ropeVisual.SetPosition(1, _harpoonButt.transform.position);
    }

    private void OnDestroy()
    {
        if (_tip != null) _tip.HitFish -= OnHitFish;

        if (_hookedFish != null)
        {
            _hookedFish.transform.SetParent(null, true);
            _hookedFish.Release();
            _hookedFish = null;
        }
    }

    // Public Methods
    public GameObject GetRopeSource()
    {
        return _ropeSource;
    }

    public void SetSub(GameObject sub)
    {
        _sub = sub;
        _subRigidbody = sub.GetComponent<Rigidbody>();
    }

    // Private Methods
    private void SetRopeLength()
    {
        _ropeJoint.linearLimit = new SoftJointLimit
        {
            limit = _ropeLength,
            bounciness = 0f,
            contactDistance = 0f
        };
    }

    private void OnHitFish(Fish fish)
    {
        if (_hookedFish != null || fish.IsHooked) return; 

        _hookedFish = fish;
        fish.Hook(); // freezes the fish

        // Stick the fish to the harpoon
        fish.transform.SetParent(_harpoon.transform, true);

        _harpoonRigidbody.linearVelocity = Vector3.zero;

        Vector3 harpoonPoint = _harpoon.transform.TransformPoint(_ropeJoint.anchor);
        float currentDistance = Vector3.Distance(harpoonPoint, _ropeSource.transform.position);
        _ropeLength = Mathf.Min(_ropeLength, currentDistance);
        SetRopeLength();

        _reachedEnd = true;
    }

    private void CollectFish()
    {
        if (_hookedFish == null) return;

        if (_hookedFish.FishData != null)
            InventoryManager.instance.AddFish(_hookedFish.FishData);

        Destroy(_hookedFish.gameObject);
        _hookedFish = null;
    }
}