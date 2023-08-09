using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    private float _rotationX;
    private float _rotationY;
    public static bool holdingLMB = false;

    public void Move(InputAction.CallbackContext context)
    {
        if (!PlayerController.GetInstance().IsCanMoving) return;

        PlayerController.GetInstance().ChangeMovement(context.ReadValue<Vector2>());
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!PlayerController.GetInstance().IsCanMoving) return;
        if (!context.started) return;

        PlayerController.GetInstance().ChangeVelocity();
    }

    public void RotationX(InputAction.CallbackContext context)
    {
        if (!PlayerController.GetInstance().IsCanRotation) return;

        _rotationY += context.ReadValue<float>() * PlayerConstants.SENSITIVITY_HOR;
        PlayerController.GetInstance().ChangeRotation(rotationPlayer: _rotationY);
    }

    public void RotationY(InputAction.CallbackContext context)
    {
        if (!PlayerController.GetInstance().IsCanRotation) return;

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

        if(Physics.Raycast(startPoint.position, startPoint.forward, out hit, PlayerConstants.DISTANCE_TO_OBJECT))
        {
            switch (hit.collider.tag)
            {
                case TagConstants.HOLD:
                    if (!PlayerController.GetInstance().HoldingObject())
                    {
                        PlayerController.GetInstance().PickupObject(hit.collider.gameObject);
                    }
                    else
                    {
                        PlayerController.GetInstance().DropObject();
                    }
                    break;
                case TagConstants.INSTRUMENT:
                case TagConstants.SEED:
                case TagConstants.ITEM:
                    PlayerController.GetInstance().PickupItem(hit.collider.gameObject);
                    break;
            }
        }
    }

    public void DropObject(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        if (PlayerController.GetInstance().HoldingItem())
        {
            PlayerController.GetInstance().DropItem();
        }
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!PlayerController.GetInstance().HoldingItem()) return;

            PlayerController.GetInstance().UseItemInPlayerHand();
        }
        else if(context.performed)
        {
            holdingLMB = true;
        }
        else if (context.canceled)
        {
            holdingLMB = false;
        }
    }

    public void Interaction(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, PlayerConstants.DISTANCE_TO_OBJECT))
        {
            GameObject hitObject = hit.collider.gameObject;
            switch (hitObject.tag)
            {
                case TagConstants.PLANT:
                    hitObject.GetComponent<PlantController>().PatchHarvesting();
                    break;
                case TagConstants.TREE:
                    hitObject.GetComponent<PlantController>().TreeHarvesting();
                    break;
            }
        }
    }
}
