
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    // Fields

    [SerializeField] private TMP_Text _fishBagText;
    [SerializeField] private GameObject _gameOverGroup;
    [SerializeField] private GameObject _heightWarningGroup;
    [SerializeField] private float _warningHeight;
    [SerializeField] private float _killHeight;
    [SerializeField] private AudioMixer _mixer;

    public static UIManager Instance;
    public static int FishCount = 0;
    private Animator _heightWarningAnimator;
    private SubmarineHealth _killObject;
    private AudioSource _audioSource;

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
        _heightWarningAnimator = _heightWarningGroup.GetComponent<Animator>();
        _killObject = SubmarineMovement.Instance.gameObject.GetComponent<SubmarineHealth>();
        _audioSource = GetComponent<AudioSource>();
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
        //Debug.Log("Disabled");
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
            InventoryManager inv = InventoryManager.instance;
            
            if (inv != null && FishCount >= inv.maxBagCapacity)
            {
                _gameOverGroup.SetActive(true);
            }
        }

        if (SubmarineMovement.Instance != null && SubmarineMovement.Instance.transform.position.y > _warningHeight && Time.timeScale == 1f)
        {
            _heightWarningGroup.SetActive(true);
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }

            _mixer.SetFloat("KlaxonPitch", 1f / (1f + (SubmarineMovement.Instance.transform.position.y - _warningHeight) / (_killHeight - _warningHeight)));
            _audioSource.pitch = 1f + (SubmarineMovement.Instance.transform.position.y - _warningHeight) / (_killHeight - _warningHeight);
            _heightWarningAnimator.speed = 1 + (SubmarineMovement.Instance.transform.position.y - _warningHeight) / (_killHeight - _warningHeight);
        }
        else
        {
            if (_audioSource.isPlaying){
                _audioSource.Pause();
            }

            if (SubmarineMovement.Instance != null)
            {
                _heightWarningGroup.SetActive(false);
            }
        }

        if (SubmarineMovement.Instance != null && SubmarineMovement.Instance.transform.position.y > _killHeight)
        {
            _killObject.TakeDamage(100);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        //Debug.Log("Destroyed");
    }

    // Public Methods

    public void FishBagUpdate(int fish)
    {
        if (_fishBagText != null)
        {
            int capacity = InventoryManager.instance != null
                ? InventoryManager.instance.maxBagCapacity
                : 5;
            _fishBagText.text = "Fish Bag: " + fish + "/" + capacity;
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

        if (SceneManager.GetActiveScene().name != "Master Map")
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

        FishCount = inv.FishCount;
        FishBagUpdate(FishCount);
    }
}
