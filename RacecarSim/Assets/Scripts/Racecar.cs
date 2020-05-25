using System;
using UnityEngine;

public class Racecar : MonoBehaviour
{
    private Action userStart = null;
    private Action userUpdate = null;
    private Action userUpdateSlow = null;

    private double updateSlowTime = 1;
    private double updateSlowCounter = 0;

    public Drive Drive { get; private set; }

    private void Awake()
    {
        this.Drive = this.gameObject.GetComponent<Drive>();
    }

    private void Start()
    {
        this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -0.2f, 0);
        this.userStart?.Invoke();
    }

    private void Update()
    {
        this.userUpdate?.Invoke();

        this.updateSlowCounter -= Time.deltaTime;
        if (this.updateSlowCounter <= 0)
        {
            this.updateSlowCounter = this.updateSlowTime;
            this.userUpdateSlow?.Invoke();
        }
    }

    void set_start_update(Action start, Action update, Action update_slow = null)
    {
        this.userStart = start;
        this.userUpdate = update;
        this.userUpdateSlow = update_slow;
    }

    double get_delta_time()
    {
        return Time.deltaTime;
    }

    void set_update_slow_time(double time)
    {
        this.updateSlowTime = time;
    }
}
