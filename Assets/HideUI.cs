using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour
{
    private bool hide;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            hide = !hide;
        }
        
        transform.GetChild(0).gameObject.SetActive(!hide);
        transform.GetChild(1).gameObject.SetActive(!hide);
    }
}
