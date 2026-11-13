
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barnacle : MonoBehaviour
{
    // Fields
    [SerializeField] private float _S1BDetectionRadius;
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
        if (_open && Vector3.Magnitude(SubmarineMovement.Instance.transform.position - transform.position) <= _S1BDetectionRadius)
        {
            _animator.Play("FastBarnacleClose");
            _open = false;
        }
        else if (!_open && Vector3.Magnitude(SubmarineMovement.Instance.transform.position - transform.position) > _S1BDetectionRadius)
        {
            _animator.Play("BarnacleOpen");
            _open = true;
        }
    }
    
    // Public Methods
    
    // Private Methods
    
}
