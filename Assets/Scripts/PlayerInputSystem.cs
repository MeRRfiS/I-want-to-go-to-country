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

    public void GetObject(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;
        Physics.Raycast(startPoint.position, startPoint.forward, out hit, PlayerConstants.DISTANCE_TO_OBJECT);

        if (hit.collider != null && hit.collider.tag == TagConstants.HOLD)
        {
            if (!PlayerController.GetInstance().HoldingObject())
            {
                PlayerController.GetInstance().PickupObject(hit.collider.gameObject);
            }
            else
            {
                PlayerController.GetInstance().DropObject();
            }
        }
        else if(hit.collider != null && hit.collider.tag == TagConstants.INSTRUMENT)
        {
            if (!PlayerController.GetInstance().HoldingInstrument())
            {
                PlayerController.GetInstance().PickupInstrument(hit.collider.gameObject);
            }
        }
    }

    public void DropObject(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (PlayerController.GetInstance().HoldingInstrument())
        {
            PlayerController.GetInstance().DropInstrument();
        }
    }
}
