using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointCloudObject
{
    public string pcName;
    public PCObjectType objectType = PCObjectType.Unknown;
    public int[] qualities = new int[] { 1, 2, 3 };
    public List<Mesh> frames = new List<Mesh>();

    public Dictionary<int, Mesh[]> pointClouds = new Dictionary<int, Mesh[]>();
    public Dictionary<int, Mesh[]> meshes = new Dictionary<int, Mesh[]>();
    public Dictionary<int, Material[]> meshMaterials = new Dictionary<int, Material[]>();

    public void LoadAssetsFromResources()
    {
        if (string.IsNullOrEmpty(pcName))
        {
            Debug.LogWarning($"[PCObject] pcName is null or empty, cannot load.");
            return;
        }

        foreach (int q in qualities)
        {
            // Load Mesh (Point Clouds)
            string pcPath = $"PointClouds/{pcName}/q{q}/PointClouds";
            Mesh[] pcs = Resources.LoadAll<Mesh>(pcPath);
            if (pcs.Length > 0)
            {
                pointClouds[q] = pcs;
                Debug.Log($"[PCObject] Loaded {pcs.Length} point clouds from {pcPath}");
            }
            else
            {
                Debug.LogWarning($"[PCObject] No point clouds found in {pcPath}");
            }

            // Load Mesh + Material (if available)
            string meshPath = $"PointClouds/{pcName}/q{q}/Meshes";
            GameObject[] meshPrefabs = Resources.LoadAll<GameObject>(meshPath);
            if (meshPrefabs.Length > 0)
            {
                List<Mesh> meshList = new List<Mesh>();
                List<Material> matList = new List<Material>();

                foreach (GameObject go in meshPrefabs)
                {
                    MeshFilter mf = go.GetComponent<MeshFilter>();
                    MeshRenderer mr = go.GetComponent<MeshRenderer>();
                    if (mf != null && mf.sharedMesh != null)
                        meshList.Add(mf.sharedMesh);
                    if (mr != null && mr.sharedMaterial != null)
                        matList.Add(mr.sharedMaterial);
                }

                meshes[q] = meshList.ToArray();
                meshMaterials[q] = matList.ToArray();
            }
        }
    }
}
