using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageToWater : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "UnderwaterEnv"; // Name of your ocean scene

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
        {
            Debug.Log("Trigger entered by non-player: " + other.gameObject.name);
            return;
        }

        if (isLoading)
            return;

        isLoading = true;
        Debug.Log($"Player entered dock! Loading ocean scene: {targetSceneName}");

        SceneManager.LoadScene(targetSceneName);
    }

    private bool IsPlayer(Collider other)
    {
        return other.GetComponentInParent<SubStats>() != null;
    }
}