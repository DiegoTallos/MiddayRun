using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera _camera;

    [SerializeField] Vector3 chaseCameraPosition, chaseCameraBackView;

    [SerializeField] float fovSpeedFactor;

    float setFov;

    CarController car;
   
    void ChaseCamera()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GetComponent<Camera>().transform.position = chaseCameraBackView;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            GetComponent<Camera>().transform.position = chaseCameraPosition;
        }
    }

    void SpeedCameraChange()
    {
        _camera.fieldOfView = setFov + car.GetCurrentSpeed() / 10 + fovSpeedFactor;
    }

    void Update()
    {
        SpeedCameraChange();
    }

    void Start()
    {
        car = GetComponent<CarController>();
        setFov = _camera.fieldOfView;
    }
}
