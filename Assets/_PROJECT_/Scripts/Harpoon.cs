
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
    
    //
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
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _initialTime = Time.fixedTime;
        _harpoonRigidbody = _harpoon.GetComponent<Rigidbody>();
        _harpoonRigidbody.linearDamping = _harpoonDamping;
        
        if (_ropeJoint != null)
            return;

        // sourceAttachment = _ropeSource.transform;
        //Rigidbody sourceBody = _ropeSource.transform.GetComponentInParent<Rigidbody>();

        _ropeLength = _maxLengthUpgrades[_subUpgrades.Harpoon];
        _reachedEnd = false;

        _ropeJoint = _harpoon.AddComponent<ConfigurableJoint>();
        _ropeJoint.autoConfigureConnectedAnchor = false;

        _ropeJoint.connectedBody = null;
        _ropeJoint.anchor = transform.InverseTransformPoint(_harpoonButt.transform.position);

        // With a connected Rigidbody, this is in that body's local space.
        // Otherwise, it is a world-space position.
        _ropeJoint.connectedAnchor = _ropeSource.transform.position;

        _ropeJoint.xMotion = ConfigurableJointMotion.Limited;
        _ropeJoint.yMotion = ConfigurableJointMotion.Limited;
        _ropeJoint.zMotion = ConfigurableJointMotion.Limited;

        _ropeJoint.angularXMotion = ConfigurableJointMotion.Free;
        _ropeJoint.angularYMotion = ConfigurableJointMotion.Free;
        _ropeJoint.angularZMotion = ConfigurableJointMotion.Free;

        // Hard rope limit, with no spring or bounce.
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
        // Add the sub momentum to the harppon so we don't ever outpace it
        if (!_appliedSubMomentum && _subRigidbody != null)
        {
            _harpoonRigidbody.linearVelocity += Vector3.Project(_subRigidbody.linearVelocity, _harpoonRigidbody.linearVelocity);
            _appliedSubMomentum = true;
        }

        Vector3 sourcePoint = _ropeSource.transform.position;
        _ropeJoint.connectedAnchor = sourcePoint;

        Vector3 harpoonPoint = _harpoon.transform.TransformPoint(_ropeJoint.anchor);

        // Small tolerance because physics constraints are approximate.
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
                // This is where an animation to bring the harppon back in the sub could go
                Destroy(_ropeSource);
                Destroy(gameObject);
            }
            SetRopeLength();
        }
    }

    private void LateUpdate()
    {
        _ropeVisual.SetPosition(0, _ropeSource.transform.position);
        _ropeVisual.SetPosition(1, _harpoonButt.transform.position);
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
}
