using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NavTesting : MonoBehaviour
{
    public GameObject roadParent;
    private GameObject[] spawnPositions;
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
        spawnPositions = new GameObject[roadParent.transform.childCount];

        foreach (Transform childSpawn in roadParent.transform)
        {
            spawnPositions[index] = childSpawn.transform.gameObject;
            index++;
        }
    }



    private void spawnAndFind() {
        int randomIndex = Random.Range(0, spawnPositions.Length - 1);
        GameObject startRoad = spawnPositions[randomIndex];
        int newIndex = randomIndex;
        while (newIndex == randomIndex)
        {
            newIndex = Random.Range(0, spawnPositions.Length - 1);
        }
        GameObject endRoad = spawnPositions[newIndex];
        spawnAndFindIndex(randomIndex, newIndex);
    }

    private void spawnAndFindIndex(int randomIndex, int newIndex)
    {
        GameObject startRoad = spawnPositions[randomIndex];
        GameObject endRoad = spawnPositions[newIndex];
        s1 = Instantiate(GreenSphere, startRoad.transform.position, startRoad.transform.rotation);
        s2 = Instantiate(BlueSphere, endRoad.transform.position, startRoad.transform.rotation);
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
                roads.Add(Instantiate(PurpleSphere, roadPiece.transform.position, roadPiece.transform.rotation));
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
            spawnAndFindIndex(30,35);
        }
    }
}
