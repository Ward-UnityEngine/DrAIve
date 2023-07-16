using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NavTesting : MonoBehaviour
{
    public GameObject roadParent;
    private GameObject[] roadObjects;
    public GameObject GreenSphere;
    public GameObject BlueSphere;
    public GameObject PurpleSphere;

    private GameObject s1;
    private GameObject s2;
    private List<GameObject> roads = new List<GameObject>();
    void Start()
    {
        //set spawn positions from parent and raytargets
        int index = 0;
        roadObjects = new GameObject[roadParent.transform.childCount];

        foreach (Transform childSpawn in roadParent.transform)
        {
            roadObjects[index] = childSpawn.transform.gameObject;
            index++;
        }
    }



    private void spawnAndFind() {
        int randomIndex = Random.Range(0, roadObjects.Length - 1);
        GameObject startRoad = roadObjects[randomIndex];
        int newIndex = randomIndex;
        while (newIndex == randomIndex)
        {
            newIndex = Random.Range(0, roadObjects.Length - 1);
        }
        GameObject endRoad = roadObjects[newIndex];
        Debug.Log("Finding for indexes: " + randomIndex + ", " + newIndex);
        spawnAndFindIndex(randomIndex, newIndex);
    }

    private void spawnAndFindIndex(int randomIndex, int newIndex)
    {
        GameObject startRoad = roadObjects[randomIndex];
        GameObject endRoad = roadObjects[newIndex];
        s1 = Instantiate(GreenSphere, startRoad.transform.Find("Nav").transform);
        s2 = Instantiate(BlueSphere, endRoad.transform.Find("Nav").transform);
        roads = new();
        NavSolver navSolver = GameObject.Find("Navigator").GetComponent<NavSolver>();
        if (navSolver != null)
        {
            List<RoadNode> solution = navSolver.SolveViaAStar(startRoad, endRoad);
            if (solution == null || solution.Count <= 0)
            {
                Debug.Log("Solution was null or empty");
                return;
            }

            foreach (RoadNode roadNode in solution)
            {
                GameObject roadPiece = roadNode.road;
                roads.Add(Instantiate(PurpleSphere, roadNode.navPosition, Quaternion.identity));
            }
        }
        else
        {
            Debug.LogWarning("Can't solve navigation meshes when there is no Navigator");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(s1);
            Destroy(s2);
            foreach (GameObject roadPiece in roads)
            {
                Destroy(roadPiece);
            }
            spawnAndFind();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Destroy(s1);
            Destroy(s2);
            foreach (GameObject roadPiece in roads)
            {
                Destroy(roadPiece);
            }
            spawnAndFindIndex(5,16);
        }
    }
}
