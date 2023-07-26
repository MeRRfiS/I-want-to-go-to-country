using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : Instrument
{
    public Funnel(int level, int durability)
    {
        Level = level;
        Durability = durability;

        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Funnel_Level_{level}");
    }

    public override void Use()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if(Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.DISTANCE_FOR_PLANT)) 
        {
            GameObject hitObject = hit.collider.gameObject;
            if (!hitObject.CompareTag(TagConstants.PLANT) && !hitObject.CompareTag(TagConstants.TREE)) return;

            hitObject.GetComponent<PlantController>().Watering();
        }
    }
}
