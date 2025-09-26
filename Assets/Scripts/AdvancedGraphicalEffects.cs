using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AdvancedGraphicalEffects : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float dofAdjustment;
    [SerializeField] float dofCap;

    [Header("Display Information")]
    [SerializeField] float focusLength;
    
    Volume postproccesing;
    CarController car;
    DepthOfField dof;

    void Update()
    {
        if (car != null)
        {
            dof.focalLength.value = Mathf.Clamp(car.GetCurrentSpeed() - dofAdjustment, 0, dofCap);
            focusLength = dof.focalLength.value;
        }      
    }

    void Start()
    {
        postproccesing = GetComponent<Volume>();
        postproccesing.profile.TryGet<DepthOfField>(out dof);

        if (GameObject.Find("Car") != null)
        {
            car = GameObject.Find("Car").GetComponent<CarController>();
        }       
    }
}
