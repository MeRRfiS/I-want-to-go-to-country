using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Funnel")]
public class Funnel : Instrument
{
    public int _maxUsings;
    private int _usings;

    public int Usings
    {
        get => _usings;
    }

    public override void Init()
    {
        if (_usings == 0)
        {
            _usings = _maxUsings;
        }
        if (_durability == 0)
        {
            _durability = _maxDurability;
        }
        _amount = 1;
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
                case TagConstants.SEEDBED:
                case TagConstants.TREE:
                    if (Usings == 0) return;

                    PlantController plant = hitObject.GetComponent<PlantController>();
                    if (!plant.IsPlantNeedWater()) return;

                    _durability--;
                    _usings--;
                    plant.Watering();
                    break;
                case TagConstants.WELL:
                    _usings = _maxUsings;
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
