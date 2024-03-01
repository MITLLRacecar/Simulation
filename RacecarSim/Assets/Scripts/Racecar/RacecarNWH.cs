using NWH.Common.Vehicles;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NWH.NUI;
#endif

namespace NWH.WheelController3D
{
    /// <summary>
    /// Wrapper for the wheel, containing wheel-specific settings
    /// and the reference to WheelUAPI - an API that allows
    /// for use of WheelController or WheelColliderUAPI with the CarController
    /// (or VehicleController in NWH Vehicle Physics).
    /// </summary>
    [Serializable]
    public class _Wheel
    {
        public bool            power;
        public bool            steer;
        public bool handbrake;
        public WheelUAPI wheelUAPI;
    }

    /// <summary>
    /// Simple vehicle controller intended as a demo script to help showcase WC3D.
    /// If you need a complete vehicle physics package that uses WC3D check out NWH Vehicle Physics.
    /// Owners of WC3D get 30% off:
    /// https://assetstore.unity.com/packages/tools/physics/nwh-vehicle-physics-107332
    /// Since it inherits from the Vehicle class it can be used with the VehicleChanger
    /// and any of the other vehicle NWH assets (aircraft, boats) where the player can exit
    /// one type of the vehicle and enter any other type of vehicle, since all of these
    /// inherit from the same Vehicle class.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RacecarNWH : Vehicle
    {
        /// <summary>
        /// Maximum brake torque to be applied to the wheel.
        /// Also used for handbrake.
        /// </summary>
        public float maxBrakeTorque = 3000f;
        
        /// <summary>
        /// Maximum motor torque to be applied to the wheel.
        /// Since in this simple version of car controller powertrain is not
        /// simulated, too high motor torque can cause wheel spin up to
        /// very high angular velocities since there is no artificial cap.
        /// </summary>
        public float maxMotorTorque = 1000f;
        
        /// <summary>
        /// Maximum steering angle in degrees of the wheels that have steer set to true.
        /// This angle is used at ~0 velocity and interpolated towards the minSteerAngle
        /// as the speed increases.
        /// </summary>
        public float maxSteeringAngle = 35;
        
        /// <summary>
        /// Minimum steering angle. Steering angle is interpolated between max and min angles
        /// as the speed increases, with the minSteeringAngle achieved at about ~20m/s. This
        /// prevents jerky vehicle control and spinning out at higher speeds.
        /// Simplified implementation.
        /// </summary>
        public float minSteeringAngle = 20;
        
        /// <summary>
        /// List of wheel containers for this vehicle.
        /// </summary>
        public List<_Wheel> wheels;

        public float driveAxis;
        public float steerAxis;
        public bool handbrakeToggle;

        protected float smoothXAxis;
        protected float xAxis;
        protected float xAxisVelocity;
        protected float yAxis;

        private Rigidbody _rigidbody;
        
        
        public override void Awake()
        {
            base.Awake();

            // Init cached values
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null) gameObject.AddComponent<Rigidbody>();
        }

        
        /// <summary>
        /// Resets the wheel torque and steering values since
        /// FixedUpdate() is not run when behaviour is disabled.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            for (int i = 0; i < wheels.Count; i++)
            {
                WheelUAPI wc = wheels[i].wheelUAPI;
                wc.BrakeTorque = maxBrakeTorque;
                wc.MotorTorque = 0f;
                wc.SteerAngle = 0f;
            }
        }

        
        /// <summary>
        /// Since v11.13f not run when vehicle is disabled.
        /// Previously FixedUpdate() was called on vehicles that were asleep (now
        /// replaced with Behaviour.enabled).
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            xAxis = steerAxis;
            yAxis = driveAxis;
            smoothXAxis = Mathf.SmoothDamp(smoothXAxis, xAxis, ref xAxisVelocity, 0.12f);

            for (int i = 0; i < wheels.Count; i++)
            {
                _Wheel w = wheels[i];
                WheelUAPI wc = w.wheelUAPI;
                wc.BrakeTorque = 0f;
                wc.MotorTorque = 0f;

                if ( w.handbrake && handbrakeToggle)
                {
                    wc.BrakeTorque = maxBrakeTorque;
                }

                if (SpeedSigned < -0.4f && yAxis > 0.1f || SpeedSigned > 0.4f && yAxis < -0.1f)
                {
                    wc.BrakeTorque = maxBrakeTorque * Mathf.Abs(yAxis);
                }

                if (w.power && 
                    SpeedSigned >= -0.5f && yAxis > 0.1f || SpeedSigned <= 0.5f && yAxis < -0.1f)
                {
                    wc.MotorTorque = maxMotorTorque * yAxis;
                }


                if (w.steer)
                {
                    wc.SteerAngle = Mathf.Lerp(maxSteeringAngle, minSteeringAngle, Speed * 0.04f) * smoothXAxis;
                }
            }
        }
        
        
        /// <summary>
        /// Set defaults.
        /// </summary>
        private void Reset()
        {
            maxBrakeTorque = 3000f;
            maxMotorTorque = 1000f;
            maxSteeringAngle = 35f;
            minSteeringAngle = 20f;

            wheels = new List<_Wheel>();

            WheelUAPI[] wheelUAPIs = GetComponentsInChildren<WheelUAPI>();
            for (int i = 0; i < wheelUAPIs.Length; i++)
            {
                WheelUAPI wheelUAPI = wheelUAPIs[i];
                wheels.Add(new _Wheel()
                           {
                               wheelUAPI = wheelUAPI,
                               steer = i < 2,
                               power = true,
                               handbrake = i > 1
                           });
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    ///     Editor for WheelController.
    /// </summary>
    [CustomEditor(typeof(RacecarNWH))]
    [CanEditMultipleObjects]
    public class RacecarNWHEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI()) return false;

            drawer.Info(
                "This is a minimalistic car controller intended for demo purposes. The vehicle behavior will not be representative " +
                "of a real vehicle since powertrain is not simulated, there are no differentials, etc. For a complete vehicle solution using WheelController3D " +
                "check out NWH Vehicle Physics 2.");

            drawer.BeginSubsection("Torque");
            drawer.Field("maxMotorTorque");
            drawer.Field("maxBrakeTorque");
            drawer.EndSubsection();

            drawer.BeginSubsection("Steering");
            drawer.Field("maxSteeringAngle");
            drawer.Field("minSteeringAngle");
            drawer.EndSubsection();

            drawer.BeginSubsection("Wheels");
            drawer.ReorderableList("wheels");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }


    /// <summary>
    ///     Editor for WheelController.
    /// </summary>
    [CustomPropertyDrawer(typeof(_Wheel))]
    [CanEditMultipleObjects]
    public class _WheelPropertyDrawer : NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label)) return false;

            drawer.Field("wheelUAPI");
            drawer.Field("power");
            drawer.Field("steer");
            drawer.Field("handbrake");

            drawer.EndProperty();
            return true;
        }
    }
#endif
}