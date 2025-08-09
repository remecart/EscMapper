using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreventPlaceBehindGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static PreventPlaceBehindGUI instance;
    public bool behindUI;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        behindUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        behindUI = false;
    }
}
