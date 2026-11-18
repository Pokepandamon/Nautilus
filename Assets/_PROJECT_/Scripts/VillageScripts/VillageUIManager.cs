using UnityEngine;
using TMPro;

public class VillageUIManager : MonoBehaviour
{
    public static VillageUIManager instance; 

    [SerializeField]public TextMeshProUGUI coinsText; 
    [SerializeField]public TextMeshProUGUI _speedCostsText;
    [SerializeField]public TextMeshProUGUI _harpoonCostsText;
    [SerializeField]public TextMeshProUGUI _stealthCostsText;

    private SubStats subStats; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        subStats = FindAnyObjectByType<SubStats>();

        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
        }

        UpdateUpgradeCosts();
    }

    public void UpdateUpgradeCosts(){
        if(subStats == null){
            return; 
        }

        if(_speedCostsText != null){
            if (subStats.SpeedLevel < subStats.SpeedUpgradeCosts.Count){
                _speedCostsText.text = "Cost: " + subStats.SpeedUpgradeCosts[subStats.SpeedLevel];}
            else{
                _speedCostsText.text = "MAX";
            }
        }

        if(_harpoonCostsText != null){
            if (subStats.HarpoonLevel < subStats.HarpoonUpgradeCosts.Count){
                _harpoonCostsText.text = "Cost: " + subStats.HarpoonUpgradeCosts[subStats.HarpoonLevel];}
            else{
                _harpoonCostsText.text = "MAX";
            }
        }

        if(_stealthCostsText != null){
            if (subStats.StealthLevel < subStats.StealthUpgradeCosts.Count){
                _stealthCostsText.text = "Cost: " + subStats.StealthUpgradeCosts[subStats.StealthLevel];}
            else{
                _stealthCostsText.text = "MAX";
            }
        }
    }

    public void OnSellFishButtonClicked(){       
        if (InventoryManager.instance != null){
            InventoryManager.instance.SellFishBag();}
    }
}