using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



//A* path node
public class RoadNode
{
    public int gCost = 0;
    public int hCost = 0;  
    public int fCost = 0;

    //if it has a road above, it has to be accessible. So if one way road goes to two way road. The one way road will have 
    // the two way road as roadBelow but the two way road won't have to one way road as road Below.
    public GameObject road;
    public Vector3 navPosition = Vector3.zero;
    public GameObject roadAbove;
    public GameObject roadBelow;
    public GameObject roadLeft;
    public GameObject roadRight;
    public int roadLength = 0;

    public RoadNode cameFromNode; //to be used when going back from the eventual solution

    public RoadNode(GameObject road)
    {
        this.road = road;
        this.navPosition = road.transform.Find("Nav").position;
    }

    public int CaculateFCost()
    {
        fCost = gCost + hCost;
        return fCost;
    }
}

public class NodeMatch
{
    public GameObject road;
    public GameObject matchedNode;
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
            RoadNode roadNode = new RoadNode(road);
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
                        float xDistance = navParentNodeTR.position.x - navNodeTR.position.x;
                        float zDistance = navParentNodeTR.position.z - navNodeTR.position.z;
                        if (MathF.Abs(xDistance) > Mathf.Abs(zDistance))
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
                            if (zDistance > 0)
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

    public List<RoadNode> SolveViaAStar(GameObject startRoad, GameObject endRoad)
    {
        RoadNode startRoadNode;
        navigationMap.TryGetValue(startRoad.name, out startRoadNode);
        RoadNode endRoadNode;
        navigationMap.TryGetValue(endRoad.name, out endRoadNode);


        if (startRoadNode == null || endRoadNode == null)
        {
            //invalid path
            return null;
        }

        List<RoadNode> openNodes = new();
        openNodes.Add(startRoadNode);
        List<RoadNode> closedNodes = new();

        //initialize nodes
        foreach (KeyValuePair<string, RoadNode> kvp in navigationMap)
        {
            kvp.Value.gCost = int.MaxValue;
            kvp.Value.hCost = 0;
            kvp.Value.CaculateFCost();
            kvp.Value.cameFromNode = null;
        }

        //start with the start node
        startRoadNode.gCost = 0;
        startRoadNode.hCost = CalculateDistanceCost(startRoadNode, endRoadNode);
        startRoadNode.CaculateFCost();

        while (openNodes.Count > 0)
        {
            //keep looping and find that solution
            RoadNode currentNode = GetLowestFCostNode(openNodes);
            if (currentNode == endRoadNode)
            {
                //reached final node
                return CalculatePath(endRoadNode);
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            foreach (RoadNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (!closedNodes.Contains(neighbourNode))
                {
                    //still need to be calculated
                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endRoadNode);
                        neighbourNode.CaculateFCost();

                        if (!openNodes.Contains(neighbourNode))
                        {
                            openNodes.Add(neighbourNode);
                        }
                    }
                }
            }
        }

        //out of nodes on the openlist, didn't find a solution

        

        return null;
    }

    private List<RoadNode> GetNeighbourList(RoadNode currentNode)
    {
        List<RoadNode> neighbours = new();
        GameObject[] roadOptions = {
                        currentNode.roadAbove, currentNode.roadBelow, currentNode.roadLeft, currentNode.roadRight
                    };
        foreach (GameObject roadOption in roadOptions)
        {
            if (roadOption != null)
            {
                RoadNode node;
                navigationMap.TryGetValue(roadOption.name, out node);
                neighbours.Add(node);
            }
        }

        return neighbours;
    }

    private int CalculateDistanceCost(RoadNode a, RoadNode b)
    {
        float dist = 0;
        dist += Mathf.Abs(a.navPosition.x - b.navPosition.x);
        dist += Mathf.Abs(a.navPosition.y - b.navPosition.y);
        dist += Mathf.Abs(a.navPosition.z - b.navPosition.z);
        dist = 100 * dist; //to up the distance, otherwise rather small values
        return Mathf.RoundToInt(dist);
    }

    private RoadNode GetLowestFCostNode(List<RoadNode> nodes)
    {
        RoadNode lowestFCostNode = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodes[i];
            }
        }
        return lowestFCostNode;
    }

    private List<RoadNode> CalculatePath(RoadNode endNode)
    {
        List<RoadNode> path = new();
        path.Add(endNode);
        RoadNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    public Dictionary<string, RoadNode> getNavigationMap()
    {
        return navigationMap;
    }


     

    /*public NavSolution solveNav(GameObject startRoad, GameObject endRoad)
    {
        int ROAD_USE_LIMIT = 1; //A car can only visit the same road piece twice

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
            if (iterations > 10000)
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
                                if (road.name == endRoad.name)
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
    }*/
}
