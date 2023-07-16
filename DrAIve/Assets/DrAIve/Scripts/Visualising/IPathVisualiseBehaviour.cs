using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathVisualiseBehaviour 
{
    //returns visualiser objects created
    public void visualise();
    public void stopVisualising();
    public bool isVisualising();
}
