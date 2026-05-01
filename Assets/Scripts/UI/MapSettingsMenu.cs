using System;
using System.Collections.Generic;
using UnityEngine;

public class MapSettingsMenu : MonoBehaviour
{
    public static MapSettingsMenu instance;

    public bool inMenu;

    public GameObject Menu;
    public List<GameObject> Pages;

    public GameObject helpMenu;
    public bool help;
    
    private void Start()
    {
        instance = this;
    }
    
    public void CloseHelpMenu()
    {
        help = !help;
        helpMenu.SetActive(help);
    }

    public void ToggleMenu()
    {
        inMenu = !inMenu;
        Menu.SetActive(inMenu);
        Pages[0].SetActive(true);
        Pages[1].SetActive(false);
    }

    public void ChangePage(int value)
    {
        var page = value == 0 ? false : true;
        Pages[0].SetActive(!page);
        Pages[1].SetActive(page);
    }
}
