
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Fields
    [SerializeField] private float _speed;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    
    private void FixedUpdate()
    {
        transform.Translate(_speed * Time.fixedDeltaTime * Vector3.forward, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 0.1f);
    }

    // Public Methods
    
    // Private Methods
    
}
