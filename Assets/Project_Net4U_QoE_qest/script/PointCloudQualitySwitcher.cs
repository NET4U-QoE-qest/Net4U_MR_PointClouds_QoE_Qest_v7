using TMPro;
using UnityEngine;

public class PointCloudQualitySwitcher : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public PointCloudController pointCloudController;

    void Start()
    {
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    void OnQualityChanged(int index)
    {
        string selected = qualityDropdown.options[index].text;
        Debug.Log($" Change qualty: {selected}");
        pointCloudController?.SetQuality(selected);
    }
}

