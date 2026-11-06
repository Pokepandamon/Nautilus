
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (Instance != this)
        {
            return;
        }

        InventoryManager.OnInventoryChange += RefreshFishCount;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        InventoryManager.OnInventoryChange -= RefreshFishCount;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (Instance != this)
        {
            return;
        }

        ReconnectUI();
        RefreshFishCount();
    }

    private void Update()
    {
        if (Instance != this)
        {
            return;
        }

        if (_gameOverGroup != null)
        {
            if (FishCount >= 5)
            {
                _gameOverGroup.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Public Methods

    public void FishBagUpdate(int fish)
    {
        if (_fishBagText != null)
        {
            _fishBagText.text = "Fish Bag: " + FishCount + "/5";
        }
    }

    // Private Methods

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReconnectUI();
        RefreshFishCount();
    }

    private void ReconnectUI()
    {
        _fishBagText = null;
        _gameOverGroup = null;

        if (SceneManager.GetActiveScene().name != "UnderwaterEnv")
        {
            return;
        }

        TMP_Text[] textObjects =
            FindObjectsByType<TMP_Text>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (TMP_Text textObject in textObjects)
        {
            if (textObject.gameObject.name == "FishBagText")
            {
                _fishBagText = textObject;
                break;
            }
        }

        GameObject[] rootObjects =
            SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            Transform[] children = root.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child.name == "GameOverGroup")
                {
                    _gameOverGroup = child.gameObject;
                    break;
                }
            }

            if (_gameOverGroup != null)
            {
                break;
            }
        }

        if (_fishBagText == null)
        {
            Debug.LogWarning(
                "UIManager: Could not find FishBagText in UnderwaterEnv."
            );
        }
    }

    private void RefreshFishCount()
    {
        InventoryManager inv = InventoryManager.instance;

        if (inv == null)
        {
            return;
        }

        int total = 0;

        foreach (InventoryItem item in inv.fishBag)
        {
            total += item.stackCount;
        }

        FishCount = total;
        FishBagUpdate(total);
    }
}
