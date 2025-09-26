using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInitialSetUp : MonoBehaviour
{
    [SerializeField] GameObject hud;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject raceObjective;
    [SerializeField] GameObject centeredText;
    [SerializeField] GameObject resultsScreen;

    void Start()
    {
        hud.SetActive(false);
        pauseMenu.SetActive(false);
        raceObjective.SetActive(true);
        centeredText.SetActive(true);
        resultsScreen.SetActive(false);
    }
}
