using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectSelection : MonoBehaviour
{
    public int page;
    public TextMeshProUGUI text;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangePage(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) ChangePage(1);
    }

    public void ChangePage(int change)
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        page = Mathf.Clamp(page + change, 0, 8);
        
        transform.GetChild(page).gameObject.SetActive(true);
        text.text = transform.GetChild(page).name;
    }
}
