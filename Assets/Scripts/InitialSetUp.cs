using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSetUp : MonoBehaviour
{
    void SetSavedScreenMode()
    {
        switch (PlayerPrefs.GetString("ScreenMode"))
        {
            case "FullScreen":
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case "Windowed":
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
    
    void Start()
    {
        SetSavedScreenMode();
    }
}
