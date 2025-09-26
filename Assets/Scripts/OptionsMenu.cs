using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("References")]   
    [SerializeField] TMPro.TMP_Dropdown qualityDropdown;
    [SerializeField] TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] TMPro.TMP_Dropdown screenDropdown;

    [SerializeField] Transform tabs;
    GameObject videoTab;
    GameObject gameplayTab;
    GameObject controlsTab;
    GameObject audioTab;

    public void QualityChange() // Cambia la calidad grafica a la seleccionada en el dropdown
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value, true);
    }

    public void ResolutionChange() // Cambia la resolucion pantalla a la seleccionada
    {
        string targetResolution = resolutionDropdown.captionText.text;

        Resolution[] resolutions = Screen.resolutions;

        foreach (Resolution resolution in resolutions)
        {
            if (resolution.ToString().Replace(" @ 60Hz", "") == targetResolution)
            {
                Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.FullScreenWindow);
                PlayerPrefs.SetString("ScreenResolution", resolution.ToString());
                PlayerPrefs.Save();
            }
        }       
    }

    public void ScreenChange() // Cambia el modo de pantalla al seleccionado
    {
        switch (screenDropdown.value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                PlayerPrefs.SetString("ScreenMode", "FullScreen");
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                PlayerPrefs.SetString("ScreenMode", "Windowed");
                break;
        }        
    }

    void GetUserResolutions() // Recoge todas las resoluciones soportadas por el monitor en el dropdown
    {
        Resolution[] resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> resolutionList = new List<string>();

        foreach (Resolution resolution in resolutions)
        {
            if (resolution.refreshRateRatio.value == 60)
            {
                string resolutionString = resolution.ToString().Replace(" @ 60Hz", "");

                resolutionList.Add(resolutionString);
            }            
        }

        resolutionDropdown.AddOptions(resolutionList);

        if (PlayerPrefs.HasKey("ScreenResolution"))
        {
            resolutionList.Add(PlayerPrefs.GetString("ScreenResolution").Replace(" @ 60Hz", ""));
            resolutionDropdown.AddOptions(resolutionList);
            resolutionDropdown.value = resolutionDropdown.options.Count;
        }
    }

    public void ChangeTab(int index)
    {
        switch (index)
        {
            case 0:
                TabActivation(true, false, false, false);
                break;

            case 1:
                TabActivation(false, true, false, false);
                break;

            case 2:
                TabActivation(false, false, true, false);
                break;

            case 3:
                TabActivation(false, false, false, true);
                break;
        }

        void TabActivation(bool videoActive, bool gameplayActive, bool controlsActive, bool audioActive)
        {
            videoTab.SetActive(videoActive);
            gameplayTab.SetActive(gameplayActive);
            controlsTab.SetActive(controlsActive);
            audioTab.SetActive(audioActive);
        }
    }

    void SetPlayerScreenMode()
    {
        switch (Screen.fullScreen)
        {
            case true:
                screenDropdown.value = 0;
                break;

            case false:
                screenDropdown.value = 1;
                break;
        }
    }

    void SetPlayerGraphicQuality()
    {
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    void OnEnable()
    {
        GetUserResolutions();
        SetPlayerScreenMode();
        SetPlayerGraphicQuality();
    }

    private void Start()
    {
        videoTab = tabs.GetChild(0).gameObject;
        gameplayTab = tabs.GetChild(1).gameObject;
        controlsTab = tabs.GetChild(2).gameObject;
        audioTab = tabs.GetChild(3).gameObject;
    }
}
