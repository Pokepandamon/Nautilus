using UnityEngine; 
using System.Collections.Generic; 

public class SubStats : MonoBehaviour 
{
    [SerializeField] private SubUpgrades _subUpgrades;

    [SerializeField] private List<int> speedUpgradeCosts = new List<int> {4 , 10}; // made a list with the cost of upgrade lvl 1, 2 etc
    [SerializeField] private List<int> stealthUpgradeCosts = new List<int> {4, 10};
    [SerializeField] private List<int> harpoonUpgradeCosts = new List<int> {5, 12};

    // The fish and submarine read these values from the shared SubUpgrades asset.
    // Keeping the village UI on the same asset makes purchased upgrades apply in
    // Master Map after leaving the village scene.
    public int SpeedLevel => _subUpgrades.Speed;
    public int StealthLevel => _subUpgrades.Stealth;
    public int HarpoonLevel => _subUpgrades.Harpoon;

    public List<int> SpeedUpgradeCosts => speedUpgradeCosts;
    public List<int> StealthUpgradeCosts => stealthUpgradeCosts;
    public List<int> HarpoonUpgradeCosts => harpoonUpgradeCosts;

    public float currentSpeed => _subUpgrades.Speed;
    public float currentSleath => _subUpgrades.Stealth;
    public float currentHarpoon => _subUpgrades.Harpoon;

    public void UpgradeSpeed(){
        if(_subUpgrades.Speed >= speedUpgradeCosts.Count){
            Debug.Log("Speed is maxed out");
            return;
        }
        int costToUpgrade = speedUpgradeCosts[_subUpgrades.Speed];

        if(InventoryManager.instance.playerCoins >= costToUpgrade){
            InventoryManager.instance.playerCoins -= costToUpgrade;
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
            _subUpgrades.Speed++;
            Debug.Log($"Speed is now Level {_subUpgrades.Speed}");

            if(VillageUIManager.instance != null){
                VillageUIManager.instance.UpdateUpgradeCosts();
            }
        }else{
            Debug.Log($"Not enough coins");
        }
    }

    public void UpgradeStealth(){
        if(_subUpgrades.Stealth >= stealthUpgradeCosts.Count){
            Debug.Log("Stealth is maxed out");
            return;
        }
        int costToUpgrade = stealthUpgradeCosts[_subUpgrades.Stealth];

        if(InventoryManager.instance.playerCoins >= costToUpgrade){
            InventoryManager.instance.playerCoins -= costToUpgrade;
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
            _subUpgrades.Stealth++;
            Debug.Log($"Stealth is now Level {_subUpgrades.Stealth}");

            if(VillageUIManager.instance != null){
                VillageUIManager.instance.UpdateUpgradeCosts();
            }
        }else{
            Debug.Log($"Not enough coins");
        }
    }

    public void UpgradeHarpoon(){
        if(_subUpgrades.Harpoon >= harpoonUpgradeCosts.Count){
            Debug.Log("Harpoon is maxed out");
            return;
        }
        int costToUpgrade = harpoonUpgradeCosts[_subUpgrades.Harpoon];

        if(InventoryManager.instance.playerCoins >= costToUpgrade){
            InventoryManager.instance.playerCoins -= costToUpgrade;
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
            _subUpgrades.Harpoon++;
            Debug.Log($"Harpoon is now Level {_subUpgrades.Harpoon}");

            if(VillageUIManager.instance != null){
                VillageUIManager.instance.UpdateUpgradeCosts();
            }
        }else{
            Debug.Log($"Not enough coins");
        }
    }
}
