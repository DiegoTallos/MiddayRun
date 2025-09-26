using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
}

public class CarController : MonoBehaviour
{
    [Header("Engine")]
    public float horsePower;
    public Drivetrain drivetrain;
    public enum Drivetrain
    {
        FrontDrive,
        RearDrive,
        AllWheelDrive
    }
    public Transmission transmission;
    public enum Transmission
    {
        Automatic,
        Manual
    }    

    [Header("Brakes")]
    [SerializeField] float frontBreakForce;
    [SerializeField] float rearBreakForce;
    [SerializeField] float passiveBreakForce;

    [Header("Gearbox")]
    public float[] gearRatios;
    public AnimationCurve hpToRpmCurve;
    public float redLine;
    public float idleRpm;    
    public float differentialRatio;
    public float changeGearTime = 0.5f;
    public float increaseGearRPM;
    public float decreaseGearRPM;

    [Header("Antiroll")]
    [SerializeField] float antiRoll;
    [SerializeField] float downForce;

    [Header("Wheel Configuration")]
    [SerializeField] float minSteerAngle;
    [SerializeField] float maxSteerAngle;
    [SerializeField] float steerAngleReduction;
    [SerializeField] float minSteerSpeed;
    [SerializeField] float maxSteerSpeed;
    [SerializeField] float steerSpeedReduction;
    [SerializeField] float wheelMass;    
    [SerializeField] float currentSteer;
    [SerializeField] float currentSteerSpeed;

    [Header("Assists")]
    [SerializeField] float centerAssistStrength;
    [SerializeField] float centerAssistSteeringStrength;

    [Header("Forward Friction")]
    [SerializeField] float forwardExtremumSlip = 0.4f;
    [SerializeField] float forwardExtremumValue = 1;
    [SerializeField] float forwardAsymptoteSlip = 0.8f;
    [SerializeField] float forwardAsymptoteValue = 0.5f;
    [SerializeField] float forwardStiffness;

    [Header("Sideways Friction")]
    [SerializeField] float sidewaysExtremumSlip = 0.2f;
    [SerializeField] float sidewaysExtremumValue = 1;
    [SerializeField] float sidewaysAsymptoteSlip = 0.5f;
    [SerializeField] float sidewaysAsymptoteValue = 0.75f;
    [SerializeField] float sidewaysStiffness;

    [Header("Advanced Configuration")]
    [SerializeField] bool defaultSettings;   
    [SerializeField] [Range(0.1f, 60000)] float frontSpringRate;
    [SerializeField] [Range(0.1f, 60000)] float rearSpringRate;
    [SerializeField] [Range(1, 20)] float frontStiffness;
    [SerializeField] [Range(1, 20)] float rearStiffness;

    [Header("References")]
    [SerializeField] Transform centerOfMass;
    [SerializeField] Transform[] wheelVisuals;
    public WheelCollider[] wheelColliders;    
    public Light[] frontLights;
    [SerializeField] Material brakeLightsMat;
    [SerializeField] Camera chaseCamera;
    
    public float accelerator;
    [HideInInspector] public float rpm;
    [HideInInspector] public int currentGear;

    public bool accelerating;
    public bool reversing;
    [HideInInspector] public bool steering;
    [HideInInspector] public bool leftSteering;
    [HideInInspector] public bool rightSteering;
    [HideInInspector] public bool braking;
    [HideInInspector] public bool frontLightsOn;
    [HideInInspector] public bool rearLightsOn;

    public GearState gearState;

    float clutch;
    public float currentTorque;
    float wheelRpm;

    Rigidbody rb;

