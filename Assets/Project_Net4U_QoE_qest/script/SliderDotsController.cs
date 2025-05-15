using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SliderDotsController : MonoBehaviour
{
    public Slider slider;
    public List<Image> dots = new();
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;

    void Start()
    {
        slider.onValueChanged.AddListener(UpdateDots);
        UpdateDots(slider.value); // Inizializza correttamente allo start
    }

    void UpdateDots(float value)
    {
        int currentIndex = Mathf.Clamp(Mathf.RoundToInt(value) - 1, 0, dots.Count - 1); // da 1/10 → 0-based

        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].color = i == currentIndex ? activeColor : inactiveColor;
        }
    }
}
