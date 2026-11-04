
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Net"))
        {
            if (_fish != null && _fish.FishData != null)
            {
                InventoryManager.instance.AddFish(_fish.FishData);
            }

            Destroy(gameObject); 
        }
    }

    // Public Methods
    
    // Private Methods

}

