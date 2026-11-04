
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
   [SerializeField] private GameObject _upgradeShopPanel;

    private InputAction _interactAction;
    private bool _isShopOpen = false; 

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
        if (_interactAction.WasPressedThisFrame())
        {
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        if (_upgradeShopPanel != null)
        {
            _isShopOpen = !_isShopOpen;

            _upgradeShopPanel.SetActive(_isShopOpen);

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
