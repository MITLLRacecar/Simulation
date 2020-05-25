using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public Racecar Racecar;

    // Start is called before the first frame update
    void Start()
    {
        Racecar.Drive.set_speed_angle(10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
