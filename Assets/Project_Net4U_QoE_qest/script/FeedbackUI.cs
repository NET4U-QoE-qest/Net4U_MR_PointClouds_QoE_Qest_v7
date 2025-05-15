using UnityEngine;
using TMPro;

public class FeedbackUIController : MonoBehaviour
{
    public TMP_Text infoText;


    public void UpdateInfo(string pointCloudName, string quality, string distance)
    {
        infoText.text = $"PC: {pointCloudName} | Quality: {quality} | Dist: {distance}";
    }

    public void SetText(string info)
    {
        if (infoText != null)
            infoText.text = info;
    }



}
