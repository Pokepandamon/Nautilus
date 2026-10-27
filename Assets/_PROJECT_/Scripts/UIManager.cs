
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
    
    // Properties
    
    // Events / Delegates
    
    // Monobehaviour Methods (Awake, Start, Update, ...)
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _fishBagText.text = "Fish Bag: 0/5";
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
        // _fishBagText.text = "Fish Bag: " + fish + "/5";
        _fishBagText.text = "Fish Bag: " + FishCount + "/5";
    }
    
    // Private Methods
    
}
