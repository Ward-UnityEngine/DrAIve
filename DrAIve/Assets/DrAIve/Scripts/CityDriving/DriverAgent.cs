using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.Experimental.Rendering;
using Unity.MLAgents.Actuators;

enum RayHitEnum { DoNotEnterZone, Road };

struct RayResult
{
    public float distance;
    public RayHitEnum rayHitType;
}


public class DriverAgent : Agent
{
    private Rigidbody rBody;
    public GameObject spawnPositionsParent;
    private Transform[] spawnPositions;

    public GameObject rayTargetParent;
    private Transform[] rayTargetDirections;

    private int doNotEnters = 0;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        //set spawn positions from parent and raytargets
        int index = 0;
        spawnPositions = new Transform[spawnPositionsParent.transform.childCount];

        foreach (Transform childSpawn in spawnPositionsParent.transform)
        {
            spawnPositions[index] = childSpawn.transform;
            index++;
        }

        index = 0;
        rayTargetDirections = new Transform[rayTargetParent.transform.childCount];
        foreach (Transform childRay in rayTargetParent.transform)
        {
            rayTargetDirections[index] = childRay.transform;
            index++;
        }
        calculateVectorObservationRequiredSize();
    }

    private int calculateVectorObservationRequiredSize()
    {
        int size = 0;
        size += rayTargetDirections.Length * 2; //every element has a distance and a number defining the type of collision
        size += 1;//velocity
        //still want to add own position and the position of the next waypoint
        Debug.Log("The observation space should be of length = " + size);
        return size;
    }

    public override void OnEpisodeBegin()
    {
        //set position to one of the spawnpositions
        int randomIndex = Random.Range(0, spawnPositions.Length - 1);
        rBody.position = spawnPositions[randomIndex].position;
        rBody.rotation = spawnPositions[randomIndex].rotation;

        //set velocity to zero
        rBody.velocity = Vector3.zero;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //rayCasts
        RayResult[] rayObservations = sendRayCasts();
        foreach (RayResult rayResult in rayObservations)
        {
            sensor.AddObservation(rayResult.distance);
            sensor.AddObservation((float)rayResult.rayHitType);
        }
        //add own velocity
        sensor.AddObservation(rBody.velocity);

        //add vector that points to the next target
        //Vector3 nextTarget = 
        //add own forward transform -> hopefully it learns to angle it towards the correct place
        //sensor.AddObservation(this.transform.forward - nextTarget. );
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        checkForEndOfEpisode();
    }


    private void checkForEndOfEpisode()
    {
        
    }


    private RayResult[] sendRayCasts()
    {
        RayResult[] rayResults = new RayResult[rayTargetDirections.Length];
        int index = 0;

        foreach (Transform rayTarget in rayTargetDirections)
        {
            Ray ray = new Ray(this.transform.position, rayTarget.position);
            RaycastHit result;
            if (Physics.Raycast(ray, out result))
            {
                RayResult res = new RayResult();
                res.distance = result.distance;
                if (result.transform.CompareTag("DoNotEnterZone"))
                {
                    res.rayHitType = RayHitEnum.DoNotEnterZone;
                }
                else if (result.transform.CompareTag("Road"))
                {
                    res.rayHitType = RayHitEnum.Road;
                }
                rayResults[index] = res;
                index++;
            }
        }

        return rayResults;
    }

    public void doNotEnterUpdate(bool entered)
    {
        if (entered)
        {
            doNotEnters++;
        }
        else
        {
            doNotEnters--;
        }
    }
}