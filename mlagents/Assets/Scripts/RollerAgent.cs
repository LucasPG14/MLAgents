using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAgent : Agent
{
    // variables
    Rigidbody rBody;
    public Transform target;
    public float forceMultiplier;
    public int numActions = 0;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y < 0.0f)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
        }

        // Move the target
        target.localPosition = new Vector3(Random.value * 8.0f - 4.0f, 0.5f, Random.value * 8.0f - 4.0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and agent information for the position
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        // Agent information for the velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Distance left to the target
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);

        numActions++;
        Debug.Log("Num Actions: " + numActions);

        // Check if we reached the target or we have fallen
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            numActions = 0;
            EndEpisode();
        }
        else if (this.transform.localPosition.y < 0.0f)
        {
            numActions = 0;
            EndEpisode();
        }
        else if (numActions >= 1000)
        {
            numActions = 0;
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
