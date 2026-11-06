using UnityEngine; 
using System.Collections.Generic; 

public class SubStats : MonoBehaviour 
{
    [SerializeField] private SubUpgrades _subUpgrades;

    [SerializeField] private List<int> speedUpgradeCosts = new List<int> {4 , 10}; // made a list with the cost of upgrade lvl 1, 2 etc
    [SerializeField] private List<int> stealthUpgradeCosts = new List<int> {4, 10};
    [SerializeField] private List<int> harpoonUpgradeCosts = new List<int> {5, 12};

    private int speedLevel = 0;  // track the levels of the player 
    private int stealthLevel = 0; 
    private int harpoonLevel = 0; 

    public int SpeedLevel => speedLevel;
    public int StealthLevel => stealthLevel;
    public int HarpoonLevel => harpoonLevel;

    public List<int> SpeedUpgradeCosts => speedUpgradeCosts;
    public List<int> StealthUpgradeCosts => stealthUpgradeCosts;
    public List<int> HarpoonUpgradeCosts => harpoonUpgradeCosts;

    public float currentSpeed => _subUpgrades.Speed + speedLevel; 
    public float currentSleath => _subUpgrades.Stealth + stealthLevel; 
    public float currentHarpoon => _subUpgrades.Harpoon + harpoonLevel; 

    public void UpgradeSpeed(){
        if(speedLevel >= speedUpgradeCosts.Count){
            Debug.Log("Speed is maxed out");
            return;
        }
        int costToUpgrade = speedUpgradeCosts[speedLevel];

        if(InventoryManager.instance.playerCoins >= costToUpgrade){
            InventoryManager.instance.playerCoins -= costToUpgrade;
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
            speedLevel++;
            Debug.Log($"Speed is now Level {speedLevel}");

            if(VillageUIManager.instance != null){
                VillageUIManager.instance.UpdateUpgradeCosts();
            }
        }else{
            Debug.Log($"Not enough coins");
        }
    }

    public void UpgradeStealth(){
        if(stealthLevel >= stealthUpgradeCosts.Count){
            Debug.Log("Stealth is maxed out");
            return;
        }
        int costToUpgrade = stealthUpgradeCosts[stealthLevel];

        if(InventoryManager.instance.playerCoins >= costToUpgrade){
            InventoryManager.instance.playerCoins -= costToUpgrade;
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
            stealthLevel++;
            Debug.Log($"Stealth is now Level {stealthLevel}");

            if(VillageUIManager.instance != null){
                VillageUIManager.instance.UpdateUpgradeCosts();
            }
        }else{
            Debug.Log($"Not enough coins");
        }
    }

    public void UpgradeHarpoon(){
        if(harpoonLevel >= harpoonUpgradeCosts.Count){
            Debug.Log("Harpoon is maxed out");
            return;
        }
        int costToUpgrade = harpoonUpgradeCosts[harpoonLevel];

        if(InventoryManager.instance.playerCoins >= costToUpgrade){
            InventoryManager.instance.playerCoins -= costToUpgrade;
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
            harpoonLevel++;
            Debug.Log($"Harpoon is now Level {harpoonLevel}");

            if(VillageUIManager.instance != null){
                VillageUIManager.instance.UpdateUpgradeCosts();
            }
        }else{
            Debug.Log($"Not enough coins");
        }
    }
}