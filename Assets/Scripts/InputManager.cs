using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void LeftJoystickMovement(Vector2 movement)
    {
        MMDebug.DebugOnScreen("left joystick", movement);

        playerController.deltaMovement = movement;
    }

    public void RightJoystickMovement(Vector2 movement)
    {
        MMDebug.DebugOnScreen("right joystick", movement);

        playerController.deltaRotation = movement;
    }
}