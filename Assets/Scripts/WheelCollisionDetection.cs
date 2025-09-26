using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelCollisionDetection : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float distance;
    
    void SphereCheckGround()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, radius, -transform.up, out hit, distance))
        {
            Debug.Log(hit.transform.name);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * distance, radius);
    }

    void Update()
    {
        SphereCheckGround();
    }
}
