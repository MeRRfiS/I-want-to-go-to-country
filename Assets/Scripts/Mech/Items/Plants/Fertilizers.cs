using System;
using UnityEngine;

public class Fertilizers: Item
{
    public int Level { get; set; }
    public int Usings { get; set; }

    public Fertilizers(int level) 
    {
        Level = level;
        Price = 250;
        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Fertilizers_{level}");
        Usings = MechConstants.MAX_USING_OF_FERTILIZER;
        IsCanSold = false;
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

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PLANT)) return;

            Usings--;
            PlantController plant = hitTransform.GetComponent<PlantController>();
            plant.Fertilizering(Level);
        }
    }
}
