using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : Instrument
{
    public int MaxUnings { get; set; }
    public int Usings { get; set; }

    public Funnel(int level, int durability)
    {
        InstrumentType = InstrumentTypeEnum.Funnel;
        Level = level;
        switch (level)
        {
            case 1:
                MaxDurability = 100;
                MaxUnings = 2;
                break;
            case 2:
                MaxDurability = 150;
                MaxUnings = 4;
                break;
            case 3:
                MaxDurability = 200;
                MaxUnings = 6;
                break;
            case 4:
                MaxDurability = 300;
                MaxUnings = 8;
                break;
            case 5:
                MaxDurability = 400;
                MaxUnings = 10;
                break;
        }
        Usings = MaxUnings;
        Durability = MaxDurability;

        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Funnel_Level_{level}");
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
                case TagConstants.TREE:
                    if (Usings == 0) return;

                    Durability--;
                    Usings--;
                    hitObject.GetComponent<PlantController>().Watering();
                    break;
                case TagConstants.WELL:
                    Usings = MaxUnings;
                    break;
            }
        }
    }
}
