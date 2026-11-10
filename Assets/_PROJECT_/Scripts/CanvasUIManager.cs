using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasUIManager : MonoBehaviour{

 public void RestartGame(){
        SceneManager.LoadScene("Start");
    }

public void QuitGame(){
        Application.Quit();
    }
}