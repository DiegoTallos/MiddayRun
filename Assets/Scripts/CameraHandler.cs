using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] GameObject chaseCamera;
    [SerializeField] GameObject cinematicCamera;

    public void SetChaseCamera()
    {
        chaseCamera.SetActive(true);
        cinematicCamera.SetActive(false);
    }
}
