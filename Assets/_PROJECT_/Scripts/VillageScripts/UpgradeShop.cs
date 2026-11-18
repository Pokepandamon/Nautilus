
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeShop : InteractableBase
{
    [SerializeField] private GameObject _upgradeShopPanel;
    private bool _isShopOpen = false;
    private InputAction _interactAction;

    private void Awake()
    {
        _interactAction = InputSystem.actions.FindAction("Interact");
        if (_upgradeShopPanel != null)
        {
            _upgradeShopPanel.SetActive(false);
        }
    }
    
    public void Update()
    {
        if (_isShopOpen && _interactAction != null && _interactAction.WasPressedThisFrame())
        {
            CloseShop();
        }
    }

    public override void Interact()
    {
        if (_isShopOpen) return;
        OpenShop();
    }
    
    public void OpenShop()
    {
        if (_upgradeShopPanel == null) return;

        _isShopOpen = true;
        _upgradeShopPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
    }
    
    public void CloseShop()
    {
        if (_upgradeShopPanel == null) return;

        _isShopOpen = false;
        _upgradeShopPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
    
}
