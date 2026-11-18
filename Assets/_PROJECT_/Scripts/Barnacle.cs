
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barnacle : MonoBehaviour
{
    // Fields
    [SerializeField] private float _S1BDetectionRadius;
    [SerializeField] private LayerMask _SB1Mask;
    private Animator _animator;
    private bool _open = true;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool detected = false;
        Collider[] possibleBosses = Physics.OverlapSphere(transform.position, _S1BDetectionRadius);
        foreach (Collider possibleBoss in possibleBosses)
        {
            if (possibleBoss.CompareTag("SB1Boss"))
            {
                detected = true;
            }
        }
        if (_open && detected)
        {
            _animator.Play("FastBarnacleClose");
            _open = false;
        }
        else if (!_open && !detected)
        {
            _animator.Play("BarnacleOpen");
            _open = true;
        }
    }
    
    // Public Methods
    
    // Private Methods
    
}
