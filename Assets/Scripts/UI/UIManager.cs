using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public TMP_Dropdown pointCloudDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle meshToggle;
    public Button playButton;
    public Button resetButton;

    private PointCloudController currentPC;
    private PointCloudsLoader loader;
    private Dictionary<string, PointCloudObject> nameToData = new Dictionary<string, PointCloudObject>();

    private void Start()
    {
        loader = PointCloudsLoader.Instance;

        if (loader == null)
        {
            Debug.LogError("[UIManager] PointCloudsLoader.Instance is null! Make sure it exists in the scene.");
            return;
        }

        if (loader.pcObjects.Count > 0)
        {
            Debug.Log("[UIManager] Data already loaded, populating dropdowns immediately.");
            PopulateDropdowns();
        }
        else
        {
            Debug.Log("[UIManager] No data loaded yet, subscribing to OnLoaded event.");
            PointCloudsLoader.OnLoaded += PopulateDropdowns;
        }
    }

    void PopulateDropdowns()
    {
        Debug.Log("[UIManager] Populating dropdowns...");
        nameToData.Clear();
        List<string> names = new List<string>();

        foreach (var pc in PointCloudsLoader.Instance.pcObjects)
        {
            Debug.Log($"[UIManager] Found object: {(pc == null ? "null" : pc.ToString())}");
            Debug.Log($"[UIManager] Object type: {pc.GetType().Name} | Name: '{pc.pcName}'");

            if (!string.IsNullOrEmpty(pc.pcName))
            {
                Debug.Log($"[UIManager] Adding point cloud '{pc.pcName}' to dropdown.");
                names.Add(pc.pcName);
                nameToData[pc.pcName] = pc;
            }
            else
            {
                Debug.LogWarning("[UIManager] Point cloud with null or empty name ignored.");
            }
        }

        Debug.Log($"[UIManager] Names found: {string.Join(", ", names)}");

        pointCloudDropdown.ClearOptions();
        pointCloudDropdown.AddOptions(names);

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string> { "1", "2", "3" });

        pointCloudDropdown.onValueChanged.AddListener(OnPointCloudChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        meshToggle.onValueChanged.AddListener(OnMeshToggle);
        playButton.onClick.AddListener(OnPlay);
        resetButton.onClick.AddListener(OnReset);

        if (names.Count > 0)
        {
            LoadPointCloud(names[0]);
        }
        else
        {
            Debug.LogWarning("[UIManager] No valid point cloud names found. Dropdown not populated.");
        }
    }

    void LoadPointCloud(string pcName)
    {
        if (!nameToData.TryGetValue(pcName, out PointCloudObject data))
        {
            Debug.LogError($"[UIManager] Could not find PointCloudObject with name '{pcName}'.");
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Preview_PointCloudPrefab");
        if (prefab == null)
        {
            Debug.LogError("[UIManager] Preview_PointCloudPrefab not found in Resources/Prefabs.");
            return;
        }

        GameObject go = Instantiate(prefab);
        var controller = go.GetComponent<PointCloudController>();
        controller.LoadPointCloud(data.pcName, 1);
        currentPC = controller;
    }

    void OnPointCloudChanged(int index)
    {
        string selected = pointCloudDropdown.options[index].text;
        LoadPointCloud(selected);
    }

    void OnQualityChanged(int index)
    {
        if (currentPC != null)
            currentPC.SetQuality(qualityDropdown.options[index].text);
    }

    void OnMeshToggle(bool isMesh)
    {
        if (currentPC != null)
            currentPC.SetIsMesh(isMesh);
    }

    public void OnPlay()
    {
        // You can enable custom logic here if needed in the future.
    }

    void OnReset()
    {
        if (currentPC != null)
        {
            currentPC.SetAnimate(false);
            currentPC.ResetView();
        }
    }
}
