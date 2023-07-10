using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoadNode
{
    //if it has a road above, it has to be accessible. So if one way road goes to two way road. The one way road will have 
    // the two way road as roadBelow but the two way road won't have to one way road as road Below.
    public GameObject road;
    public GameObject roadAbove;
    public GameObject roadBelow;
    public GameObject roadLeft;
    public GameObject roadRight;
    public int roadLength;
}

public class NodeMatch
{
    public GameObject road;
    public GameObject matchedNode;
}

public class NavSolution
{
    //class that contains the length of the solution and a list of all the roads
    public int pathLength = 0;
    public List<GameObject> roads = new();
}

public class NavSolver: MonoBehaviour
{
    private Dictionary<string, RoadNode> navigationMap;

    private void Start()
    {
        float MATCH_DISTANCE_THRESHOLD = 0.6f;
        //load the whole nav system for all the road pieces
        navigationMap = new Dictionary<string, RoadNode>();
        GameObject[] roads = GameObject.FindGameObjectsWithTag("Road");
        //go through all roads and nodes of the roads and find the closest one to each node of the road piece
        foreach (GameObject road in roads)
        {
            RoadNode roadNode = new RoadNode();
            roadNode.road = road;
            roadNode.roadLength = road.GetComponent<RoadLength>().length;

            Transform navParentNodeTR = road.transform.Find("Nav");
            foreach (Transform navNodeTR in navParentNodeTR)
            {
                NodeMatch match = findMatchToNavNode(navNodeTR, road,roads,MATCH_DISTANCE_THRESHOLD);
                if (match != null)
                {
                    TrafficDirection direction = match.matchedNode.GetComponent<TrafficDirection>();
                    if (direction == null)
                    {
                        Debug.Log("Didn't find TrafficDirection component for match: " + match.matchedNode.name);
                    }
                    if (direction.direction != DirectionOption.OneWayOnlyOut)
                    {
                        float xDistance = road.transform.position.x - match.road.transform.position.x;
                        float yDistance = road.transform.position.y - match.road.transform.position.y;
                        if (MathF.Abs(xDistance) > Mathf.Abs(yDistance))
                        {
                            //in the x direction
                            if (xDistance > 0)
                            {
                                //towards the left
                                roadNode.roadLeft = match.road;
                            }
                            else
                            {
                                //towards the right
                                roadNode.roadRight = match.road;
                            }
                        }
                        else
                        {
                            //in the y direction
                            if (yDistance > 0)
                            {
                                //towards bottom
                                roadNode.roadBelow = match.road;
                            }
                            else
                            {
                                roadNode.roadAbove = match.road;
                            }
                        }
                    }
                    //else, it is a one way which we aren't allowed to enter
                }
                else
                {
                    //Debug.Log("Didn't find a further path for road: " + road.name + " and node: " + navNodeTR.name);
                }




            }
            navigationMap.Add(road.name, roadNode);
        }


    }

    private NodeMatch findMatchToNavNode(Transform navNodeTR, GameObject road, GameObject[] roads, float MATCH_DISTANCE_THRESHOLD )
    {
        foreach (GameObject road2 in roads)
        {
            if (road2 != road)
            {
                foreach (Transform navNodeTR2 in road2.transform.Find("Nav"))
                {
                    if (Vector3.Distance(navNodeTR.position, navNodeTR2.position) < MATCH_DISTANCE_THRESHOLD)
                    {
                        //It should be a match
                        NodeMatch match = new NodeMatch();
                        match.road = road2;
                        match.matchedNode = navNodeTR2.gameObject;
                        return match;

                    }
                }
            }
        }
        return null;
    }

     

    public NavSolution solveNav(GameObject startRoad, GameObject endRoad)
    {
        int ROAD_USE_LIMIT = 2; //A car can only visit the same road piece twice

        List<NavSolution> solutions = new List<NavSolution>();
        NavSolution startSolution = new NavSolution();
        startSolution.roads.Add(startRoad);
        startSolution.pathLength = 0;
        solutions.Add(startSolution);

        //strategy, go from one node recursive. But solve always all things one step more untill the shortest solution has been found
        int iterations = 0;

        while (true)
        {
            iterations++;
            if (iterations > 100000)
            {
                Debug.Log("Reached max iterations");
                return null;
            }
            if (solutions.Count <= 0)
            {
                //didn't find a solution and list is empty
                Debug.LogWarning("Couldn't find a solution to reach: " + endRoad.name + " from: " + startRoad.name);
                return null;
            }

            NavSolution possibleSolution = solutions[0];
            //remove that solution from the list, if we can move on from there we will find the way, if we can't no point in bothering
            solutions.RemoveAt(0);
            GameObject lastRoad = possibleSolution.roads[possibleSolution.roads.Count - 1];
            //for each node check what the possible next steps are
            RoadNode targetNode;
            navigationMap.TryGetValue(lastRoad.name, out targetNode);
            if (targetNode != null)
            {
                GameObject[] roadOptions = {
                        targetNode.roadAbove, targetNode.roadBelow, targetNode.roadLeft, targetNode.roadRight
                    };
                foreach (GameObject road in roadOptions)
                {
                    if (road != null)
                    {
                        //create new solution option
                        RoadNode nodeForLength;
                        navigationMap.TryGetValue(road.name, out nodeForLength);
                        if (nodeForLength != null)
                        {
                            //first check if node wasn't more then limit in the solution. Don't want to keep going back and forwards
                            int currentOcurrances = possibleSolution.roads.Count(x => x == road);
                            if (currentOcurrances < ROAD_USE_LIMIT) {
                                List<GameObject> roadsClone = new List<GameObject>(possibleSolution.roads);
                                roadsClone.Add(road);
                                NavSolution newSolution = new NavSolution();
                                newSolution.roads = roadsClone;
                                newSolution.pathLength = possibleSolution.pathLength + nodeForLength.roadLength;
                                solutions.Add(newSolution);
                                if (road.name == lastRoad.name)
                                {
                                    //found the solution !
                                    return newSolution;
                                }
                            }                            
                        }
                        else
                        {
                            Debug.LogWarning("Didn't find requested node in the navigationMap, name was:" + lastRoad.name + " and some node already pointed to it so this really shouldn't happen");


                        }
                    }
                }
            }
            else
            {
                //something went wrong
                Debug.LogWarning("Didn't find requested node in the navigationMap, name was:" + lastRoad.name);
            }
            //sort it with the smallest in front
            solutions = solutions.OrderBy(x => x.pathLength).ToList();


        }
    }
}
