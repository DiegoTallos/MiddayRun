using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject options;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OptionsMenu()
    {
        switch (options.activeSelf)
        {
            case true:
                LoadOptions(false);
                break;

            case false:
                LoadOptions(true);
                break;
        }
    }

    public void ExitGame()
    {
        Application.Quit(); 
    }

    void LoadOptions(bool load)
    {
        switch(load)
        {
            case true:
                menu.SetActive(false);
                options.SetActive(true);               
                break;

            case false:
                menu.SetActive(true);
                options.SetActive(false);
                break;
        }
    }

    void Start()
    {
        LoadOptions(false);
    }
}
