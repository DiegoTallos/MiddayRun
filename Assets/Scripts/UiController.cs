using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [Header("Gameobjects References")]
    [SerializeField] GameObject hud;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject resultsScreen;

    [Header("Text References")]
    [SerializeField] TextMeshProUGUI speed;
    [SerializeField] TextMeshProUGUI gear;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] TextMeshProUGUI lapCounter;
    [SerializeField] TextMeshProUGUI fpsCounter;
    [SerializeField] TextMeshProUGUI totalRecord;
    [SerializeField] TextMeshProUGUI fastestLap;
    [SerializeField] TextMeshProUGUI centeredText;
    [SerializeField] TextMeshProUGUI[] lapTimeTexts;
    [SerializeField] TextMeshProUGUI[] totalTimeTexts;

    [Header("Audio References")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuPopup;
    [SerializeField] AudioClip buttonClick;

    [Header("Other References")]
    [SerializeField] CarController car;
    [SerializeField] RaceProgress raceProgress;
    [SerializeField] CursorHandler cursorHandler;

    [Header("RPM")]
    public Transform rpmNeedle;
    public float minNeedleRotation;
    public float maxNeedleRotation;

    AudioManager audioManager;

    void UpdatePlayerHudElements()
    {
        speed.text = car.GetCurrentSpeed().ToString();
        rpmNeedle.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(minNeedleRotation, maxNeedleRotation, car.rpm / (car.redLine * 1.1f)));
        gear.text = (car.gearState == GearState.Neutral) ? "N" : (car.currentGear + 1).ToString();

        if (car.reversing)
        {
            gear.text = "R";
        }

        string[] getTimer = GameManager.Instance.TimeConvertToString(Time.time);
        timer.text = getTimer[2] + ":" + getTimer[1] + ":" + getTimer[0];

        RecordTimes();

        lapCounter.text = raceProgress.lap + 1 + "/" + GameManager.Instance.laps;        
        fpsCounter.text = ((int)(1f / Time.smoothDeltaTime) + 1).ToString();
    }

    public void SetPauseMenu()
    {
        switch(pauseMenu.activeSelf)
        {
            case false:
                Time.timeScale = 0;
                cursorHandler.CursorActive();
                hud.SetActive(false);
                pauseMenu.SetActive(true);
                PlayAudio(menuPopup); //Remplazar
                audioManager.PauseAudio(audioManager.music);
                break;

            case true:
                Resume();                
                break;           
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        cursorHandler.CursorActive();
        hud.SetActive(true);
        pauseMenu.SetActive(false);       
        PlayAudio(menuPopup); //remplzar
        audioManager.PlayAudio(audioManager.music, true);
    }

    public void ToMainMenu()
    {
        PlayAudio(buttonClick);
        SceneManager.LoadScene(0);
    }

    public void ExitToWindows()
    {
        PlayAudio(buttonClick);
        Application.Quit();
    }

    void PlayAudio(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.Play();
    }

    public void RecordTimes()
    {
        if (PlayerPrefs.GetFloat("FastestLap") != 0) 
        {
            string[] timer = GameManager.Instance.TimeConvertToString(PlayerPrefs.GetFloat("FastestLap"));
            fastestLap.text = timer[2] + ":" + timer[1] + ":" + timer[0];
        }
        else
        {
            fastestLap.text = "-- : -- : --";
        }

        if (PlayerPrefs.GetFloat("TotalRecord") != 0)
        {
            string[] timer = GameManager.Instance.TimeConvertToString(PlayerPrefs.GetFloat("TotalRecord"));
            totalRecord.text = timer[2] + ":" + timer[1] + ":" + timer[0];
        }
        else
        {
            totalRecord.text = "-- : -- : --";
        }
    }

    public void SetCenteredText(string text)
    {
        centeredText.text = text;   
    }

    public void SetHudActive(bool state)
    {
        hud.gameObject.SetActive(state);
    }

    public void ShowResultsScreen()
    {
        SetHudActive(false);
        resultsScreen.SetActive(true);
    }

    public void SetLapTimes(string[] originalTime, string[] diferenceTime, string[] finalTime, bool newRecord)
    {
        lapTimeTexts[0].text = originalTime[2] + ":" + originalTime[1] + ":" + originalTime[0];
        lapTimeTexts[1].text = diferenceTime[2] + ":" + diferenceTime[1] + ":" + diferenceTime[0];
        lapTimeTexts[2].text = finalTime[2] + ":" + finalTime[1] + ":" + finalTime[0];

        switch (newRecord)
        {
            case true:
                lapTimeTexts[1].text = lapTimeTexts[1].text.Insert(0, "- ");
                lapTimeTexts[1].color = Color.green;
                break;

            case false:
                lapTimeTexts[1].text = lapTimeTexts[1].text.Insert(0, "+ ");
                lapTimeTexts[1].color = Color.red;
                break;
        }
    }

    public void SetTotalTimes(string[] originalTime, string[] diferenceTime, string[] finalTime, bool newRecord)
    {
        totalTimeTexts[0].text = originalTime[2] + ":" + originalTime[1] + ":" + originalTime[0];
        totalTimeTexts[1].text = diferenceTime[2] + ":" + diferenceTime[1] + ":" + diferenceTime[0];
        totalTimeTexts[2].text = finalTime[2] + ":" + finalTime[1] + ":" + finalTime[0];

        switch (newRecord)
        {
            case true:
                totalTimeTexts[1].text = totalTimeTexts[1].text.Insert(0, "- ");
                totalTimeTexts[1].color = Color.green;
                break;

            case false:
                totalTimeTexts[1].text = totalTimeTexts[1].text.Insert(0, "+ ");
                totalTimeTexts[1].color = Color.red;
                break;
        }
    }

    void Update()
    {
        UpdatePlayerHudElements();
    }

    void Start()
    {
        audioManager = GameManager.Instance.audioManager;
    }
}
