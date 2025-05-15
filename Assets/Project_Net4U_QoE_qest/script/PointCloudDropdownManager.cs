using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class PointCloudDropdownManager : MonoBehaviour
{
    public TMP_Dropdown pointCloudDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown distanceDropdown;
    public PointCloudController controller;
    public QoESliderManager qoeManager;

    private List<string> pointCloudNames = new();
    private float currentDistance = 1f;

    private static bool hasInitialized = false;

    void Start()
    {
        if (hasInitialized)
            return;

        hasInitialized = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        string basePath = Application.persistentDataPath + "/PointClouds";
#else
        string basePath = Path.Combine(Application.dataPath, "PointClouds");
#endif

        if (Directory.Exists(basePath))
        {
            string[] dirs = Directory.GetDirectories(basePath);
            foreach (var dir in dirs)
            {
                string pcName = Path.GetFileName(dir);
                pointCloudNames.Add(pcName);
            }

            Debug.Log($"[DropdownManager] Found {pointCloudNames.Count} point clouds.");
        }
        else
        {
            Debug.LogWarning($"[DropdownManager] Directory not found: {basePath}");
        }

        pointCloudDropdown.ClearOptions();
        pointCloudDropdown.AddOptions(pointCloudNames);

        pointCloudDropdown.onValueChanged.AddListener(OnPointCloudChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        distanceDropdown.onValueChanged.AddListener(OnDistanceChanged);

        OnDistanceChanged(distanceDropdown.value);

        if (pointCloudNames.Count > 0)
        {
            controller.LoadPointCloud(pointCloudNames[pointCloudDropdown.value], controller.CurrentQuality);
            controller.transform.position = GetPositionInFront(currentDistance);
            UpdateFeedbackPanel();
        }
    }

    void OnPointCloudChanged(int index)
    {
        string name = pointCloudNames[index];
        controller.LoadPointCloud(name, controller.CurrentQuality);
        controller.transform.position = GetPositionInFront(currentDistance);
        Debug.Log($"[DropdownManager] Selected point cloud: {name}");
        UpdateFeedbackPanel();
    }

    void OnQualityChanged(int index)
    {
        string quality = qualityDropdown.options[index].text;
        controller.SetQuality(quality);
        controller.transform.position = GetPositionInFront(currentDistance);
        Debug.Log($"[DropdownManager] Quality changed to: {quality}");
        UpdateFeedbackPanel();
    }

    void OnDistanceChanged(int index)
    {
        string option = distanceDropdown.options[index].text;
        if (float.TryParse(option.Replace("m", ""), out float distance))
        {
            currentDistance = distance;
            controller.transform.position = GetPositionInFront(distance);
            Debug.Log($"[DropdownManager] Distance set to: {distance}m");
            UpdateFeedbackPanel();
        }
    }

    public void StartPointCloud()
    {
        string name = pointCloudNames[pointCloudDropdown.value];
        int quality = controller.CurrentQuality;
        controller.LoadPointCloud(name, quality);
        controller.transform.position = GetPositionInFront(currentDistance);
        Debug.Log($"[StartButton] Started point cloud: {name} (Quality: {quality})");
    }

    Vector3 GetPositionInFront(float distance)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            Debug.LogWarning("[DropdownManager] Main camera not found.");
            return Vector3.zero;
        }

        Vector3 forward = camera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 position = camera.transform.position + forward * distance;
        position.y = 0f;
        return position;
    }

    void UpdateFeedbackPanel()
    {
        if (qoeManager == null)
            qoeManager = FindObjectOfType<QoESliderManager>();

        if (qoeManager != null)
        {
            string pcName = pointCloudNames[pointCloudDropdown.value];
            string quality = qualityDropdown.options[qualityDropdown.value].text;
            string distance = distanceDropdown.options[distanceDropdown.value].text;
            qoeManager.UpdatePCInfo(pcName, quality, distance);
        }
    }
}