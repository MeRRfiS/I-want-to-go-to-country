using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    private float _rotationX;
    private float _rotationY;

    public void Move(InputAction.CallbackContext context)
    {
        PlayerController.GetInstance().ChangeMovement(context.ReadValue<Vector2>());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        PlayerController.GetInstance().ChangeVelocity();
    }

    public void RotationX(InputAction.CallbackContext context)
    {
        _rotationY += context.ReadValue<float>() * PlayerConstants.SENSITIVITY_HOR;
        PlayerController.GetInstance().ChangeRotation(rotationPlayer: _rotationY);
    }

    public void RotationY(InputAction.CallbackContext context)
    {
        _rotationX -= context.ReadValue<float>() * PlayerConstants.SENSITIVITY_VERT;
        _rotationX = Mathf.Clamp(_rotationX,
                                 PlayerConstants.MINIMUM_VERT,
                                 PlayerConstants.MAXIMUM_VERT);
        PlayerController.GetInstance().ChangeRotation(rotationCamera: _rotationX);
    }
}
