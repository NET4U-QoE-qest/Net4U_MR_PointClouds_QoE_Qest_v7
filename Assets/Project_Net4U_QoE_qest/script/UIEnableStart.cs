using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class UIEnableStart : MonoBehaviour
{
    void Start()
    {
        var eventSystem = EventSystem.current;
        var inputModule = eventSystem?.currentInputModule as InputSystemUIInputModule;

        if (inputModule != null)
        {
            inputModule.point?.action?.Enable();
            inputModule.leftClick?.action?.Enable();
            inputModule.middleClick?.action?.Enable();
            inputModule.rightClick?.action?.Enable();
            inputModule.scrollWheel?.action?.Enable();
            inputModule.submit?.action?.Enable();
            inputModule.cancel?.action?.Enable();
            inputModule.move?.action?.Enable();
        }
    }
}
