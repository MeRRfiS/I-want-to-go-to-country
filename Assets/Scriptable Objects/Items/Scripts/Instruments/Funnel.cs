using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Funnel")]
public class Funnel : Instrument
{
    [field: SerializeField] public int MaxUsings;
    private int _usings;

    public int Usings
    {
        get => _usings;
    }

    public override void Init()
    {
        if (_usings == 0)
        {
            _usings = MaxUsings;
        }
        if (_durability == 0)
        {
            _durability = MaxDurability;
        }
        Amount = 1;
    }

    public override void GetItemInHand()
    {
        IsInHand = true;
        HandsAnimationManager.GetInstance().IsHoldInst(false);
        HandsAnimationManager.GetInstance().IsHoldFunnel(true);
        HandsAnimationManager.GetInstance().IsHoldStaf(false);
    }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if(Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM)) 
        {
            GameObject hitObject = hit.collider.gameObject;
            switch (hitObject.tag)
            {
                case TagConstants.PLANT:
                    if (Usings == 0) return;

                    PlantController plant = hitObject.GetComponent<PlantController>();
                    if (!plant.IsPlantNeedWater()) return;

                    _durability--;
                    _usings--;
                    plant.Watering();
                    RuntimeManager.PlayOneShot(FMODEvents.instance.Water);
                    break;
                case TagConstants.TREE:
                    if (Usings == 0) return;

                    TreesController tree = hitObject.GetComponent<TreesController>();
                    if (!tree.IsPlantNeedWater()) return;

                    _durability--;
                    _usings--;
                    tree.WateringTree();
                    RuntimeManager.PlayOneShot(FMODEvents.instance.Water);
                    break;
                case TagConstants.WELL:
                    _usings = MaxUsings;
                    break;
            }
        }
    }

    public override void Destruct()
    {
        _usings = 0;
        _durability = 0;
    }
}
