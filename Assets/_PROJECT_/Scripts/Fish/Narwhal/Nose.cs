using UnityEngine;

public class Nose : MonoBehaviour
{
    // Fields
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    // TODO: verify hurts tagged objects being triggers or colliders
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Submarine"))
        {
            Destroy(gameObject);
        }
    }

    // Public Methods
    
    // Private Methods
    
}
