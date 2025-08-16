using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectSelection : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TMP_Dropdown dropdown;

    public void ChangePage()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
        
        transform.GetChild(dropdown.value).gameObject.SetActive(true);
    }
}
