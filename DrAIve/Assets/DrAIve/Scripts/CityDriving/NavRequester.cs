using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public void newPathToFollow()
    {
        Dictionary<string, RoadNode> navigationMap = navSolver.getNavigationMap();
        float min_distance_to_car = float.MaxValue;
        GameObject closest_road = null;
        foreach (RoadNode roadNode in navigationMap.Values)
        {
            GameObject road = roadNode.road;
            float simple_dist = HelperMethods.simpleDistance(road.transform.position, this.transform.position);
            if (simple_dist < min_distance_to_car)
            {
                min_distance_to_car = simple_dist;
                closest_road = road;
            }
        }
        GameObject startRoad = closest_road;
        GameObject endRoad = closest_road;
        while (endRoad == startRoad)
        {
            int randomIndex = Random.Range(0, navigationMap.Count - 1);
            endRoad = navigationMap.ElementAt(randomIndex).Value.road;
        }
        requestNewPath(startRoad, endRoad);
    }

    public Transform nextTransformToTarget()
    {
        currentIndex++;
        if (currentIndex >= pathToFollow.Length)
        {
            return null;
        }
        return pathToFollow[currentIndex].road.transform.Find("Nav").transform;
    }

    public Transform goalTransform()
    {
        return pathToFollow[pathToFollow.Length - 1].road.transform.Find("Nav").transform;
    }

    public Vector3 getRandomRoadPosition()
    {
        Dictionary<string, RoadNode> navigationMap = navSolver.getNavigationMap();
        return navigationMap.ElementAt(Random.Range(0, navigationMap.Count - 1)).Value.road.transform.Find("Nav").transform.position;
    }

}
