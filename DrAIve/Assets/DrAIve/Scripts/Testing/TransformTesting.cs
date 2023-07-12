using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTesting : MonoBehaviour
{

    public GameObject toInstantiate;
    public Transform road;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(this.transform.position);
            Instantiate(toInstantiate,road);
        }
    }
}
