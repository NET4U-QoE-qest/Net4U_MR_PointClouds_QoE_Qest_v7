using System.Collections.Generic;
using UnityEngine;

public class AnimatePointCloudBase : MonoBehaviour
{
    public Mesh[] CurrentMeshes;
    public Material[] meshMaterials;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public PCObjectType objectType;
    public PCMaterialType CurrentMaterial;

    private bool animate = false;
    public bool IsAnimating => animate;

    private bool isMesh = false;
    public bool IsMesh => isMesh;

    private int currentQuality = 1;
    private int currentIndex = 0;
    private float frameRate = 30f;

    private void Start()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        Material vertexMat = Resources.Load<Material>("Materials/PCX_VertexColor");
        if (vertexMat != null)
        {
            meshRenderer.material = vertexMat;
        }
        else
        {
            Debug.LogWarning("Material PCX_VertexColor not found in Resources/Materials/");
        }
    }

    private void Update()
    {
        if (animate && CurrentMeshes != null && CurrentMeshes.Length > 0)
        {
            currentIndex = (int)(Time.time * frameRate) % CurrentMeshes.Length;
            meshFilter.mesh = CurrentMeshes[currentIndex];
        }
    }

    public void StartAnimation()
    {
        animate = true;
    }

    public void StopAnimation()
    {
        animate = false;
    }

    public void SetAnimate(bool active)
    {
        if (active)
            StartAnimation();
        else
            StopAnimation();
    }

    public void SetCurrentObject(PCObjectType type)
    {
        objectType = type;
    }

    public void SetCurrentQuality(int quality)
    {
        currentQuality = quality;
    }

    public void SetIsMesh(bool useMesh)
    {
        isMesh = useMesh;
    }

    // Fallback method: load from Resources folder
    public void LoadQuality(string pcName, int quality)
    {
        string path = $"PointClouds/{pcName}/q{quality}/PointClouds/";
        Mesh[] meshes = Resources.LoadAll<Mesh>(path);

        if (meshes != null && meshes.Length > 0)
        {
            CurrentMeshes = meshes;
            Debug.Log($"[AnimatePCBase] Loaded {meshes.Length} frames from Resources at {path}");
        }
        else
        {
            Debug.LogWarning($"[AnimatePCBase] No frames found in Resources at {path}");
        }
    }

    // New method: load from external List<Mesh> (e.g., from Android/data)
    public void LoadQuality(List<Mesh> meshList)
    {
        if (meshList != null && meshList.Count > 0)
        {
            CurrentMeshes = meshList.ToArray();
            Debug.Log($"[AnimatePCBase] Loaded {meshList.Count} meshes from external source.");
        }
        else
        {
            Debug.LogWarning("[AnimatePCBase] Provided mesh list is null or empty.");
        }
    }
}
