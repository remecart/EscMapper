using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenNewReleases : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text textMesh;

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL("https://github.com/remecart/EscMapper/releases/latest");
    }
}