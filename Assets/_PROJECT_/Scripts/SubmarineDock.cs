
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmarineDock : MonoBehaviour
{
    // Fields

    [SerializeField] private Transform submarine;
    [SerializeField] private string villageSceneName = "Village";

    private bool hasLeftDock = false;
    private bool isLoading = false;

    // Properties

    // Events / Delegates

    // Monobehaviour Methods (Awake, Start, Update, ...)

    private void OnTriggerExit(Collider other)
    {
        if (!IsSubmarine(other))
            return;

        hasLeftDock = true;

        Debug.Log("Submarine left dock. Fishing trip started!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsSubmarine(other))
            return;

        if (!hasLeftDock || isLoading)
            return;

        isLoading = true;

        Debug.Log("Submarine returned to dock. Loading village!");

        SceneManager.LoadScene(villageSceneName);
    }

    // Public Methods

    // Private Methods

    private bool IsSubmarine(Collider other)
    {
        return submarine != null &&
               (other.transform == submarine ||
                other.transform.IsChildOf(submarine));
    }
}
