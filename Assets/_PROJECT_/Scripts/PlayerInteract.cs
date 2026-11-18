
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
   [SerializeField] private GameObject _interactPrompt;
   [SerializeField] private float interact_dist = 3f;

    private InputAction _interactAction;
    private bool _isLookingAtInteractable = false;
    private InteractableBase _currentInteractable;

    void Awake()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
    }    

    private void OnEnable()
    {
        _interactAction.Enable();
    }

    private void OnDisable()
    {
        _interactAction.Disable();
    }

    void Update()
    {
        if(_interactAction == null) return; 

        if(Time.timeScale == 0f){ // if shop is open then pressing E will close it 
            if(_interactAction.WasPressedThisFrame()){
                
                Debug.Log("E was pressed closing shop");

                if(NpcVillagers.currentShopOpen != null){
                    Debug.Log("Shop closing");
                    NpcVillagers.currentShopOpen.CloseShop();
                }
            }

            if(_interactPrompt!= null){
                _interactPrompt.SetActive(false);
            }
            return;
        }

        CheckForInteractable();

        if(_interactAction.WasPressedThisFrame()){
            if(_isLookingAtInteractable && _currentInteractable != null){
                _currentInteractable.Interact();
            }
        }
    }


    private void CheckForInteractable(){
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool foundInteractable = false;
        InteractableBase hitInteractable = null;

        if(Physics.Raycast(ray, out hit, interact_dist)){ // will cast a ray in front of player
            if(hit.collider.TryGetComponent<InteractableBase>(out hitInteractable)){ 
                foundInteractable = true;
            }
        }

        if (_isLookingAtInteractable != foundInteractable || _currentInteractable != hitInteractable) // will check if its either true or false 
    {
        _isLookingAtInteractable = foundInteractable; // then change if not equal to each other 
        _currentInteractable = hitInteractable;

        if (_interactPrompt != null)
        {
            _interactPrompt.SetActive(_isLookingAtInteractable);
        }
    }
    }

}
