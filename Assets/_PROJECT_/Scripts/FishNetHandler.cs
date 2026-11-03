
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishNetHandler : MonoBehaviour
{
    // Fields
    public bool IsCatchable = true;
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    private void OnTriggerEnter(Collider other)
    {
        if (IsCatchable && other.CompareTag("Net"))
        {
            UIManager.FishCount++;
            UIManager.Instance.FishBagUpdate(0);
            Destroy(gameObject);
        }
    }

    // Public Methods
    
    // Private Methods
    
}
