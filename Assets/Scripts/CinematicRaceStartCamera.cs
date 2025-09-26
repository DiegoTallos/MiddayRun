using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicRaceStartCamera : MonoBehaviour
{
    [SerializeField] float cameraHeight;
    [SerializeField] float cameraDistance;
    [SerializeField] float rotationSpeed;

    [SerializeField] Transform carTransform;

    Transform childCamera;
    Tween tweenTransform;
    
    void Update()
    {
        childCamera.transform.LookAt(transform);       
    }

    public void CameraRotation(float camY, float camZ)
    {
        tweenTransform.Kill();
        transform.position = carTransform.position;
        childCamera.localPosition = new Vector3(childCamera.position.x, camY, camZ);
        tweenTransform = transform.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
        tweenTransform.Play();              
    }

    private void OnDisable()
    {
        tweenTransform.Kill();
    }

    void Start()
    {       
        childCamera = transform.GetChild(0);
        CameraRotation(cameraDistance, cameraDistance);
    }
}
