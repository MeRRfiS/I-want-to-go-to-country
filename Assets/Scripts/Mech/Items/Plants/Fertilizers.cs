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
        Usings = 10;
    }

    public override void UseItem()
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position, startPoint.forward, out hit, MechConstants.DISTANCE_FOR_PLANT))
        {
            Transform hitTransform = hit.collider.gameObject.transform;
            if (!hitTransform.CompareTag(TagConstants.PLANT)) return;

            PlantController plant = hitTransform.GetComponent<PlantController>();
            plant.Fertilizering(Level);
        }
    }
}
