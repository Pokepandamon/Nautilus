
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
   [SerializeField] private GameObject _upgradeShopPanel;
   [SerializeField] private GameObject _interactPrompt;
   [SerializeField] private float interact_dist = 3f;

    private InputAction _interactAction;
    private bool _isShopOpen = false; 
    private bool _isLookingAtInteractable = false;

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

       if (_isShopOpen)
        {
            if (_interactAction != null && _interactAction.WasPressedThisFrame())
            {
                TogglePanel();
            }
            return;
        }

        CheckForInteractable();

        if (_interactAction != null && _interactAction.WasPressedThisFrame())
        {
            if(_isLookingAtInteractable){
                TogglePanel();
            }
        }
    }


    private void CheckForInteractable(){
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool foundInteractable = false;

        if(Physics.Raycast(ray, out hit, interact_dist)){ // will cast a ray in front of player
            if(hit.collider.CompareTag("Interactable")){ 
                foundInteractable = true;
            }
        }

        if (_isLookingAtInteractable != foundInteractable) // will check if its either true or false 
    {
        _isLookingAtInteractable = foundInteractable; // then change if not equal to each other 

        if (_interactPrompt != null)
        {
            _interactPrompt.SetActive(_isLookingAtInteractable);
        }
    }
    }

    private void TogglePanel()
    {
        if (_upgradeShopPanel != null)
        {
            _isShopOpen = !_isShopOpen;

            _upgradeShopPanel.SetActive(_isShopOpen);

            if(_interactPrompt != null){ // hides prompt while in shop
                _interactPrompt.SetActive(false);
            }

            if (_isShopOpen){
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
            }else{
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
            }
            
        }

    }
}
