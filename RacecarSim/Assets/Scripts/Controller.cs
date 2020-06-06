using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles input from an Xbox controller.
/// </summary>
public class Controller : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// Maps Xbox buttons to the corresponding button name used in Unity.
    /// </summary>
    private static readonly Dictionary<Button, string> buttonMap = new Dictionary<Button, string>()
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

    /// <summary>
    /// Maps Xbox triggers to the corresponding axis name used in Unity.
    /// </summary>
    private static readonly Dictionary<Trigger, string> triggerMap = new Dictionary<Trigger, string>()
    {
        { Trigger.LEFT, "LT" },
        { Trigger.RIGHT, "RT" }
    };

    /// <summary>
    /// Maps Xbox joysticks to a the x and y axis names used in Unity
    /// </summary>
    private static readonly Dictionary<Joystick, Tuple<string, string>> joystickMap = new Dictionary<Joystick, Tuple<string, string>>()
    {
        { Joystick.LEFT, new Tuple<string, string>("LJoyX", "LJoyY") },
        { Joystick.RIGHT, new Tuple<string, string>("RJoyX", "RJoyY") },
    };

    /// <summary>
    /// Add this string to any trigger or joystick name to get the name of the alternative keyboard axis used in Unity
    /// </summary>
    private string altCode = "Alt";
    #endregion

    #region Public Interface
    /// <summary>
    /// The buttons on an Xbox controller.
    /// </summary>
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

    /// <summary>
    /// The triggers on an Xbox controller.
    /// </summary>
    public enum Trigger
    {
        LEFT,
        RIGHT
    }

    /// <summary>
    /// The joysticks on an Xbox controller.
    /// </summary>
    public enum Joystick
    {
        LEFT,
        RIGHT
    }

    /// <summary>
    /// Returns true if the provided button is pressed.
    /// </summary>
    /// <param name="button">A button on an Xbox controller.</param>
    /// <returns>True if the provided button is currently pressed.</returns>
    public bool IsDown(Button button)
    {
        return Input.GetButton(buttonMap[button]);
    }

    /// <summary>
    /// Returns true if the provided button was pressed this frame.
    /// </summary>
    /// <param name="button">A button on an Xbox controller.</param>
    /// <returns>True if the provided button was pressed this frame.</returns>
    public bool WasPressed(Button button)
    {
        return Input.GetButtonDown(buttonMap[button]);
    }

    /// <summary>
    /// Returns true if the provided button was released this frame.
    /// </summary>
    /// <param name="button">A button on an Xbox controller.</param>
    /// <returns>True if the provided button was released this frame.</returns>
    public bool was_released(Button button)
    {
        return Input.GetButtonUp(buttonMap[button]);
    }

    /// <summary>
    /// Returns the value of a trigger.
    /// </summary>
    /// <param name="trigger">A trigger on an Xbox controller.</param>
    /// <returns>The value of the provided trigger, ranging from 0 (unpressed) to 1 (fully pressed).</returns>
    public float GetTrigger(Trigger trigger)
    {
        float triggerValue = Input.GetAxis(triggerMap[trigger]);

        // If no input, check alternative (keyboard) input
        if (triggerValue == 0)
        {
            triggerValue = Input.GetAxis(triggerMap[trigger] + this.altCode);
        }

        return triggerValue;
    }

    /// <summary>
    /// Returns the coordinates of a joystick.
    /// </summary>
    /// <param name="joystick">A joystick on an Xbox controller.</param>
    /// <returns>The x and y coordinates of the provided joystick, ranging from (-1, -1) (bottom left) to (1, 1) (top right)</returns>
    public Vector2 GetJoystick(Joystick joystick)
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
    #endregion
}
