using System;
using UnityEngine;

public class ExitEditor : MonoBehaviour
{
    public GameObject UI;
    public static ExitEditor instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (UI.activeSelf && Input.GetKeyDown(KeyCode.Escape)) ToggleMenu();
    }

    // Start is called before the first frame update
    public void Exit()
    {
        Application.Quit();
    }

    public void ToggleMenu()
    {
        UI.SetActive(!UI.activeSelf);
    }
}
