using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class RandomPointCloudPlayer : MonoBehaviour
{
    [Header("Prefabs & References")]
    public GameObject pointCloudPrefab;
    public Transform spawnAnchor;
    public TextMeshProUGUI randomStatusBanner;

    [Header("Settings")]
    public float distanceFromCamera = 2f;
    public int qualityLevel = 2;

    private List<string> remainingPointClouds = new();
    private string basePath;
    private System.Random random = new();
    private GameObject currentInstance;

    private bool isRunning = false;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        basePath = Path.Combine(Application.persistentDataPath, "PointClouds");
#else
        basePath = Path.Combine(Application.dataPath, "PointClouds");
#endif
    }

    public void StartRandomSequence()
    {
        if (isRunning) return;

        if (!Directory.Exists(basePath))
        {
            Debug.LogError("[RandomPlayer] PointClouds directory not found!");
            return;
        }

        remainingPointClouds.Clear();
        foreach (var dir in Directory.GetDirectories(basePath))
        {
            string pcName = Path.GetFileName(dir);
            remainingPointClouds.Add(pcName);
        }

        if (remainingPointClouds.Count == 0)
        {
            ShowBanner("No point clouds available.", 3f);
            return;
        }

        isRunning = true;
        ShowBanner("Starting random point cloud sequence...", 2f);
        LoadNextRandomPointCloud();
    }

    public void SubmitAndContinue()
    {
        if (!isRunning) return;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        if (remainingPointClouds.Count > 0)
        {
            ShowBanner("Evaluation submitted. Loading next...", 1.5f);
            LoadNextRandomPointCloud();
        }
        else
        {
            ShowBanner("Sequence complete. All point clouds evaluated.", 3f);
            isRunning = false;
        }
    }

    private void LoadNextRandomPointCloud()
    {
        int index = random.Next(remainingPointClouds.Count);
        string nextPC = remainingPointClouds[index];
        remainingPointClouds.RemoveAt(index);

        currentInstance = Instantiate(pointCloudPrefab);
        var controller = currentInstance.GetComponent<PointCloudController>();
        if (controller != null)
        {
            controller.LoadPointCloud(nextPC, qualityLevel);
            controller.transform.position = GetSpawnPosition();
        }
        else
        {
            Debug.LogError("[RandomPlayer] PointCloudController missing on prefab!");
        }
    }

    private Vector3 GetSpawnPosition()
    {
        Camera camera = Camera.main;
        if (camera == null)
            return Vector3.zero;

        Vector3 forward = camera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 position = camera.transform.position + forward * distanceFromCamera;
        position.y = camera.transform.position.y;
        return position;
    }

    private void ShowBanner(string message, float duration)
    {
        if (randomStatusBanner == null) return;
        StopAllCoroutines();
        StartCoroutine(BannerRoutine(message, duration));
    }

    private IEnumerator BannerRoutine(string message, float duration)
    {
        randomStatusBanner.text = message;
        randomStatusBanner.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        randomStatusBanner.gameObject.SetActive(false);
    }
}
