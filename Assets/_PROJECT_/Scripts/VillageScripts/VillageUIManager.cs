using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VillageUIManager : MonoBehaviour
{
    public static VillageUIManager instance; 

    [SerializeField]public TextMeshProUGUI coinsText; 
    [SerializeField]public TextMeshProUGUI _speedCostsText;
    [SerializeField]public TextMeshProUGUI _harpoonCostsText;
    [SerializeField]public TextMeshProUGUI _stealthCostsText;

    private SubStats subStats; 
    private TextMeshProUGUI fishBagText;
    private GameObject fishBagPanel;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        InventoryManager.OnInventoryChange += UpdateFishBagDisplay;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= UpdateFishBagDisplay;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleFishBagDisplay();
        }
    }

    private void Start()
    {
        subStats = FindAnyObjectByType<SubStats>();

        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.UpdateCoins(InventoryManager.instance.playerCoins);
        }

        UpdateUpgradeCosts();
        CreateFishBagDisplay();
        UpdateFishBagDisplay();
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

    private void CreateFishBagDisplay()
    {
        if (fishBagText != null || coinsText == null)
        {
            return;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Village fish bag display could not find a Canvas.");
            return;
        }

        fishBagPanel = new GameObject("Fish Bag Display", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fishBagPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = fishBagPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 1f);
        panelRect.anchorMax = new Vector2(1f, 1f);
        panelRect.pivot = new Vector2(1f, 1f);
        panelRect.anchoredPosition = new Vector2(-24f, -145f);
        panelRect.sizeDelta = new Vector2(300f, 180f);

        Image panelImage = fishBagPanel.GetComponent<Image>();
        panelImage.color = new Color(0.02f, 0.06f, 0.12f, 0.96f);

        GameObject textObject = Instantiate(coinsText.gameObject, fishBagPanel.transform);
        textObject.name = "Fish Bag Text";
        fishBagText = textObject.GetComponent<TextMeshProUGUI>();
        fishBagText.raycastTarget = false;
        fishBagText.fontSize = 24f;
        fishBagText.alignment = TextAlignmentOptions.TopLeft;
        fishBagText.color = Color.white;

        RectTransform textRect = fishBagText.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(16f, 12f);
        textRect.offsetMax = new Vector2(-16f, -12f);

        fishBagPanel.SetActive(false);
    }

    private void ToggleFishBagDisplay()
    {
        if (fishBagPanel == null)
        {
            return;
        }

        bool shouldShow = !fishBagPanel.activeSelf;
        fishBagPanel.SetActive(shouldShow);

        if (shouldShow)
        {
            UpdateFishBagDisplay();
        }
    }

    private void UpdateFishBagDisplay()
    {
        if (fishBagText == null)
        {
            return;
        }

        InventoryManager inventory = InventoryManager.instance;
        if (inventory == null)
        {
            fishBagText.text = "Fish Bag\nUnavailable";
            return;
        }

        StringBuilder bagContents = new StringBuilder();
        bagContents.Append($"Fish Bag: {inventory.FishCount} / {inventory.maxBagCapacity}");

        if (inventory.FishCount == 0)
        {
            bagContents.Append("\n\nEmpty");
        }
        else
        {
            foreach (InventoryItem item in inventory.fishBag)
            {
                if (item.fishData != null && item.stackCount > 0)
                {
                    bagContents.Append($"\n• {item.fishData.fishName} x{item.stackCount}");
                }
            }
        }

        fishBagText.text = bagContents.ToString();
    }
}
