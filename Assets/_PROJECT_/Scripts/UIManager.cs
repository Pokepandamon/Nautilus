using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Fields
    [SerializeField] private TMP_Text _fishBagText;
    [SerializeField] private GameObject _gameOverGroup;

    public static UIManager Instance;
    public static int FishCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FishCount = 0;
        _fishBagText.text = "Fish Bag: 0/5";
    }

    private void OnEnable()
    {
        InventoryManager.OnInventoryChange += RefreshFishCount;
    }

    private void OnDisable()
    {
        // The event is static, so always unsubscribe or it will call a destroyed object
        InventoryManager.OnInventoryChange -= RefreshFishCount;
    }

    private void Start()
    {
        // Start runs after every Awake, so InventoryManager.instance exists by now
        RefreshFishCount();
    }

    private void Update()
    {
        if (FishCount >= 5)
        {
            _gameOverGroup.SetActive(true);
        }
    }

    // Public Methods
    public void FishBagUpdate(int fish)
    {
        _fishBagText.text = "Fish Bag: " + FishCount + "/5";
    }

    // Private Methods
    private void RefreshFishCount()
    {
        InventoryManager inv = InventoryManager.instance;
        if (inv == null) return;

        int total = 0;
        foreach (InventoryItem item in inv.fishBag)
            total += item.stackCount;

        FishCount = total;
        FishBagUpdate(total);
    }
}