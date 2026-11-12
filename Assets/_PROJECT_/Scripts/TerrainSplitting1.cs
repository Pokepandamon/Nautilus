using UnityEngine;

public class TerrainSplitting1 : MonoBehaviour
{
    [SerializeField] private Terrain original;
    [SerializeField] private Terrain sideA;
    [SerializeField] private Terrain sideB;

    [SerializeField] private float slope = 2f;
    [SerializeField] private float intercept = 100f;

    [ContextMenu("Generate Split")]
    private void GenerateSplit()
    {
        if (original == null || sideA == null || sideB == null)
        {
            Debug.LogError("Assign all three terrains.", this);
            return;
        }

        SplitAlongLine(original, sideA, sideB, slope, intercept);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(sideA.terrainData);
        UnityEditor.EditorUtility.SetDirty(sideB.terrainData);
        UnityEditor.AssetDatabase.SaveAssets();
#endif

        Debug.Log("Terrain split generated.", this);
    }

    // Put this method inside a script.
    // Call once when setting up the split, not every frame.
    public static void SplitAlongLine(
        Terrain original,
        Terrain sideA,
        Terrain sideB,
        float m,
        float b)
    {
        TerrainData source = original.terrainData;
        TerrainData dataA = sideA.terrainData;
        TerrainData dataB = sideB.terrainData;
    
        if (source == dataA || source == dataB || dataA == dataB)
            throw new System.ArgumentException(
                "Each terrain must have its own TerrainData asset.");
    
        int resolution = source.holesResolution;
    
        // Assumes both copies have the original's position, size,
        // rotation, scale, and resolution.
        bool[,] originalMask = source.GetHoles(
            0, 0, resolution, resolution);
    
        bool[,] maskA = new bool[resolution, resolution];
        bool[,] maskB = new bool[resolution, resolution];
    
        Vector3 position = original.transform.position;
        Vector3 size = source.size;
    
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // World position at the center of this hole-grid cell.
                float worldX = position.x
                    + (x + 0.5f) / resolution * size.x;
    
                float worldZ = position.z
                    + (z + 0.5f) / resolution * size.z;
    
                bool belongsToA = worldZ >= m * worldX + b;
    
                // true = terrain surface; false = hole.
                maskA[z, x] = originalMask[z, x] && belongsToA;
                maskB[z, x] = originalMask[z, x] && !belongsToA;
            }
        }
    
        dataA.SetHoles(0, 0, maskA);
        dataB.SetHoles(0, 0, maskB);
    }
}