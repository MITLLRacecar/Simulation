using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;
using UnityEngine.UI;
using System;

public class Hud : MonoBehaviour
{
    public Racecar Racecar;

    private enum Texts
    {
        TrueSpeed = 1,
        LinearAcceleration = 5,
        AngularVelocity = 8
    }

    private Text[] texts;

    private void Start()
    {
        texts = this.GetComponentsInChildren<Text>();
    }

    private void Update()
    {
        this.texts[Texts.TrueSpeed.GetHashCode()].text = this.Racecar.GetComponent<Rigidbody>().velocity.magnitude.ToString("F1");


        Vector3 linearAcceleration = this.Racecar.Physics.get_linear_acceleration();
        Vector3 angularVelocity = this.Racecar.Physics.get_angular_velocity();

        this.texts[Texts.LinearAcceleration.GetHashCode()].text = FormatVector3(this.Racecar.Physics.get_linear_acceleration());
        this.texts[Texts.AngularVelocity.GetHashCode()].text = FormatVector3(this.Racecar.Physics.get_angular_velocity());
    }

    private string FormatVector3(Vector3 vector)
    {
        return $"({FormatFloat(vector.x)},{FormatFloat(vector.y)},{FormatFloat(vector.z)})";       
    }

    private string FormatFloat(float value)
    {
        string str = value.ToString("F1");
        if (str[0] != '-')
        {
            return $" {str}";
        }

        return str;
    }
}
