using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    int checkpoint;
    int checkpointCount;

    float lastLapTime;

    void OnTriggerEnter(Collider other)
    {       
        if (other.gameObject.GetComponent<RaceProgress>() != null)
        {
            RaceProgress obj = other.gameObject.GetComponent<RaceProgress>();

            if (obj.gameObject.layer == 3 && obj.checkpoint == checkpoint - 1)
            {
                obj.checkpoint = checkpoint;

                if (checkpoint == checkpointCount)
                {
                    obj.checkpoint = 0;
                    obj.lap += 1;

                    GameManager.Instance.SaveLapRecord(Time.time - GameManager.Instance.timeOffset - lastLapTime);

                    if (lastLapTime == 0)
                    {
                        lastLapTime = Time.time - GameManager.Instance.timeOffset;
                    }
                    else if (lastLapTime >= Time.time - GameManager.Instance.timeOffset - lastLapTime)
                    {
                        lastLapTime = Time.time - GameManager.Instance.timeOffset - lastLapTime;
                    }

                    //GameObject.Find("UI").GetComponent<UiController>().RecordTimes();
                    GameManager.Instance.ui.RecordTimes();
                }

                if (obj.lap == GameManager.Instance.laps)
                {
                    GameManager.Instance.SaveRaceRecord(Time.time - GameManager.Instance.timeOffset);
                    GameManager.Instance.ui.RecordTimes();
                    GameManager.Instance.lastLapTime = lastLapTime;
                    GameManager.Instance.FinishRace();
                }                
            }
        }              
    }

    void Start()
    {
        checkpoint = int.Parse(gameObject.name);
        checkpointCount = transform.parent.childCount;
    }
}
