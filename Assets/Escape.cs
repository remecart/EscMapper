using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    public List<GameObject> menus;
    // Update is called once per frame
    private bool shit = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            shit = false;
            foreach (var menu in menus)
            {
                if (menu.activeInHierarchy)
                {
                    shit = true;
                }
            }

            if (shit == true)
            {
                foreach (var menu in menus)
                {
                    menu.SetActive(false);
                }
                MapSettingsMenu.instance.inMenu = false;
            }
            else
            {
                menus[0].SetActive(true);
                MapSettingsMenu.instance.inMenu = true;
            }
        }
    }
}
