using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavRequester : MonoBehaviour
{
    private RoadNode[] pathToFollow = null;
    private int currentIndex = 0;

    private NavSolver navSolver = null;

    private void Start()
    {
        navSolver = GameObject.Find("Navigator").GetComponent<NavSolver>();
    }

    public RoadNode[] getSolution()
    {
        return pathToFollow;
    }

    public void requestNewPath(GameObject startRoad, GameObject endRoad)
    {
        List<RoadNode> solution = navSolver.SolveViaAStar(startRoad, endRoad);
        if (solution == null || solution.Count <= 0)
        {
            Debug.Log("Solution was null or empty");
            return;
        }
        else {
            pathToFollow = solution.ToArray();
            currentIndex = 0;
        }
    }

    public Transform nextTransformToTarget()
    {
        currentIndex++;
        if (currentIndex < pathToFollow.Length)
        {
            return pathToFollow[currentIndex].road.transform.Find("Nav").transform;
        }
        else
        {
            return null;
        }
    }
}
