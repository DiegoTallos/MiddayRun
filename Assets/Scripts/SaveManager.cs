using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public void ResetRecords()
    {
        PlayerPrefs.SetFloat("FastestLap", 0);
        PlayerPrefs.SetFloat("TotalRecord", 0);
        PlayerPrefs.Save();
    }
}
