using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<InventoryItem> fishBag = new List<InventoryItem>();
    public int maxBagCapacity = 5; 
    public int playerCoins = 0;

    public static event Action OnInventoryChange;

    public void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }

    public void AddFish(FishData _fishCaught){
        InventoryItem fish_inside = null; 

        for(int i = 0; i < fishBag.Count; i++){
            if(fishBag[i].fishData == _fishCaught){
                fish_inside = fishBag[i];
                break;
            }
        }

        if(fish_inside != null){
            fish_inside.stackCount += 1;
        }else{
            if(fishBag.Count >= maxBagCapacity){
                Debug.Log("Fishbag is full!");
                return;
            }

            InventoryItem newFish = new InventoryItem(_fishCaught);
            fishBag.Add(newFish);
        }
        Debug.Log($"Caught {_fishCaught.fishName}!");

        OnInventoryChange?.Invoke(); // will trigger event that the inventory has been changed
    }

    public void SellFishBag(){
        int totalEarnings = 0;

        for(int i = 0; i < fishBag.Count; i ++){
            totalEarnings += fishBag[i].fishData.baseSellPrice * fishBag[i].stackCount;
        }

        playerCoins += totalEarnings;
        UpdateCoins(playerCoins);
        fishBag.Clear();

        Debug.Log($"Sold all fish for {totalEarnings}");

        OnInventoryChange?.Invoke();
    }

    public void UpdateCoins(int coins){
        if(VillageUIManager.instance != null && VillageUIManager.instance.coinsText != null){
            VillageUIManager.instance.coinsText.text = "Coins: " + coins;
        }
    }
}