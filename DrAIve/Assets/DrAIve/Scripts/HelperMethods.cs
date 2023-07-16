using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperMethods
{
    public static float simpleDistance(Vector3 pos1, Vector3 pos2)
    {
        float distance = 0f;
        distance += Mathf.Abs(pos1.x - pos2.x);
        distance += Mathf.Abs(pos1.y - pos2.y);
        distance += Mathf.Abs(pos1.z - pos2.z);
        return distance;
    }
}
