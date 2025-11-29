using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreviewHardcoded : MonoBehaviour
{
    public TMP_Dropdown map;

    public void ChangeMap()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
        this.transform.GetChild(map.value).gameObject.SetActive(true);
    }

    private void Update()
    {
        if (MapManager.instance.currentLayer == 0) this.transform.position = new Vector3(0, 1000000, 0);
        else this.transform.position = new Vector3(0, 0, 0);
    }
}
