using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceObjectiveInteraction : MonoBehaviour
{
    [SerializeField] GameObject hud;
    [SerializeField] GameObject raceObjective;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            hud.SetActive(true);
            raceObjective.SetActive(false);
            GameManager.Instance.StartRace();
            gameObject.GetComponent<RaceObjectiveInteraction>().enabled = false;
        }
    }
}
