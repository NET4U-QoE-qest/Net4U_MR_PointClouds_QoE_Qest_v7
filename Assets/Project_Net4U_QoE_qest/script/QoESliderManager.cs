using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System;

public class QoESliderManager : MonoBehaviour
{
    public Slider qoeSlider;
    public TMP_Text numericValueText;
    public TMP_Text pcInfoText;

    public Button buttonNext;
    public Button buttonPrev;
    public Button buttonSubmit;

    public TextMeshProUGUI saveNotification;

    private string currentPCName = "N/A";
    private string currentQuality = "N/A";
    private string currentDistance = "N/A";

    private bool isSliderUpdating = false;
    private bool hasSubmittedRecently = false;

    private Coroutine notificationCoroutine;


    void Start()
    {
        UpdateNumericValueText();
        qoeSlider.onValueChanged.AddListener(delegate { UpdateNumericValueText(); });

        buttonNext.onClick.RemoveAllListeners();
        buttonNext.onClick.AddListener(IncrementSlider);

        buttonPrev.onClick.RemoveAllListeners();
        buttonPrev.onClick.AddListener(DecrementSlider);

        buttonSubmit.onClick.RemoveAllListeners();
        buttonSubmit.onClick.AddListener(SubmitEvaluation);

        StartCoroutine(ForceInitialUpdate());

        Debug.Log($"[QoESliderManager] Submit button listeners: {buttonSubmit.onClick.GetPersistentEventCount()}");
    }

    private IEnumerator ForceInitialUpdate()
    {
        yield return null; // Wait one frame to ensure correct initialization
        UpdateNumericValueText();
    }

    private void UpdateNumericValueText()
    {
        numericValueText.text = $"{qoeSlider.value}/10";
    }

    public void UpdatePCInfo(string pcName, string quality, string distance)
    {
        currentPCName = pcName;
        currentQuality = quality;
        currentDistance = distance;
        pcInfoText.text = $"PC: {pcName}\nQuality: {quality}\nDistance: {distance}";
    }

    public void IncrementSlider()
    {
        if (isSliderUpdating) return;
        StartCoroutine(SliderCooldown());

        if (qoeSlider.value < qoeSlider.maxValue)
            qoeSlider.value++;
    }

    public void DecrementSlider()
    {
        if (isSliderUpdating) return;
        StartCoroutine(SliderCooldown());

        if (qoeSlider.value > qoeSlider.minValue)
            qoeSlider.value--;
    }

    private IEnumerator SliderCooldown()
    {
        isSliderUpdating = true;
        yield return new WaitForSeconds(0.25f); // Anti-double-tap delay
        isSliderUpdating = false;
    }

    public void SubmitEvaluation()
    {
        if (hasSubmittedRecently)
        {
            Debug.Log("[QoESliderManager] Skipped duplicate submit");
            return;
        }

        hasSubmittedRecently = true;
        StartCoroutine(ResetSubmitCooldown());

        int score = Mathf.RoundToInt(qoeSlider.value);
        string info = pcInfoText != null ? pcInfoText.text.Replace("\n", " | ") : "N/A";

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string line = $"{timestamp}, {info}, {score}/10";

        string filename = "QoE evaluation.csv";
        string path = Path.Combine(Application.persistentDataPath, filename);

        try
        {
            File.AppendAllText(path, line + Environment.NewLine);
            Debug.Log($"[QoESliderManager] Evaluation saved:\n{line}\nPath: {path}");

            if (saveNotification != null)
            {
                saveNotification.text = "Evaluation saved!";
                if (notificationCoroutine != null) StopCoroutine(notificationCoroutine);
                notificationCoroutine = StartCoroutine(HideNotificationRoutine(3f));
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"[QoESliderManager] Failed to save evaluation: {e.Message}");
        }
    }

    private IEnumerator ResetSubmitCooldown()
    {
        yield return new WaitForSeconds(0.3f);  // Protezione: 300 ms
        hasSubmittedRecently = false;
    }

    private IEnumerator HideNotificationRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (saveNotification != null)
        {
            saveNotification.text = "";
        }
        notificationCoroutine = null;
    }

}
