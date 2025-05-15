using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Pcx;

[RequireComponent(typeof(AnimatePointCloudBase))]
public class PointCloudController : MonoBehaviour
{
    public PointCloudObject data;

    private AnimatePointCloudBase animator;

    private bool isMesh = false;
    private int currentQuality = 1;

    void Awake()
    {
        animator = GetComponent<AnimatePointCloudBase>();
    }

    public void SetQuality(string qualityString)
    {
        if (data == null) return;

        int quality = int.Parse(qualityString.Replace("q", ""));
        currentQuality = quality;
        Debug.Log($"[PCController] Changing quality to: q{quality}");

        data.frames.Clear();

#if UNITY_ANDROID && !UNITY_EDITOR
        string basePath = Application.persistentDataPath;
#else
        string basePath = Path.Combine(Application.dataPath, "PointClouds");
#endif

        string folderPath = Path.Combine(basePath, "PointClouds", data.pcName, $"q{quality}", "PointClouds");

       
        string testPath = Path.Combine(basePath, "PointClouds", "frame001.ply");
        Debug.Log($"[PCController] Full test path example: {Application.persistentDataPath}");

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"[PCController] Folder not found: {folderPath}");
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.ply");
        System.Array.Sort(files);

        List<Mesh> meshList = new();

        foreach (var file in files)
        {
            var mesh = RuntimePLYLoader.LoadMeshFromPLY(file);
            if (mesh != null)
            {
                meshList.Add(mesh);
            }
            else
            {
                Debug.LogWarning($"[PCController] Invalid mesh in file: {file}");
            }
        }

        data.frames.AddRange(meshList);
        Debug.Log($"[PCController] Loaded {meshList.Count} frames from {folderPath}");

        animator.LoadQuality(meshList);
        animator.StopAnimation();
        animator.StartAnimation();
        Debug.Log("[PCController] Animation restarted with new quality");
    }

    public void SetIsMesh(bool mesh)
    {
        isMesh = mesh;
        Debug.Log($"[PCController] Mesh mode: {isMesh}");
    }

    public void SetAnimate(bool active)
    {
        if (active)
            animator.StartAnimation();
        else
            animator.StopAnimation();

        Debug.Log($"[PCController] Animate: {active}");
    }

    public void ResetView()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        SetQuality("q3");
        SetIsMesh(false);
        SetAnimate(false);

        Debug.Log($"[PCController] View reset");
    }

    public void LoadPointCloud(string name, int quality)
    {
        if (data != null)
        {
            data.pcName = name;
        }

        // transform.position = new Vector3(0, 0, 3.0f);

        if (data != null)
        {
            data.pcName = name;
        }

        // Posiziona il point cloud davanti alla camera, un po' più in basso
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 forward = cam.transform.forward.normalized;
            Vector3 pos = cam.transform.position + forward * 3.0f;
            pos.y -= 1.0f; // Abbassa di 1 metro rispetto alla camera
            transform.position = pos;
            Debug.Log($"[PCController] Position set dynamically in front of camera at {transform.position}");
        }
        else
        {
            transform.position = new Vector3(0, 0, 3.0f);
            Debug.LogWarning("[PCController] Main camera not found, using default position.");
        }

        transform.localScale = Vector3.one * 0.002f;

        Material vertexMat = Resources.Load<Material>("Materials/PCX_VertexColor");
        if (vertexMat != null)
        {
            foreach (Transform child in transform)
            {
                var renderer = child.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = vertexMat;
                    Debug.Log($"[PCController] Material applied to {child.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[PCController] Material PCX_VertexColor not found in Resources/Materials");
        }

        SetQuality($"q{quality}");
    }

    public int CurrentQuality => currentQuality;
}
