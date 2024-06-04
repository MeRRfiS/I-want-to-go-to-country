using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    private float _rotationX;
    private float _rotationY;
    public static bool holdingLMB = false;

    public static Func<bool> BlockInputSystem;

    private bool BlockStatus()
    {
        if(BlockInputSystem == null) return false;

        return BlockInputSystem.Invoke();
    }

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
        if (BlockStatus()) return;
        if (!context.started) return;

        if (PlayerController.GetInstance().IsHoldingObject())
        {
            PlayerController.GetInstance().DropObject();
            return;
        }

        if (PlayerController.GetInstance().IsHoldingNPC())
        {
            PlayerController.GetInstance().DropNPC();
            return;
        }

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if(Physics.Raycast(startPoint.position, startPoint.forward, out hit, PlayerConstants.DISTANCE_TO_OBJECT))
        {
            switch (hit.collider.tag)
            {
                case TagConstants.HOLD:
                    PlayerController.GetInstance().PickupObject(hit.collider.gameObject);
                    break;
                case TagConstants.NPC:
                    PlayerController.GetInstance().PickupNPC(hit.collider.GetComponent<NPCController>());
                    break;
                case TagConstants.INSTRUMENT:
                case TagConstants.ITEM:
                    PlayerController.GetInstance().PickupItem(hit.collider.gameObject);
                    break;
            }
        }
    }

    public void DropObject(InputAction.CallbackContext context)
    {
        if (BlockStatus()) return;
        if (!context.started) return;

        if (PlayerController.GetInstance().IsHoldingItem())
        {
            PlayerController.GetInstance().DropItem();
        }
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (BlockStatus()) return;
        if (context.started)
        {
            if (!PlayerController.GetInstance().IsHoldingItem()) return;

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

    public void InteractionByLMB(InputAction.CallbackContext context)
    {
        if (BlockStatus()) return;
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
                    hitObject.GetComponent<TreesController>().TreeHarvesting();
                    break;
            }
        }
    }

    public void InteractionByEKey(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (BlockStatus()) return;

        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, PlayerConstants.DISTANCE_TO_OBJECT))
        {
            GameObject hitObject = hit.collider.gameObject;
            switch (hitObject.tag)
            {
                case TagConstants.SHOP:
                    if (!hitObject.GetComponent<ShopController>().enabled) return;

                    hitObject.GetComponent<ShopController>().LoadGoodsForSellingToUI();
                    hitObject.GetComponent<ShopController>().LoadGoodsForDayToUI();
                    UIController.GetInstance().SwitchActiveShopMenu();
                    break;
                case TagConstants.MAIL_BOX:
                    QuestSystemController questSystem;
                    if (!hitObject.GetComponent<QuestSystemController>())
                    {
                        questSystem = hitObject.transform.GetComponentInParent<QuestSystemController>();
                    }
                    else
                    {
                        questSystem = hitObject.GetComponent<QuestSystemController>();
                    }
                    questSystem.LoadDayQuestListToUI();
                    UIController.GetInstance().SwitchActiveQuestMenu();
                    break;
                case TagConstants.BUILDING:
                    hitObject.GetComponent<BuildController>().LoadCraftsCollectionToUI();
                    break;
                case TagConstants.CHEST:
                    hitObject.GetComponent<ChestController>().LoadChestInventoryToUI();
                    break;
            }
        }
    }
}
