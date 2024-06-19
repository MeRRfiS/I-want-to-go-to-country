using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerConstants
{
    public const float MOVEMENT_SPEED = 5f;
    public const float JUMP_SPEED = 4f;

    public const float MINIMUM_VERT = -45.0f;
    public const float MAXIMUM_VERT = 80.0f;

    public const float DISTANCE_TO_OBJECT = 2.5f;
    public const float MAX_MASS_HELD_OBJECT = 10;
    public const float PICKUP_FORCE = 25f;

    public static float SENSITIVITY_VERT = 0.2f;
    public static float SENSITIVITY_HOR = 0.2f;
}
