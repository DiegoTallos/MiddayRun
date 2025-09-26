using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteRecordsButton : MonoBehaviour
{
    public void OnButtonPress()
    {
        GameManager.Instance.saveManager.ResetRecords();
    }
}
