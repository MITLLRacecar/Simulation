using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public enum Button
    {
        A,
        B,
        X,
        Y,
        LB,
        RB,
        LJOY,
        RJOY
    }

    public enum Trigger
    {
        LEFT,
        RIGHT
    }

    public enum Joystick
    {
        LEFT,
        RIGHT
    }

    private Dictionary<Button, string> buttonMap = new Dictionary<Button, string>()
    {
        { Button.A, "A" },
        { Button.B, "B" },
        { Button.X, "X" },
        { Button.Y, "Y" },
        { Button.LB, "LB" },
        { Button.RB, "RB" },
        { Button.LJOY, "LClick" },
        { Button.RJOY, "RClick" }
    };

    private Dictionary<Trigger, string> triggerMap = new Dictionary<Trigger, string>()
    {
        { Trigger.LEFT, "LT" },
        { Trigger.RIGHT, "RT" }
    };

    private Dictionary<Joystick, Tuple<string, string>> joystickMap = new Dictionary<Joystick, Tuple<string, string>>()
    {
        { Joystick.LEFT, new Tuple<string, string>("LJoyX", "LJoyY") },
        { Joystick.RIGHT, new Tuple<string, string>("RJoyX", "RJoyY") },
    };

    private string altCode = "Alt";

    public bool is_down(Button button)
    {
        return Input.GetButton(buttonMap[button]);
    }

    public bool was_pressed(Button button)
    {
        return Input.GetButtonDown(buttonMap[button]);
    }

    public bool was_released(Button button)
    {
        return Input.GetButtonUp(buttonMap[button]);
    }

    public float get_trigger(Trigger trigger)
    {
        float triggerValue = Input.GetAxis(triggerMap[trigger]);

        // If no input, check alternative (keyboard) input
        if (triggerValue == 0)
        {
            triggerValue = Input.GetAxis(triggerMap[trigger] + this.altCode);
        }

        return triggerValue;
    }

    public Vector2 get_joystick(Joystick joystick)
    {
        float xAxis = Input.GetAxis(joystickMap[joystick].Item1);
        float yAxis = Input.GetAxis(joystickMap[joystick].Item2);

       // If no input, check alternative (keyboard) input
       if (xAxis == 0 && yAxis == 0)
       {
            xAxis = Input.GetAxis(joystickMap[joystick].Item1 + this.altCode);
            yAxis = Input.GetAxis(joystickMap[joystick].Item2 + this.altCode);
        }

        return new Vector2(xAxis, yAxis);
    }
}