    void VehiculeControl()
    {
        Accelerate();
        Steer();
        Brake();
        Antiroll();
        LimitSteer();
        AddDownForce();
        UpdateWheels();

        void Accelerate()
        {
            if (gearState != GearState.Changing) 
            {
                if (gearState == GearState.Neutral)
                {
                    clutch = 0;

                    if (accelerator > 0)
                    {
                        gearState = GearState.Running;
                    }
                }
                else
                {
                    clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
                }                
            }
            else
            {
                clutch = 0;
            }

            currentTorque = CalculateTorque();
            
            if (reversing)
            {
                currentTorque = -currentTorque / 2;
            }

            if (accelerating)
            {
                SetDrivetrainTorque();
            }            
            else if (!steering)
            {
                ResetWheelTorque();
                ApplyBrakes(passiveBreakForce, passiveBreakForce);
            }

            void SetDrivetrainTorque()
            {
                switch (drivetrain)
                {
                    case Drivetrain.FrontDrive:
                        SetWheelTorque(currentTorque, currentTorque, 0, 0);
                        break;

                    case Drivetrain.RearDrive:
                        SetWheelTorque(0, 0, currentTorque, currentTorque);
                        break;

                    case Drivetrain.AllWheelDrive:
                        SetWheelTorque(currentTorque, currentTorque, currentTorque, currentTorque);
                        break;
                }
            }
           
            float CalculateTorque()
            {
                float torque = 0;

                if (rpm < idleRpm + 200 && accelerator == 0 && currentGear == 0)
                {
                    gearState = GearState.Neutral;
                }

                if (gearState == GearState.Running && clutch > 0) 
                {
                    if (rpm > increaseGearRPM && !reversing)
                    {
                        StartCoroutine(ChangeGear(1));                     
                    }
                    else if (rpm < decreaseGearRPM)
                    {
                        StartCoroutine(ChangeGear(-1));
                    }
                }

                if (clutch < 0.1)
                {
                    rpm = Mathf.Lerp(rpm, Mathf.Max(idleRpm, redLine * accelerator) + Random.Range(-50, 50), Time.deltaTime);
                }
                else
                {
                    wheelRpm = Mathf.Abs((wheelColliders[2].rpm + wheelColliders[3].rpm) / 2) * gearRatios[currentGear] * differentialRatio;
                    rpm = Mathf.Lerp(rpm, Mathf.Max(idleRpm - 100, wheelRpm), Time.deltaTime * 3);
                    torque = (hpToRpmCurve.Evaluate(rpm / redLine) * horsePower / rpm) * gearRatios[currentGear] * differentialRatio * 5252 * clutch;
                }

                return torque;
            }
            
            void SetWheelTorque(float frontLeftTorque, float frontRightTorque, float rearLeftTorque, float rearRightTorque)
            {
                wheelColliders[0].motorTorque = frontLeftTorque;
                wheelColliders[1].motorTorque = frontRightTorque;
                wheelColliders[2].motorTorque = rearLeftTorque;
                wheelColliders[3].motorTorque = rearRightTorque;
            }

            void ResetWheelTorque()
            {
                foreach (WheelCollider wheel in wheelColliders)
                {
                    wheel.motorTorque = 0;
                }
            }
        }

        void Steer()
        {
            if (rightSteering)
            {
                WheelRotation(wheelColliders[0], currentSteer);
                WheelRotation(wheelColliders[1], currentSteer);
            }
            else if (leftSteering)
            {
                WheelRotation(wheelColliders[0], -currentSteer);
                WheelRotation(wheelColliders[1], -currentSteer);
            }
            else 
            {
                WheelRotation(wheelColliders[0], 0);
                WheelRotation(wheelColliders[1], 0);
            }


            void WheelRotation(WheelCollider wheel, float minSteer)
            {
                wheel.steerAngle = Mathf.Lerp(wheel.steerAngle, minSteer, currentSteerSpeed * Time.deltaTime);
            }
        }

        void Brake()
        {
            if (braking)
            {
                ApplyBrakes(frontBreakForce, rearBreakForce);
                BrakeLights(true);
            }
            else if (accelerating)
            {
                ApplyBrakes(0, 0);                
                rb.drag = 0;
            }

            if (!braking)
            {
                BrakeLights(false);
            }           
        }

        void UpdateWheels()
        {
            for (int i = 0; i < wheelColliders.Length; i++)
            {
                wheelColliders[i].GetWorldPose(out Vector3 pos, out Quaternion rot);
                wheelVisuals[i].transform.position = pos;
                wheelVisuals[i].transform.rotation = rot;
            }
        }

        void ApplyBrakes(float frontBrakeForce, float rearBreakForce)
        {
            wheelColliders[0].brakeTorque = frontBrakeForce;
            wheelColliders[1].brakeTorque = frontBrakeForce;
            wheelColliders[2].brakeTorque = rearBreakForce;
            wheelColliders[3].brakeTorque = rearBreakForce;
        }

        IEnumerator ChangeGear(int gearChange)
        {
            gearState = GearState.CheckingChange;
            if (currentGear + gearChange >= 0)
            {
                if (gearChange > 0)
                {
                    yield return new WaitForSeconds(0.7f);

                    if (rpm < increaseGearRPM || currentGear >= gearRatios.Length - 1)
                    {
                        gearState = GearState.Running;
                        yield break;
                    }                 
                }
                if (gearChange < 0)
                {
                    yield return new WaitForSeconds(0.1f);

                    if (rpm > decreaseGearRPM || currentGear <= 0)
                    {
                        gearState = GearState.Running;
                        yield break;
                    }
                }

                gearState = GearState.Changing;
                yield return new WaitForSeconds(changeGearTime);
                currentGear += gearChange;
            }

            if (gearState != GearState.Neutral)
            {
                gearState = GearState.Running;
            }           
        }
        
        void Antiroll()
        {
            WheelHit hit;
            float travelL = 1.0f;
            float travelR = 1.0f;


            bool groundedL = wheelColliders[2].GetGroundHit(out hit);
            if (groundedL)
            {
                travelL = (-wheelColliders[2].transform.InverseTransformPoint(hit.point).y - wheelColliders[2].radius) / wheelColliders[2].suspensionDistance;
            }

            bool groundedR = wheelColliders[3].GetGroundHit(out hit);
            if (groundedR)
            {
                travelR = (-wheelColliders[3].transform.InverseTransformPoint(hit.point).y - wheelColliders[3].radius) / wheelColliders[3].suspensionDistance;
            }

            float antiRollForce = (travelL - travelR) * antiRoll;

            if (groundedL)
                rb.AddForceAtPosition(wheelColliders[2].transform.up * -antiRollForce, wheelColliders[2].transform.position);

            if (groundedR)
                rb.AddForceAtPosition(wheelColliders[3].transform.up * antiRollForce, wheelColliders[3].transform.position);
        }

        void LimitSteer()
        {
            currentSteer = Mathf.Clamp(maxSteerAngle - rb.velocity.magnitude * steerAngleReduction, minSteerAngle, maxSteerAngle);
            currentSteerSpeed = Mathf.Clamp(maxSteerSpeed - (rb.velocity.magnitude * steerSpeedReduction) / 100, minSteerSpeed, maxSteerSpeed);
        }

        void AddDownForce()
        {
            rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
        }
    }

