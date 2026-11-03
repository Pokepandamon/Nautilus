using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Fields

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)

    // Public Methods

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Private Methods
}