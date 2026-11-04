using System.Collections;
using UnityEngine; 
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public FishData fishData;
    public int stackCount;

    public InventoryItem(FishData data){
        fishData = data; 
        stackCount = 1;
    }
}