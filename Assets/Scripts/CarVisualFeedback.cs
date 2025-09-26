using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVisualFeedback : MonoBehaviour
{
    [SerializeField] float chassisTurnAngle;
    [SerializeField] float chassisTurnSpeed;
    
    [SerializeField] Transform chassis;
    [SerializeField] GameObject trails;

    CarController car;
    
    void ChassisTurnAngle()
    {
        if (car.leftSteering)
        {
            Turn(360 - chassisTurnAngle);
        }
        else if (car.rightSteering)
        {
            Turn(chassisTurnAngle);
        }
        else if (!car.steering)
        {
            Turn(0);
        }

        void Turn(float turnAngle)
        {
            float angle = Mathf.LerpAngle(chassis.eulerAngles.z, turnAngle, chassisTurnSpeed * Time.deltaTime);
            chassis.eulerAngles = new Vector3(chassis.eulerAngles.x, chassis.eulerAngles.y, angle);
        }
    }

    void BrakingTrails(bool braking)
    {        
        trails.SetActive(braking);
    }

    void BrakeSuspensionResponse(bool braking)
    {
        float angle;
        
        switch (braking)
        {
            case true:
                angle = Mathf.LerpAngle(chassis.eulerAngles.x, 2, chassisTurnSpeed * Time.deltaTime);
                chassis.eulerAngles = new Vector3(angle, chassis.eulerAngles.y, chassis.eulerAngles.z);
                break;

            case false:
                angle = Mathf.LerpAngle(chassis.eulerAngles.x, 0, chassisTurnSpeed * Time.deltaTime);
                chassis.eulerAngles = new Vector3(angle, chassis.eulerAngles.y, chassis.eulerAngles.z);
                break;
        }       
    }

    void Update()
    {
        ChassisTurnAngle();
        BrakingTrails(car.braking);
        BrakeSuspensionResponse(car.braking);
    }

    void Start()
    {
        car = GetComponent<CarController>();
    }
}
