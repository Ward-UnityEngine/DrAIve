using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavSolver
{
    public GameObject startRoad;
    public GameObject endRoad;

    public NavSolver(GameObject startRoad, GameObject endRoad)
    {
        this.startRoad = startRoad;
        this.endRoad = endRoad;
    }

    public void solveNav()
    {
        List<List<GameObject>> solutions = new List<List<GameObject>>();
        List<GameObject> firstNode = new List<GameObject>
        {
            startRoad
        };
        solutions.Add(firstNode);
        //strategy, go from one node recursive. But solve always all things one step more untill the shortest solution has been found
        bool solutionNotFound = true;
        while (solutionNotFound)
        {
            //go one node further
            foreach (List<GameObject> possibleSolution in solutions)
            {
                GameObject lastNode = possibleSolution.[possibleSolution.Count-1];
                NavPathNode navPathNode = new NavPathNode(lastNode, null);
                navPathNode.solveNode();
            }
        }
    }
}
