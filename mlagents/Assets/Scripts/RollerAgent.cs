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
    public int maxActions = 5000;
    public GameObject[] spawnPositions;
    public Collider spawnArea;
    private GameObject[] sceneColliders;
    private GameObject[] traps;
    public RayPerceptionSensorComponent3D raycast;


    // Start is called before the first frame update
    void Start()
    {
        sceneColliders = GameObject.FindGameObjectsWithTag("laberint");
        traps = GameObject.FindGameObjectsWithTag("trap");
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {

        // Spawn method 5 spawns
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        int numRandom = Random.Range((int)0.0f, (int)spawnPositions.Length);
        this.transform.localPosition = spawnPositions[numRandom].transform.position;

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

        if (rBody.velocity.x < 0.2f && rBody.velocity.z < 0.2f)
        {
            SetReward(-0.1f);
        }

        if (this.transform.localPosition.y < 0.0f)
        {
            Debug.Log("Fallen");
            numActions = 0;
            SetReward(-0.8f);
            EndEpisode();
        }
        else if (numActions >= maxActions)
        {
            Debug.Log("Max actions reached");
            numActions = 0;
            SetReward(-0.3f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "trap")
        {
            Debug.Log("Hit trap");
            numActions = 0;
            SetReward(-0.8f);
            EndEpisode();
        }
        else if (collision.gameObject.tag == "laberint")
        {
            Debug.Log("Hit laberint");
            SetReward(-0.6f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "reward")
        {
            Debug.Log("Got Reward");
            SetReward(1.0f);
            numActions = 0;
            EndEpisode();
        }
    }
}
