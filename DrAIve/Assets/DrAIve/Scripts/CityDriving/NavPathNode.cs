using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNode
{
    public GameObject nextRoad;
    public GameObject nextNavNode;
}

public class NavPathNode
{
    private GameObject currentRoad;
    private GameObject currentNavNode;

    public NavPathNode(GameObject currentRoad, GameObject currentNavNode)
    { 
        this.currentRoad = currentRoad; 
        this.currentNavNode = currentNavNode;
    }

    public List<NextNode> solveNode()
    {
        List<NextNode> result = new List<NextNode>();
        foreach (GameObject navNode in currentRoad.transform.Find("Nav"))
        {
            //for each node that isn't the current node on the road piece, we can go there 
            if (navNode != null && navNode != currentNavNode)
            {
                NextNode nextNode = new NextNode();
                //check which node is close by -> that is the next node

            }
        }

        return result;


    }


}
