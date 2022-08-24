using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Random = UnityEngine.Random;

public class PreyBehavior : Agent
{
  [SerializeField] private Transform predatorTransform;

    [SerializeField] private float forceMultiplier = 10f;
    
    private Rigidbody m_Rigidbody;
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.position.y < -0f)
        {
            this.m_Rigidbody.angularVelocity = Vector3.zero;
            this.m_Rigidbody.velocity  = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0f);
        }
        predatorTransform.localPosition = new Vector3(Random.value * 8 -4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // target position and agent position
        sensor.AddObservation(predatorTransform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        
        // agent velocity
        sensor.AddObservation(m_Rigidbody.velocity.x);
        sensor.AddObservation(m_Rigidbody.velocity.z);
        
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;

        // add force on the agent on x and z axis
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        m_Rigidbody.AddForce(controlSignal * forceMultiplier);
        
        // calculate distance to the target
        float distanceToTarget =
            Vector3.Distance( this.transform.localPosition,predatorTransform.localPosition);
        
        // check if the agent is close to the target

        if (distanceToTarget < 2.5f)
        {
            // deduct reward from the agent
            SetReward(-1.0f);
            
            // end the current episode to reset
            EndEpisode();
        }
        else
        {
            // give reward to the agent
            SetReward(1.0f);
            // end the current episode to reset
            EndEpisode();
        }
        
        // check if the agent falls to the void
        if (this.transform.localPosition.y < -0f)
        {
            // reset the current episode
            EndEpisode();
        }
        



    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        var continuousActionOut = actionOut.ContinuousActions;

        continuousActionOut[0] = Input.GetAxis("Horizontal");
        continuousActionOut[1] = Input.GetAxis("Vertical");
    }
    
    
}
