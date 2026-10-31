
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonRope : MonoBehaviour
{
    // Fields
    [SerializeField] private Transform _source;
    [SerializeField] private Transform _target;
    private LineRenderer _lineRenderer;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    
    private void Awake()
    {
        // Setup the line renderer
        _lineRenderer = GetComponent<LineRenderer>();
        Vector3[] initialPos = {_source.position, _target.position};
        _lineRenderer.SetPositions(initialPos);
    }

    private void Update()
    {
        // Update the ends of the rope
        Vector3[] currentPos = {_source.position, _target.position};
        _lineRenderer.SetPositions(currentPos);
    }
    
    // Public Methods
    
    // Private Methods
    
}
