// Define operating system here (WINDOWS, MAC, or LINUX)
#define WINDOWS

using System;
using UnityEngine;

/// <summary>
/// Handles input from an Xbox controller.
/// </summary>
/// <remarks>
/// Xbox controller mapping varies based on operating system.
/// See https://wiki.unity3d.com/index.php/Xbox360Controller for details.
/// </remarks>
public class Controller : MonoBehaviour
{
    #region Constants
    /// <summary>
    /// The key on the keyboard corresponding to each Xbox controller button in Button.
    /// </summary>
    private static readonly KeyCode[] keyboardButtonMap =
    {
        KeyCode.Alpha1, // A
        KeyCode.Alpha2, // B
        KeyCode.Alpha3, // X
        KeyCode.Alpha4, // Y
        KeyCode.Z, // LB
        KeyCode.Slash, // RB
        KeyCode.Alpha5, // LJOY
        KeyCode.Alpha6, // RJOY
        KeyCode.Return, // START
        KeyCode.Backspace // BACK
    };

    /// <summary>
    /// The key on the keyboard corresponding to each Xbox trigger in Trigger.
    /// </summary>
    private static readonly KeyCode[] keyboardTriggerMap =
    {
        KeyCode.LeftShift,
        KeyCode.RightShift
    };

    /// <summary>
    /// The four keys on the keyboard corresponding to each Xbox joystick in Joystick, ordered (left, right, down, up).
    /// </summary>
    private static readonly KeyCode[][] keyboardJoystickMap =
    {
        new KeyCode[] { KeyCode.A, KeyCode.D, KeyCode.S, KeyCode.W },
        new KeyCode[] { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.UpArrow },
    };

#if WINDOWS
    /// <summary>
    /// The Unity button name corresponding to each Xbox button in Button.
    /// </summary>
    private static readonly string[] buttonMap =
    {
        "button 0", // A
        "button 1", // B
        "button 2", // X
        "button 3", // Y
        "button 4", // LB
        "button 5", // RB
        "button 8", // LJOY
        "button 9", // RJOY
        "button 7", // START
        "button 6" // BACK
    };

    /// <summary>
    /// The Unity axis name corresponding to each Xbox trigger in Trigger.
    /// </summary>
    private static readonly string[] triggerMap =
    {
        "9th axis",
        "10th axis"
    };

    /// <summary>
    /// The Unity axis names corresponding to the X and Y axes of each Xbox joystick in Joystick.
    /// </summary>
    private static readonly string[][] joystickMap =
    {
        new string[] { "X axis", "Y axis" },
        new string[] { "4th axis", "5th axis" }
    };
#elif MAC
    private static readonly string[] buttonMap =
    {
        "button 16", // A
        "button 17", // B
        "button 18", // X
        "button 19", // Y
        "button 13", // LB
        "button 14", // RB
        "button 11", // LJOY
        "button 12", // RJOY
        "button 9", // START
        "button 10" // BACK
    };

    private static readonly string[] triggerMap =
    {
        "5th axis",
        "6th axis"
    };

    private static readonly string[][] joystickMap =
    {
        new string[] { "X axis", "Y axis" },
        new string[] { "3rd axis", "4th axis" }
    };
#elif LINUX
    private static readonly string[] buttonMap =
    {
        "button 0", // A
        "button 1", // B
        "button 2", // X
        "button 3", // Y
        "button 4", // LB
        "button 5", // RB
        "button 9", // LJOY
        "button 10", // RJOY
        "button 7", // START
        "button 6" // BACK
    };

    private static readonly string[] triggerMap =
    {
        "9th axis",
        "10th axis"
    };

    private static readonly string[][] joystickMap =
    {
        new string[] { "X axis", "Y axis" },
        new string[] { "4th axis", "5th axis" }
    };
#endif
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
        RJOY,
        START,
        BACK
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
        int index = button.GetHashCode();
        if (button == Button.BACK)
        {
            return Input.GetButton(Controller.buttonMap[index]) || Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace);
        }
        return Input.GetButton(Controller.buttonMap[index]) || Input.GetKey(Controller.keyboardButtonMap[index]);
    }

    /// <summary>
    /// Returns true if the provided button was pressed this frame.
    /// </summary>
    /// <param name="button">A button on an Xbox controller.</param>
    /// <returns>True if the provided button was pressed this frame.</returns>
    public bool WasPressed(Button button)
    {
        int index = button.GetHashCode();
        if (button == Button.BACK)
        {
            return Input.GetButtonDown(Controller.buttonMap[index]) || Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace);
        }
        return Input.GetButtonDown(Controller.buttonMap[index]) || Input.GetKeyDown(Controller.keyboardButtonMap[index]);
    }

    /// <summary>
    /// Returns true if the provided button was released this frame.
    /// </summary>
    /// <param name="button">A button on an Xbox controller.</param>
    /// <returns>True if the provided button was released this frame.</returns>
    public bool WasReleased(Button button)
    {
        int index = button.GetHashCode();
        if (button == Button.BACK)
        {
            return Input.GetButtonUp(Controller.buttonMap[index]) || Input.GetKeyUp(KeyCode.Delete) || Input.GetKeyUp(KeyCode.Backspace);
        }
        return Input.GetButtonUp(Controller.buttonMap[index]) || Input.GetKeyUp(Controller.keyboardButtonMap[index]);
    }

    /// <summary>
    /// Returns the value of a trigger.
    /// </summary>
    /// <param name="trigger">A trigger on an Xbox controller.</param>
    /// <returns>The value of the provided trigger, ranging from 0 (unpressed) to 1 (fully pressed).</returns>
    public float GetTrigger(Trigger trigger)
    {
        int index = trigger.GetHashCode();
        float triggerValue = Input.GetAxis(Controller.triggerMap[index]);

        // If no input, check keyboard input
        if (triggerValue == 0)
        {
            triggerValue = Convert.ToInt32(Input.GetKey(Controller.keyboardTriggerMap[index]));
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
        int index = joystick.GetHashCode();
        float xAxis = Input.GetAxis(Controller.joystickMap[index][0]);
        float yAxis = Input.GetAxis(Controller.joystickMap[index][1]);

        // If no input, check alternative (keyboard) input
        if (xAxis == 0 && yAxis == 0)
        {
            xAxis = Convert.ToInt32(Input.GetKey(Controller.keyboardJoystickMap[index][1]))
                - Convert.ToInt32(Input.GetKey(Controller.keyboardJoystickMap[index][0]));
            yAxis = Convert.ToInt32(Input.GetKey(Controller.keyboardJoystickMap[index][3]))
                - Convert.ToInt32(Input.GetKey(Controller.keyboardJoystickMap[index][2]));
        }

        return new Vector2(xAxis, yAxis);
    }
#endregion
}
