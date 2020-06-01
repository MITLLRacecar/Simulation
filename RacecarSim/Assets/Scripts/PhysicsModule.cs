using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhysicsModule : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 prevVelocity;
    private Vector3 averageAcceleration = Vector3.zero;

    private const int accelerationSamples = 10;

    private void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.prevVelocity = this.rb.velocity;
    }

    private void FixedUpdate()
    {
        // TODO: project onto car's axis
        Vector3 curAcceleration = (this.rb.velocity - this.prevVelocity) / Time.deltaTime;
        this.averageAcceleration += (curAcceleration - this.averageAcceleration) / PhysicsModule.accelerationSamples;
        prevVelocity = this.rb.velocity;
    }

    #region Python Interface
    public Vector3 get_linear_acceleration()
    {
        return this.averageAcceleration;
    }

    public Vector3 get_angular_velocity()
    {
        return this.rb.angularVelocity;
    }
    #endregion
}
