using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static Instance
    public static GameManager Instance;

    public int laps;

    private GameManager() { }      

    float originalLapTime;
    float originalTotalTime;  

    [HideInInspector] public float setLapTime;
    [HideInInspector] public float setTotalTime;
    [HideInInspector] public float timeOffset;
    [HideInInspector] public float lastLapTime;

    [HideInInspector] public bool gameEnded;

    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public SaveManager saveManager;
    [HideInInspector] public UiController ui;

    PlayerInput playerInput;
    CinematicRaceStartCamera cinematicCamera;
    
    AudioClip clip;

    public void StartRace()
    {
        StartCoroutine(RaceIEnumerator());
    }

    IEnumerator RaceIEnumerator()
    {
        cinematicCamera.CameraRotation(3, 10);

        for (int i = 3; i > 0; i--)
        {
            audioManager.PlayAudio(clip, audioManager.general, false);
            ui.SetCenteredText(i.ToString());            
            yield return new WaitForSeconds(1);
        }

        audioManager.PlayAudio(Resources.Load<AudioClip>("Audio/Sounds/countdown_go"), audioManager.general, false);
        ui.SetCenteredText("GO!");

        playerInput.inputLocked = false;

        timeOffset = Time.time;

        GameObject.Find("Cameras").GetComponent<CameraHandler>().SetChaseCamera();

        audioManager.PlayAudio(Resources.Load<AudioClip>("Audio/Music/SaveMe"), audioManager.music, true);

        yield return new WaitForSeconds(1);

        ui.SetCenteredText("");        
    } 

    public string[] TimeConvertToString(float time)
    {
        string[] timerString = new string[3];
        time -= timeOffset;
        
        if (time.ToString().Contains(","))
        {
            string[] tempText = time.ToString("F3").Split(",");
            timerString[0] = tempText[1];
        }

        float seconds = (int)time;

        seconds = seconds % 60;

        string secondsString = seconds.ToString();

        switch (secondsString.Length)
        {
            case 1:
                timerString[1] = "0" + secondsString;
                break;

            default:
                timerString[1] = secondsString;
                break;
        }

        timerString[2] = ((int)time / 60).ToString();

        return timerString;
    }

    public int[] TimeConvertToInt(float time)
    {
        int[] timerInt = new int[3];
        string[] timerString = TimeConvertToString(time);

        timerInt[0] = int.Parse(timerString[0]);
        timerInt[1] = int.Parse(timerString[1]);
        timerInt[2] = int.Parse(timerString[2]);

        return timerInt;
    }

    public float LoadSavedLapRecord()
    {
        float record = PlayerPrefs.GetFloat("FastestLap");
        return record;
    }

    public float LoadSavedRaceRecord()
    {
        float record = PlayerPrefs.GetFloat("TotalRecord");
        return record;
    }

    public void SaveLapRecord(float record)
    {        
        if (record < LoadSavedLapRecord() || LoadSavedLapRecord() == 0 || record == 0)
        {
            PlayerPrefs.SetFloat("FastestLap", record);
            PlayerPrefs.Save();
        }
    }

    public void SaveRaceRecord(float record)
    {
        if (record < LoadSavedRaceRecord() || LoadSavedRaceRecord() == 0 || record == 0)
        {
            PlayerPrefs.SetFloat("TotalRecord", record);
            PlayerPrefs.Save();
        }
    }

    public void FinishRace()
    {
        Time.timeScale = 0;
        
        audioManager.PlayAudio(Resources.Load<AudioClip>("Audio/Music/race_completed_mu"), audioManager.music, true);

        int[] oldLapTime = TimeConvertToInt(originalLapTime);
        int[] currentLapTime = TimeConvertToInt(lastLapTime);

        int[] oldTotalTime = TimeConvertToInt(originalTotalTime);
        int[] currentTotalTime = TimeConvertToInt(Time.time - timeOffset);

        if (originalLapTime != 0 || originalTotalTime != 0)
        {
            string[] lapTimeDifference = new string[3];

            lapTimeDifference[0] = NumberDifference(oldLapTime[0], currentLapTime[0]).ToString();
            lapTimeDifference[1] = NumberDifference(oldLapTime[1], currentLapTime[1]).ToString();
            lapTimeDifference[2] = NumberDifference(oldLapTime[2], currentLapTime[2]).ToString();

            string[] totalTimeDifference = new string[3];

            totalTimeDifference[0] = NumberDifference(oldTotalTime[0], currentTotalTime[0]).ToString();
            totalTimeDifference[1] = NumberDifference(oldTotalTime[1], currentTotalTime[1]).ToString();
            totalTimeDifference[2] = NumberDifference(oldTotalTime[2], currentTotalTime[2]).ToString();

            ui.SetLapTimes(TimeConvertToString(originalLapTime), lapTimeDifference, TimeConvertToString(lastLapTime), lastLapTime < originalLapTime);
            ui.SetTotalTimes(TimeConvertToString(originalTotalTime), totalTimeDifference, TimeConvertToString(Time.time - timeOffset), Time.time - timeOffset < originalTotalTime);
        }
     
        ui.ShowResultsScreen();

        playerInput.inputLocked = true;
        gameEnded = true;
    }

    int NumberDifference(int num1, int num2)
    {
        int count;

        count = Mathf.Max(num2, num1) - Mathf.Min(num1, num2);

        return count;
    }

    void GetReferences()
    {       
        audioManager = transform.GetChild(0).GetComponent<AudioManager>();
        saveManager = transform.GetChild(1).GetComponent<SaveManager>();
        cinematicCamera = FindObjectOfType<CinematicRaceStartCamera>(); ;
        playerInput = FindObjectOfType<PlayerInput>();
        ui = FindObjectOfType<UiController>();
        clip = Resources.Load<AudioClip>("Audio/Sounds/countdown");
        originalLapTime = PlayerPrefs.GetFloat("FastestLap");
        originalTotalTime = PlayerPrefs.GetFloat("TotalRecord");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetReferences();
        Time.timeScale = 1;
    }
}
