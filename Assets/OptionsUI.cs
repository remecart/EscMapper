using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    public GameObject UI;
    
    public void DisableUI()
    {
        UI.SetActive(false);
    }
    public void EnableUI()
    {
        UI.SetActive(true);
    }
}
