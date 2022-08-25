using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimplePreyBehavior : Agent
{
  [SerializeField] private Transform foodTransform;

    [SerializeField] private float forceMultiplier = 500f;
    [SerializeField] private float rotationMultiplier = 60f;
    
    private Rigidbody m_Rigidbody;

    private RayPerceptionSensorComponent3D m_RayPerceptionSensorComponent3D;
    private RayPerceptionSensor m_RayPerceptionSensor;
    
    float rotationAmount = 0f;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        // m_RayPerceptionSensorComponent3D = GetComponent<RayPerceptionSensorComponent3D>();
        // m_RayPerceptionSensor = m_RayPerceptionSensorComponent3D.RaySensor;
        // Debug.Log(m_RayPerceptionSensor.RayPerceptionOutput.RayOutputs);
    }

    public override void OnEpisodeBegin()
    {
        this.m_Rigidbody.angularVelocity = Vector3.zero;
        this.m_Rigidbody.velocity  = Vector3.zero;
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        foodTransform.localPosition = new Vector3(Random.Range(-9,9), 0.5f, Random.Range(-9,9));

        rotationAmount = 0f;
        // var output = _rayPerceptionSensor.RayPerceptionOutput;
        // var targetLocation =  output.RayOutputs[0].HitGameObject.transform.localPosition;

        // var r = m_RayPerceptionSensorComponent3D.RaySensor;
        // var z = r.RayPerceptionOutput.RayOutputs[0].HitGameObject.transform.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // // target localPosition and localPosition 
        sensor.AddObservation(foodTransform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        //
        //check each ray if it hits a target
        // foreach (var raySensor in m_RayPerceptionSensor.RayPerceptionOutput.RayOutputs)
        // {
        //     sensor.AddObservation(raySensor.HitGameObject.transform.localPosition);
        // }

        // Vector3 targetForward = foodTransform.forward;
        // float directionDot = Vector3.Dot(this.transform.forward, targetForward);
        // sensor.AddObservation(directionDot);
        // agent velocity
        sensor.AddObservation(m_Rigidbody.velocity.x);
        sensor.AddObservation(m_Rigidbody.velocity.z);
        // sensor.AddObservation(rotationAmount);
        // sensor.AddObservation(forceMultiplier);
        // sensor.AddObservation(this.transform.rotation.y);
        
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        // Vector3 controlSignalRotation = Vector3.zero;

        var forwardForce = 0f;
        

        // add force on the agent on x and z axis
        controlSignal.x = actionBuffers.DiscreteActions[0];
        controlSignal.z = actionBuffers.DiscreteActions[1];

        switch (actionBuffers.DiscreteActions[0])
        {
            case 0: forwardForce = 0f; break;;
            case 1: forwardForce = 1f;
                print("forward going"); break;;
            case 2: forwardForce = -1f; break;;
        }    
        switch (actionBuffers.DiscreteActions[1])
        {
            case 0: rotationAmount = 0f;  break;;
            case 1: rotationAmount += 1f; break;;
            case 2: rotationAmount += -1f; break;;
        }
        // switch (actionBuffers.DiscreteActions[2])
        // {
        //     case 0: forceMultiplier += 1f; break;;
        //     case 1: forceMultiplier += -1f; break;;
        // } 
        // switch (actionBuffers.DiscreteActions[3])
        // {
        //     case 0: rotationMultiplier += 1f; break;;
        //     case 1: rotationMultiplier += -1f; break;;
        // }
        
        // controlSignalRotation.y = actionBuffers.ContinuousActions[2];

        //set rotation
        // Quaternion rotation = Quaternion.Euler(0, controlSignalRotation.y, 0);
        // this.transform.rotation = rotation;
        
        
        m_Rigidbody.AddForce(transform.forward * forwardForce * forceMultiplier * Time.deltaTime);

        if (rotationAmount != 0)
        {
            Quaternion rotation = Quaternion.Euler(0, rotationAmount * Time.deltaTime * rotationMultiplier, 0); 
            this.transform.localRotation = rotation;
        }
        
        
        // calculate distance to the target
        // float distanceToTarget =
        //     Vector3.Distance( this.transform.localPosition,foodTransform.localPosition);
        //
        // check if the agent is close to the target

        // if (distanceToTarget < 1.5f)
        // {
        //     // give reward to the agent
        //     AddReward(1.0f);
        //     
        //     // end the current episode to reset
        //     EndEpisode();
        // }
        // else
        // {
        //     // deduct reward from the agent
        //     // SetReward(-0.2f); 
        // }
        //
        // check if the agent falls to the void
        
        
        if (this.transform.localPosition.y < -0f)
        {
            AddReward(-1f);
        
            // reset the current episode
            EndEpisode();
        }

        if (m_Rigidbody.velocity.magnitude < 1f)
        {
            AddReward(-0.1f);
        }
        //



    }

    public override void Heuristic(in ActionBuffers actionOut)
    {
        var discreteActionOut = actionOut.DiscreteActions;

        var forwardAction = 0;
        var rotationAction = 0;

        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
            print("forward");
        }
        if (Input.GetKey(KeyCode.S))
        {
            forwardAction = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rotationAction = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotationAction = 2;
        }

        discreteActionOut[0] = forwardAction;
        discreteActionOut[1] = rotationAction;
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.CompareTag("food"))
        {
            
            AddReward(1.0f);
            // EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            // hits a wall
            AddReward(-0.5f);
            // EndEpisode();
        } 
    }

    public void FoundFood()
    {
        AddReward(1f);
        // EndEpisode();
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
