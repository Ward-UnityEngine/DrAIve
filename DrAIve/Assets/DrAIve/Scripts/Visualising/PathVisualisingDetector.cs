using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualisingDetector : MonoBehaviour
{
    [SerializeField] private bool enablePathVisualiser;

    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //check if gameobject has a visualise me component
                IPathVisualiseBehaviour visualiseBehaviour = hit.transform.gameObject.GetComponent<IPathVisualiseBehaviour>();
                if (visualiseBehaviour != null)
                {
                    if (!visualiseBehaviour.isVisualising())
                    {
                        visualiseBehaviour.visualise();
                    }
                    else { 
                        visualiseBehaviour.stopVisualising();
                    }
                }
            }
        }
    }
}
