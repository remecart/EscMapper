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
}
