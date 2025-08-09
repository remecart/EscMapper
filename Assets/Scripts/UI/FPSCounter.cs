using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        text.text = $"FPS: {fps.ToString("F2", CultureInfo.InvariantCulture)}";
    }
}