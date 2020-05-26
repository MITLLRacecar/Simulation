using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PhysicsModule : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 prevVelocity;

    private void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.prevVelocity = this.rb.velocity;
    }

    private void FixedUpdate()
    {
        prevVelocity = this.rb.velocity;
    }

    #region Python Interface
    public Vector3 get_linear_acceleration()
    {
        return this.prevVelocity - this.rb.velocity;
    }

    public Vector3 get_angular_velocity()
    {
        return this.rb.angularVelocity;
    }
    #endregion
}
