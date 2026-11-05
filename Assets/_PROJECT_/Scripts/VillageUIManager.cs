using UnityEngine;
using TMPro;

public class VillageUIManager : MonoBehaviour
{
    public static VillageUIManager instance; 

    [SerializeField]public TextMeshProUGUI coinsText; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
        }
    }
}