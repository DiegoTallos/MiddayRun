using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorHandler : MonoBehaviour
{
    [SerializeField] bool dontLock;

    public void CursorActive()
    {
        switch(Cursor.lockState)
        {
            case CursorLockMode.None:
                if (!dontLock)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }               
                break;

            case CursorLockMode.Locked:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    void Start()
    {
        CursorActive();
    }
}
