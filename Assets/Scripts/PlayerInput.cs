using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] CarController car;
    [SerializeField] UiController ui;

    public bool inputLocked;

    void GetInputs()
    {
        if (!inputLocked)
        {
            // Acceleration Input

            car.accelerating = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S);
            car.reversing = Input.GetKey(KeyCode.S);

            car.accelerator = Mathf.Abs(Input.GetAxis("Vertical"));

            // Steering Input

            if (Input.GetKey(KeyCode.A))
            {
                car.leftSteering = true;
            }
            else
            {
                car.leftSteering = false;
            }

            if (Input.GetKey(KeyCode.D))
            {
                car.rightSteering = true;
            }
            else
            {
                car.rightSteering = false;
            }

            if (car.leftSteering || car.rightSteering)
            {
                car.steering = true;
            }
            else
            {
                car.steering = false;
            }

            // Breaking Input

            if (Input.GetKey(KeyCode.Space))
            {
                car.braking = true;
            }
            else
            {
                car.braking = false;
            }

            // Turn on lights Input

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!car.frontLightsOn)
                {
                    foreach (Light light in car.frontLights)
                    {
                        light.gameObject.SetActive(true);
                    }

                    car.frontLightsOn = true;
                }
                else
                {
                    foreach (Light light in car.frontLights)
                    {
                        light.gameObject.SetActive(false);
                    }

                    car.frontLightsOn = false;
                }
            }

            // Respawn

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }

            // Pause Menu

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ui.SetPauseMenu();
            }
            
            /*
            if (Input.GetKeyDown(KeyCode.V))
            {
                GameManager.Instance.FinishRace();
            }
            */
        }

        // Continue from Results

        if (GameManager.Instance.gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.Instance.audioManager.MuteAllAudio();
                SceneManager.LoadScene(0);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                GameManager.Instance.audioManager.MuteAllAudio();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void Update()
    {
        GetInputs();
    }
}
