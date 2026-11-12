// Save as Assets/Editor/TerrainEquationSplitter.cs.
// Open Tools > Terrain > Split With Two Lines. Editor only.
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainEquationSplitter : EditorWindow
{
    private Terrain source;
    private float slope1, intercept1 = 100f;
    private float slope2, intercept2 = 200f;
    private GameObject previewRoot;
    private Terrain previewSource;
    private bool sourceWasActive;
    private TerrainData[] previewData;

    [MenuItem("Tools/Terrain/Split With Two Lines")]
    private static void Open() => GetWindow<TerrainEquationSplitter>("Terrain Split");

    private void OnEnable() => SceneView.duringSceneGui += DrawLines;

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DrawLines;
        ClearPreview();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "Lines use world coordinates: z = slope * x + intercept.\n" +
            "Generate Preview creates three independent copies. Regenerating discards preview edits.\n" +
            "Save As New Terrains keeps the copies and creates their TerrainData assets.",
            MessageType.Info);

        EditorGUI.BeginChangeCheck();
        source = (Terrain)EditorGUILayout.ObjectField("Original Terrain", source, typeof(Terrain), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Line 1 (yellow)", EditorStyles.boldLabel);
        slope1 = EditorGUILayout.FloatField("Slope", slope1);
        intercept1 = EditorGUILayout.FloatField("Intercept", intercept1);
        EditorGUILayout.LabelField("Line 2 (cyan)", EditorStyles.boldLabel);
        slope2 = EditorGUILayout.FloatField("Slope", slope2);
        intercept2 = EditorGUILayout.FloatField("Intercept", intercept2);
        if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();

        using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
        {
            if (GUILayout.Button("Generate / Update Preview"))
            {
                try { GeneratePreview(); }
                catch (Exception e) { ClearPreview(); Debug.LogException(e); }
            }
            if (GUILayout.Button("Clear Preview")) ClearPreview();
            using (new EditorGUI.DisabledScope(previewRoot == null))
            {
                if (GUILayout.Button("Save As New Terrains")) SaveTerrains();
            }
        }
        EditorGUILayout.HelpBox(
            "The original is hidden during preview and restored when the preview is cleared or this window closes.\n" +
            "After saving, the original stays disabled. Closing this window then keeps the saved terrains.\n" +
            "Hole edges follow the terrain grid. Trees/details are omitted from the copies.",
            MessageType.None);
    }

    private void GeneratePreview()
    {
        if (!source || !source.terrainData || !source.gameObject.scene.IsValid())
            throw new InvalidOperationException("Assign a Terrain in an open scene.");
        if (previewRoot && source.transform.IsChildOf(previewRoot.transform))
            throw new InvalidOperationException("Choose the original terrain, not a preview copy.");
        if (Quaternion.Angle(source.transform.rotation, Quaternion.identity) > 0.01f ||
            (source.transform.lossyScale - Vector3.one).sqrMagnitude > 0.000001f)
            throw new InvalidOperationException("The source must have world rotation zero and world scale one.");
        if (!Finite(slope1) || !Finite(slope2) || !Finite(intercept1) || !Finite(intercept2))
            throw new InvalidOperationException("Line parameters must be finite numbers.");

        ClearPreview();
        previewSource = source;
        sourceWasActive = source.gameObject.activeSelf;

        previewRoot = new GameObject(source.name + "_Split");
        previewRoot.hideFlags = HideFlags.DontSave;
        SceneManager.MoveGameObjectToScene(previewRoot, source.gameObject.scene);
        previewData = new TerrainData[3];

        TerrainData original = source.terrainData;
        int resolution = original.holesResolution;
        bool[,] originalHoles = original.GetHoles(0, 0, resolution, resolution);
        var masks = new bool[3][,];
        for (int i = 0; i < 3; i++) masks[i] = new bool[resolution, resolution];

        Vector3 origin = source.transform.position;
        Vector3 size = original.size;
        for (int z = 0; z < resolution; z++)
        {
            float worldZ = origin.z + (z + 0.5f) / resolution * size.z;
            for (int x = 0; x < resolution; x++)
            {
                float worldX = origin.x + (x + 0.5f) / resolution * size.x;
                float a = slope1 * worldX + intercept1;
                float b = slope2 * worldX + intercept2;
                int region = worldZ < Mathf.Min(a, b) ? 0 :
                    worldZ < Mathf.Max(a, b) ? 1 : 2;
                masks[region][z, x] = originalHoles[z, x];
            }
        }

        string[] names = { "BelowBothLines", "BetweenLines", "AboveBothLines" };
        for (int i = 0; i < 3; i++)
        {
            TerrainData data = Instantiate(original);
            previewData[i] = data;
            data.name = source.name + "_" + names[i];
            data.hideFlags = HideFlags.DontSave;
            // Vegetation is not partitioned by this tool. Omit it to avoid duplicates.
            data.treeInstances = Array.Empty<TreeInstance>();
            data.detailPrototypes = Array.Empty<DetailPrototype>();
            data.SetHoles(0, 0, masks[i]);

            GameObject go = Terrain.CreateTerrainGameObject(data);
            go.hideFlags = HideFlags.DontSave;
            go.transform.SetParent(previewRoot.transform, false);
            go.transform.position = origin;
            go.name = data.name;
            go.layer = source.gameObject.layer;
            go.tag = source.gameObject.tag;
            Terrain terrain = go.GetComponent<Terrain>();
            // Retain the original's material and Terrain component settings.
            EditorUtility.CopySerialized(source, terrain);
            terrain.terrainData = data;
            terrain.enabled = true;
            terrain.allowAutoConnect = false;
            TerrainCollider collider = go.GetComponent<TerrainCollider>();
            TerrainCollider originalCollider = source.GetComponent<TerrainCollider>();
            if (originalCollider) EditorUtility.CopySerialized(originalCollider, collider);
            else collider.enabled = false;
            collider.terrainData = data;
            terrain.Flush();
        }
        source.gameObject.SetActive(false);
        SceneView.RepaintAll();
    }

    private void ClearPreview()
    {
        if (previewRoot) DestroyImmediate(previewRoot);
        if (previewData != null)
            foreach (TerrainData data in previewData)
                if (data && !AssetDatabase.Contains(data)) DestroyImmediate(data);
        if (previewSource) previewSource.gameObject.SetActive(sourceWasActive);
        previewRoot = null;
        previewData = null;
        previewSource = null;
        SceneView.RepaintAll();
    }

    private void SaveTerrains()
    {
        // Save the currently displayed preview, not un-applied parameter changes.
        string folder = AssetDatabase.GenerateUniqueAssetPath("Assets/TerrainSplit");
        AssetDatabase.CreateFolder("Assets", folder.Substring("Assets/".Length));
        for (int i = 0; i < previewData.Length; i++)
        {
            previewData[i].hideFlags = HideFlags.None;
            AssetDatabase.CreateAsset(previewData[i], folder + "/Region" + i + ".asset");
        }
        foreach (Transform child in previewRoot.transform)
            child.gameObject.hideFlags = HideFlags.None;
        previewRoot.hideFlags = HideFlags.None;
        EditorSceneManager.MarkSceneDirty(previewRoot.scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = previewRoot;
        Debug.Log("Saved split TerrainData assets to " + folder + ". Save your scene too.");
        previewRoot = null;
        previewData = null;
        previewSource = null;
    }

    private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

    private void DrawLines(SceneView view)
    {
        if (!source || !source.terrainData) return;
        DrawLine(slope1, intercept1, Color.yellow);
        DrawLine(slope2, intercept2, Color.cyan);
    }

    private void DrawLine(float slope, float intercept, Color color)
    {
        if (!Finite(slope) || !Finite(intercept)) return;
        Vector3 origin = source.transform.position;
        Vector3 size = source.terrainData.size;
        // Clip the equation to the terrain's X/Z rectangle before sampling it.
        float xMin = origin.x, xMax = origin.x + size.x;
        if (slope == 0f)
        {
            if (intercept < origin.z || intercept > origin.z + size.z) return;
        }
        else
        {
            float a = (origin.z - intercept) / slope;
            float b = (origin.z + size.z - intercept) / slope;
            xMin = Mathf.Max(xMin, Mathf.Min(a, b));
            xMax = Mathf.Min(xMax, Mathf.Max(a, b));
            if (xMin > xMax) return;
        }
        Handles.color = color;
        Vector3 previous = default;
        for (int i = 0; i <= 256; i++)
        {
            float x = Mathf.Lerp(xMin, xMax, i / 256f);
            float z = slope * x + intercept;
            float y = origin.y + source.terrainData.GetInterpolatedHeight(
                (x - origin.x) / size.x, (z - origin.z) / size.z);
            Vector3 current = new Vector3(x, y + 0.2f, z);
            if (i > 0) Handles.DrawAAPolyLine(3f, previous, current);
            previous = current;
        }
    }
}
