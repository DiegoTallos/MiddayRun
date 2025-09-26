using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CarController car;
    [SerializeField] CinemachineVirtualCamera cam;

    [Header("Configuration")]
    [SerializeField] float shakeIntensity;

    CinemachineBasicMultiChannelPerlin perlin;

    void Update()
    {
        perlin.m_FrequencyGain = Mathf.Clamp(((float)car.GetCurrentSpeed() / 100) - 1, 0, 1.5f);
        perlin.m_AmplitudeGain = shakeIntensity;
    }

    void Start()
    {
        perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
}