    void VehiculeStartUp()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.transform.localPosition;
        }
        else
        {
            rb.automaticCenterOfMass = true;
        }
    }

    void SetAdvancedSettings()
    {
        WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
        WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();

        foreach(WheelCollider wheel in wheelColliders)
        {
            wheel.mass = wheelMass;

            forwardFriction.extremumSlip = forwardExtremumSlip;
            forwardFriction.extremumValue = forwardExtremumValue;
            forwardFriction.asymptoteSlip = forwardAsymptoteSlip;
            forwardFriction.asymptoteValue = forwardAsymptoteValue;
            forwardFriction.stiffness = forwardStiffness;

            sidewaysFriction.extremumSlip = sidewaysExtremumSlip;
            sidewaysFriction.extremumValue = sidewaysExtremumValue;
            sidewaysFriction.asymptoteSlip = sidewaysAsymptoteSlip;
            sidewaysFriction.asymptoteValue = sidewaysAsymptoteValue;
            sidewaysFriction.stiffness = sidewaysStiffness;

            wheel.forwardFriction = forwardFriction;
            wheel.sidewaysFriction = sidewaysFriction;
        }  
    }

    void BrakeLights(bool turnOn)
    {
        switch(turnOn)
        {
            case true:
                brakeLightsMat.SetFloat("_Light_Intensity", 120);
                break;

            case false:
                brakeLightsMat.SetFloat("_Light_Intensity", 0);
                break;
        }
    }

    public int GetCurrentSpeed()
    {
        return Mathf.RoundToInt(rb.velocity.magnitude * 2.23694f);
    }

    void FixedUpdate()
    {
        VehiculeControl();
    }

    void Start()
    {
        VehiculeStartUp();
        SetAdvancedSettings();
    }
}
