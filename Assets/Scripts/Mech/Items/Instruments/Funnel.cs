using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : Instrument
{
    public Funnel(int level, int durability)
    {
        Level = level;
        Durability = durability;
    }

    protected override void UseInstrument()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if(Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.DISTANCE_FOR_PLANT)) 
        {
            GameObject hitObject = hit.collider.gameObject;
            if (!hitObject.CompareTag(TagConstants.PLANT)) return;

            hitObject.GetComponent<PlantController>().Watering();
        }
    }
}
