
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Fish))]
public class Guppy : MonoBehaviour
{
    // Fields
    [SerializeField] private float _speed = 3f;
    
    private Fish _fish;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void Awake()
    {
        _fish = GetComponent<Fish>();
    }

    // Public Methods
    
    // Private Methods

}

