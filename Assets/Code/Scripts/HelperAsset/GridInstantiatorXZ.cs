using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridInstantiatorXZ : MonoBehaviour
{
    [Header("Prefab / Parent")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform parentOverride;

    [Header("Grid Size")]
    [Min(1)][SerializeField] private int columns = 5;
    [Min(1)][SerializeField] private int rows = 5;

    [Header("Offsets (World Units)")]
    [SerializeField] private float xOffset = 2f;
    [SerializeField] private float zOffset = 2f;

    [Header("Placement")]
    [SerializeField] private Transform origin;
    [SerializeField] private bool centerOnOrigin = false;

    [Header("Lifecycle")]
    [SerializeField] private bool generateOnStart = true;

    private readonly List<GameObject> spawned = new();

    private void Start()
    {
        if (generateOnStart)
            Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab이 지정되지 않았습니다.", this);
            return;
        }

        ClearSpawned();

        Transform originTf = origin != null ? origin : transform;
        Transform parentTf = parentOverride != null ? parentOverride : transform;

        Vector3 basePos = originTf.position;

        if (centerOnOrigin)
        {
            float width = (columns - 1) * xOffset;
            float depth = (rows - 1) * zOffset;
            basePos -= new Vector3(width * 0.5f, 0f, depth * 0.5f);
        }

        Quaternion rot = originTf.rotation;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 worldPos = basePos + new Vector3(c * xOffset, 0f, r * zOffset);
                GameObject go = CreatePrefabInstance(parentTf);

                go.transform.SetPositionAndRotation(worldPos, rot);
                go.name = $"{prefab.name}_{r}_{c}";
                spawned.Add(go);
            }
        }
    }

    private GameObject CreatePrefabInstance(Transform parent)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject instance =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            Undo.RegisterCreatedObjectUndo(instance, "Grid Instantiate Prefab");
            return instance;
        }
#endif
        return Instantiate(prefab, parent);
    }

    [ContextMenu("Clear Spawned")]
    public void ClearSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] == null) continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(spawned[i]);
            else
                Destroy(spawned[i]);
#else
            Destroy(spawned[i]);
#endif
        }
        spawned.Clear();
    }
}
