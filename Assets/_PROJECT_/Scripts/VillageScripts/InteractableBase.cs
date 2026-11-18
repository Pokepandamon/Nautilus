
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBase : MonoBehaviour
{
    public virtual void Interact(){
        Debug.Log("Player interacted with object");
    }
    
}
