using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCoDriver : MonoBehaviour
{
    private Transform toGoTo;
    private NavRequester navRequester;

    private const float MATCHING_DISTANCE = 7f;

    public GameObject roadParent;
    private GameObject[] roadObjects;

    private void Start()
    {
        navRequester = GetComponent<NavRequester>();

        //set spawn positions from parent and raytargets
        int index = 0;
        roadObjects = new GameObject[roadParent.transform.childCount];

        foreach (Transform childSpawn in roadParent.transform)
        {
            roadObjects[index] = childSpawn.transform.gameObject;
            index++;
        }
    }

   

    private void Update()
    {
        if (toGoTo == null)
        {
            getNewRandomRoute();
            toGoTo = navRequester.nextTransformToTarget();
        }
        if (Vector3.Distance(transform.position, toGoTo.position) < MATCHING_DISTANCE)
        {
            //found match -> new toGoTo
            toGoTo = navRequester.nextTransformToTarget();
        }
    }


    private void getNewRandomRoute() {
        //new route
        //first check which road is closest to car
        float min_distance_to_car = float.MaxValue;
        GameObject closest_road = null;
        foreach (GameObject road in roadObjects)
        {
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
            int randomIndex = Random.Range(0, roadObjects.Length - 1);
            endRoad = roadObjects[randomIndex];
        }
        navRequester.requestNewPath(startRoad, endRoad);
    }
}
