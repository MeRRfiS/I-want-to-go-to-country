using System;
using UnityEngine;

public class Fertilizers: Item
{
    public int Level { get; set; }
    public int Usings { get; set; }

    public Fertilizers(int level) 
    {
        Level = level;
        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Fertilizers_{level}");
        Usings = MechConstants.MAX_USING_OF_FERTILIZER;
    }

    public override bool IsItemCountZero()
    {
        if (Usings == 0) return true;

        return false;
    }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.DISTANCE_FOR_PLANT))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PLANT)) return;

            Usings--;
            PlantController plant = hitTransform.GetComponent<PlantController>();
            plant.Fertilizering(Level);
        }
    }
}
