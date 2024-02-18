using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerService
{
    public bool IsCanRotation();
    public bool IsCanMoving();
    public bool IsCanUsingItem();
    public bool HoldingObject();
    public bool HoldingItem();
    public void ApplyGravity();
    public void ApplyMovement();
    public void ApplyRotation();
    public void ApplyMovementObject();
    public void ChangeMovement(Vector2 movement);
    public void ChangeVelocity();
    public void ChangeRotation(float rotationCamera = 0, float rotationPlayer = 0);
    public void PickupObject(GameObject heldObject);
    public void DropObject();
    public void UseItemInPlayerHand();
    public void SwitchActiveController(bool state);
}
