using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a zone in which cars are not allowed to surpass a certain maximum speed.
/// </summary>
public class SpeedLimit : MonoBehaviour
{
    #region Set in Unity Editor
    /// <summary>
    /// The maximum speed (in meters/second) which cars are allowed to travel within this speed limit zone.
    /// </summary>
    [SerializeField]
    private float maxSpeed = 0.5f;
    #endregion

    /// <summary>
    /// The message shown when a car breaks the speed limit.
    /// </summary>
    private string FailureMessage
    {
        get
        {
            return $"You traveled above {this.maxSpeed} m/s within the speed limit zone";
        }
    }            

    /// <summary>
    /// The cars currently within the speed limit zone.
    /// </summary>
    private readonly HashSet<Racecar> cars = new HashSet<Racecar>();

    private void Update()
    {
        foreach (Racecar car in this.cars)
        {
            if (car.Physics.LinearVelocity.magnitude > this.maxSpeed)
            {
                LevelManager.HandleFailure(car.Index, this.FailureMessage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Racecar car = other.GetComponentInParent<Racecar>();
        if (car != null)
        {
            cars.Add(car);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Racecar car = other.GetComponentInParent<Racecar>();
        if (car != null)
        {
            cars.Remove(car);
        }
    }
}
