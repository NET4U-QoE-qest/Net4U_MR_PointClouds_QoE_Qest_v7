using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectChanger : MonoBehaviour
{
    public static ObjectChanger Instance { get; private set; }

    public GameObject objectButtonParent;
    public GameObject objectButtonPrefab;

    // Stores buttons by point cloud name
    private Dictionary<string, GameObject> objectButtons = new Dictionary<string, GameObject>();
    private Dictionary<string, TextMeshPro> objectTexts = new Dictionary<string, TextMeshPro>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        SpawnAndSetupButtons();
    }

    private void SpawnAndSetupButtons()
    {
        int counter = 0;
        float spacing = 0.0225f;

        foreach (var pcObject in PointCloudsLoader.Instance.pcObjects)
        {
            string pcName = pcObject.pcName;
            if (string.IsNullOrEmpty(pcName)) continue;

            GameObject currentButton = Instantiate(objectButtonPrefab, objectButtonParent.transform);
            currentButton.transform.localPosition = new Vector3(-0.0275f + counter * spacing, 0f, 0f);
            currentButton.transform.localScale = new Vector3(0.2f, 0.33f, 0.2f);

            string capturedName = pcName; // Prevent closure issue
            currentButton.GetComponentInChildren<TextMeshPro>().text = capturedName;
            currentButton.GetComponent<Button>().onClick.AddListener(() => OnObjectButtonPressed(capturedName));

            objectButtons.Add(capturedName, currentButton);
            objectTexts.Add(capturedName, currentButton.GetComponentInChildren<TextMeshPro>());

            counter++;
        }
    }

    public void HighlightSelectedObject(string selectedName)
    {
        foreach (var kv in objectTexts)
        {
            kv.Value.color = kv.Key == selectedName ? Color.green : Color.white;
        }
    }

    public void OnObjectButtonPressed(string pcName)
    {
        ConfigurationSceneManager.Instance.ChangeCurrentPCObject(pcName);
        HighlightSelectedObject(pcName);
    }
}
