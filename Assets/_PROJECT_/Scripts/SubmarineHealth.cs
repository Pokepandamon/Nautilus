using UnityEngine; 

public class SubmarineHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private GameObject _deathPanel;

    private int _currentSubHp; 

    private void Awake(){
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
            Debug.Log("The submarine has sunk");
        }

    }
    private void Die(){
        Time.timeScale = 0f;
        if(_deathPanel != null){
            _deathPanel.SetActive(true);
        }
    }
}