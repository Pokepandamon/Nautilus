using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CanvasUIManager : MonoBehaviour {

    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private GameObject _pauseMenu;
    private InputAction _pauseAction;

    private bool _isPaused = false;

    public void Awake(){
        _pauseAction = InputSystem.actions.FindAction("Pause");
    }

    private void OnEnable(){
        if(_pauseAction != null){
            _pauseAction.performed += OnPauseTriggered;
            _pauseAction.Enable();
        }
    }

    private void OnDisable(){
        if(_pauseAction != null){
            _pauseAction.performed -= OnPauseTriggered;
            _pauseAction.Disable();
        }
    }

    private void OnPauseTriggered(InputAction.CallbackContext context)
    {
        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void RestartGame(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ResumeGame(){
        _isPaused = false; 
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame(){
        _isPaused = true; 
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}