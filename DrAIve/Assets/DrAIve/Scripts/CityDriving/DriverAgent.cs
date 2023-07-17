using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.Experimental.Rendering;
using Unity.MLAgents.Actuators;



//action space
//0:
//  0: coasting
//  1: accelerating
//  2: braking

//1:
//  0: not steering
//  1: steering left
//  2: steering right

//2:
//  0:not handbraking
//  1:handbraking


public class DriverAgent : Agent
{
    private Rigidbody rBody;

    private PrometeoCarController prometeoCarController;
    private NavRequester navRequester;

    private Vector3 nextTargetPosition;
    private Vector3 goalPosition;
    private const float MATCHING_DISTANCE = 7f;


    private int doNotEnters = 0;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        prometeoCarController = GetComponent<PrometeoCarController>();
        navRequester = GetComponent<NavRequester>();

        
    }

    public override void OnEpisodeBegin()
    {
        //set new spawn position to random position of a road
        this.transform.position = navRequester.getRandomRoadPosition();
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        rBody.rotation = Quaternion.identity;
        requestNextGoal();
    }

    private void requestNextTarget()
    {
        Transform nextTransform = navRequester.nextTransformToTarget();
        if (nextTransform != null)
        {
            nextTargetPosition = nextTransform.position;
        }
        else {
            Debug.LogWarning("Car requested a next target but there is no next target anymore");
        }
    }

    private void requestNextGoal()
    {
        navRequester.newPathToFollow();
        goalPosition = navRequester.goalTransform().position;
        requestNextTarget();
    }

    private void Update()
    {
        if (this.transform.position.y < -30f)
        {
            //car fell down
            AddReward(-100f);
            EndEpisode();
        }

        if (Vector3.Distance(transform.position, nextTargetPosition) < MATCHING_DISTANCE)
        {
            if (nextTargetPosition == goalPosition)
            {
                //reached the end of the line -> Bowser: "At the end of the line, I'll make you mine"
                AddReward(200f);
                requestNextGoal();
            }
            else
            {
                //found match -> new toGoTo
                AddReward(10f);
                requestNextTarget();
            }

        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {

        //ai observations
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(this.rBody.velocity);
        sensor.AddObservation(this.rBody.angularVelocity);
        sensor.AddObservation(this.transform.rotation);
        sensor.AddObservation(goalPosition); sensor.AddObservation(nextTargetPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        int speedOption = actions.DiscreteActions[0];
        int steeringOption = actions.DiscreteActions[1];
        int handBrakeOption = actions.DiscreteActions[2];
        Debug.Log(speedOption.ToString() + steeringOption.ToString() + handBrakeOption.ToString());
        if (speedOption == 0)
        {
            prometeoCarController.accelerating = false;
            prometeoCarController.braking = false;
        }
        else if (speedOption == 1)
        {
            prometeoCarController.accelerating = true;
            prometeoCarController.braking = false;
        }
        else
        {
            prometeoCarController.accelerating = false;
            prometeoCarController.braking = true;
        }

        if (steeringOption == 0)
        {
            prometeoCarController.goingLeft = false;
            prometeoCarController.goingRight = false;
        }
        else if (steeringOption == 1)
        {
            prometeoCarController.goingLeft = true;
            prometeoCarController.goingRight = false;
        }
        else
        {
            prometeoCarController.goingLeft = false;
            prometeoCarController.goingRight = true;
        }

        if (handBrakeOption == 0)
        {
            prometeoCarController.handbraking = false;
        }
        else { prometeoCarController.handbraking = true; }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 2;
        }
        else { discreteActions[0] = 0; }

        if (Input.GetKey(KeyCode.A))
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[1] = 2;
        }
        else{discreteActions[1] = 0; }

        if (Input.GetKey(KeyCode.Space))
        {
            discreteActions[2] = 1;
        }
        else
        {
            discreteActions[2] = 0;
        }
        Debug.Log(actionsOut);
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