using UnityEngine;
using UnityEngine.UI;

public class ButtonDebug : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log("Submit button clicked!");
            });
        }
        else
        {
            Debug.LogWarning(" Not foud Button in " + gameObject.name);
        }
    }
}
