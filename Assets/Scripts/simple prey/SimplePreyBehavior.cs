using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimplePreyBehavior : Agent
{
  [SerializeField] private Transform foodTransform;

    [SerializeField] private float forceMultiplier = 10f;
    
    private Rigidbody m_Rigidbody;

    private RayPerceptionSensorComponent3D m_RayPerceptionSensorComponent3D;
    private RayPerceptionSensor m_RayPerceptionSensor;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_RayPerceptionSensorComponent3D = GetComponent<RayPerceptionSensorComponent3D>();
        m_RayPerceptionSensor = m_RayPerceptionSensorComponent3D.RaySensor;
    }

    public override void OnEpisodeBegin()
    {
        if (this.transform.position.y < -0f)
        {
            this.m_Rigidbody.angularVelocity = Vector3.zero;
            this.m_Rigidbody.velocity  = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0f);
        }
        foodTransform.position = new Vector3(Random.value * 10 -1, 0.5f, Random.value * 10 - 1);
         // var output = _rayPerceptionSensor.RayPerceptionOutput;
         // var targetLocation =  output.RayOutputs[0].HitGameObject.transform.position;

         // var r = m_RayPerceptionSensorComponent3D.RaySensor;
         // var z = r.RayPerceptionOutput.RayOutputs[0].HitGameObject.transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // // target position and agent position
        // sensor.AddObservation(foodTransform.localPosition);
        // sensor.AddObservation(this.transform.localPosition);
        //
        //check each ray if it hits a target
        foreach (var raySensor in m_RayPerceptionSensor.RayPerceptionOutput.RayOutputs)
        {
            sensor.AddObservation(raySensor.HitGameObject.transform.position);
        }
        
        
        // agent velocity
        sensor.AddObservation(m_Rigidbody.velocity.x);
        sensor.AddObservation(m_Rigidbody.velocity.z);
        // sensor.AddObservation(this.transform.rotation.y);
        
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        // Vector3 controlSignalRotation = Vector3.zero;
        

        // add force on the agent on x and z axis
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        // controlSignalRotation.y = actionBuffers.ContinuousActions[2];

        //set rotation
        // Quaternion rotation = Quaternion.Euler(0, controlSignalRotation.y, 0);
        // this.transform.rotation = rotation;
        
        m_Rigidbody.AddForce(controlSignal * forceMultiplier);
        
        // calculate distance to the target
        float distanceToTarget =
            Vector3.Distance( this.transform.position,foodTransform.position);
        
        // check if the agent is close to the target

        if (distanceToTarget < 1.5f)
        {
            // give reward to the agent
            SetReward(1.0f);
            
            // end the current episode to reset
            EndEpisode();
        }
        else
        {
            // deduct reward from the agent
            // SetReward(-0.2f); 
        }
        
        // check if the agent falls to the void
        if (this.transform.position.y < -0f)
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

    private Transform FindTheFoodLocation(RayPerceptionOutput.RayOutput[] rayOutputs)
    {
        var count = rayOutputs.Length;

        for (int i = 0; i < count; i++)
        {
           // find the closest one 
        }

        // the following is just not to show an error
        return rayOutputs[0].HitGameObject.transform;
    }
    
}
