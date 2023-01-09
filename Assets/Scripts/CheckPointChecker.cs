using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointChecker : MonoBehaviour
{
    public CarMovement theCar;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "CheckPoint")
        {
            //Debug.Log("Hit cp " + other.GetComponent<CheckPoints>().checkpointNumber);

            theCar.CheckPointHit(other.GetComponent<CheckPoints>().checkpointNumber);
        }
    }
}
