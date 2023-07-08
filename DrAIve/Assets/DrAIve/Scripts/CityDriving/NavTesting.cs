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
        NavSolver navSolver = new NavSolver(startRoad, endRoad);
        navSolver.solveNav();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spawnAndFind();
        }
    }
}
