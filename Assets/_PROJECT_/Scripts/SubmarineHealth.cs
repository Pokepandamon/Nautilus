using System;
using UnityEngine; 

public class SubmarineHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private GameObject _deathPanel;
    
    [SerializeField] private AudioClip _submarineHurt;
    [SerializeField] private AudioClip _submarineBreak;
    
    private AudioSource _audioSource;

    private int _currentSubHp;

    public static event Action OnDeath;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _currentSubHp = _maxHealth; 

        if(_healthBar != null){
            _healthBar.SetMaxHealth(_maxHealth);
        }
    }

    public void TakeDamage(int damage){
        _currentSubHp -= damage;
        _currentSubHp = Mathf.Clamp(_currentSubHp, 0, _maxHealth); //make sure the health doesn't drop below 0 
        Debug.Log("Sub got hit, health decreased by 1");

        if(_healthBar != null){
            _healthBar.SetHealth(_currentSubHp);
        }

        if(_currentSubHp <= 0){
            Die();
            _audioSource.PlayOneShot(_submarineBreak);
            Debug.Log("The submarine has sunk");
        }
        else
        {
            _audioSource.PlayOneShot(_submarineHurt);
        }

    }
    private void Die(){
        Time.timeScale = 0f;
        OnDeath?.Invoke();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(_deathPanel != null){
            _deathPanel.SetActive(true);
        }
    }
}