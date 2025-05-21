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
            Debug.LogError("❌ PointCloudsLoader.Instance è null! Assicurati che esista in scena.");
            return;
        }

        if (loader.pcObjects.Count > 0)
        {
            Debug.Log("[UIManager] Dati già presenti, popolamento immediato.");
            PopulateDropdowns();
        }
        else
        {
            Debug.Log("[UIManager] Nessun dato ancora caricato, mi sottoscrivo a OnLoaded.");
            PointCloudsLoader.OnLoaded += PopulateDropdowns;
        }
    }

    void PopulateDropdowns()
    {
        Debug.Log("[UIManager] Popolamento dropdown avviato.");
        nameToData.Clear();
        List<string> names = new List<string>();

        foreach (var pc in PointCloudsLoader.Instance.pcObjects)
        {
            Debug.Log($"[UIManager] Trovato oggetto: {(pc == null ? "null" : pc.ToString())}");
            Debug.Log($"[UIManager] Trovato oggetto: {pc.GetType().Name} | Nome: '{pc.pcName}'");

            if (!string.IsNullOrEmpty(pc.pcName))
            {
                Debug.Log($"✅ Aggiungo point cloud '{pc.pcName}' al dropdown.");
                names.Add(pc.pcName);
                nameToData[pc.pcName] = pc;
            }
            else
            {
                Debug.LogWarning("⚠️ Point cloud con nome nullo o vuoto ignorato.");
            }
        }

        Debug.Log($"[UIManager] Nomi trovati: {string.Join(", ", names)}");

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
            Debug.LogWarning("[UIManager] Nessun nome valido trovato. Dropdown non popolato.");
        }
    }

    void LoadPointCloud(string pcName)
    {
        if (!nameToData.TryGetValue(pcName, out PointCloudObject data))
        {
            Debug.LogError($"❌ Impossibile trovare PointCloudObject con nome '{pcName}'.");
            return;
        }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/Preview_PointCloudPrefab");
        if (prefab == null)
        {
            Debug.LogError("❌ Preview_PointCloudPrefab non trovato in Resources/Prefabs");
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
        //string fixedName = "BlueSpin_UVG_vox10_25_0_250"; // Nome della cartella principale
        //string fixedQuality = "1"; // Qualità fissa q1

        //Debug.Log($"▶ PLAY fissa | {fixedName} - q{fixedQuality}");

        //currentPC = PointCloudsLoader.Instance.Spawn(fixedName, fixedQuality);
        
        //if (currentPC != null)
       // {
       //     currentPC.SetIsMesh(meshToggle.isOn); // puoi disattivare anche questo se vuoi sempre point cloud
        //    currentPC.SetAnimate(true);
        //}
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
