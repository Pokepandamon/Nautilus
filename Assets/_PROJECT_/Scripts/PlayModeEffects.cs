using UnityEngine;
using UnityEngine.Rendering;

public class PlayModeEffects : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableBuiltInFog = true;
    
    [Header("URP / HDRP Volume")]
    public Volume postProcessVolume;

    [Header("Caustics Objects / Projectors")]
    public GameObject[] causticsObjects;

    void Awake()
    {
        // Enable built-in Lighting window fog
        if (enableBuiltInFog)
        {
            RenderSettings.fog = true;
        }

        // Enable URP/HDRP Volume (for Volumetric Fog or Post-Processing)
        if (postProcessVolume != null)
        {
            postProcessVolume.enabled = true;
        }

        // Enable Caustics projectors, decals, or lights
        foreach (GameObject obj in causticsObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}