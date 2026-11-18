
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NpcVillagers : InteractableBase
{
    public static NpcVillagers currentShopOpen {get; private set;} // since we have multiple villagers this will be useful to know which shop is open

    [SerializeField] private string villagerName;

    [SerializeField] private GameObject villagerShopPanel; 

    [SerializeField] private AudioClip greetingClip;
    [SerializeField] private AudioClip exitClip;
    [SerializeField] private AudioClip thanksClip;

    [SerializeField] private FishData fishWanted;
    [SerializeField] private int bonusSellPrice; 

    private AudioSource _audioSource;
    private Animator _animator; 
    private bool _isShopOpen = false;

    private void Awake(){
        _audioSource = GetComponent<AudioSource>();
        _audioSource.spatialBlend = 1f; 
        _animator = GetComponent<Animator>();

        if(villagerShopPanel != null){
            villagerShopPanel.SetActive(false);
        }
    }


    public override void Interact(){
        if(_isShopOpen) return; 

        OpenShop();
    }

    private void OpenShop(){
        _isShopOpen = true; 

        currentShopOpen = this;

        if(villagerShopPanel != null){
            villagerShopPanel.SetActive(true);
        }

        PlayAudio(greetingClip);
        if(_animator != null){
            _animator.SetTrigger("Greeting");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void SellWantedFish(){
        if(fishWanted == null || InventoryManager.instance == null) {
            return;}

        
        int totalEarnings = InventoryManager.instance.SellSpecificFish(fishWanted, bonusSellPrice);

        if(totalEarnings > 0){

            PlayAudio(thanksClip);
            if(_animator != null){
                _animator.SetTrigger("Dance 1");
            }

            Debug.Log($"Sold {fishWanted.fishName} for {totalEarnings} coins");
        }else{
            Debug.Log($"You don't have any {fishWanted.fishName}");
        }
    }

    public void CloseShop(){
        if(!_isShopOpen) return;

        _isShopOpen = false;

        if(villagerShopPanel != null){
            villagerShopPanel.SetActive(false);
        }

        currentShopOpen = null;

        PlayAudio(exitClip);

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
     }

     private void PlayAudio(AudioClip clip){
        if(clip != null && _audioSource != null){
            _audioSource.PlayOneShot(clip);
        }
     }
    
}
