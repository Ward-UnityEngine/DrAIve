using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotEnterTriggerBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Player has hit a do not enter zone
            other.gameObject.GetComponent<DriverAgent>().doNotEnterUpdate(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Player has left a do not enter zone
            other.gameObject.GetComponent<DriverAgent>().doNotEnterUpdate(false);
        }
    }
}
