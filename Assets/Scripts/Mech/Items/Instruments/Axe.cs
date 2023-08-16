using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

public class Axe : Instrument 
{
    private PlantController tree = null;
    private UnityEvent myEvent;

    private int HitCount { get; set; }
    private int TimeChop { get; set; }

    public Axe(int level, int durability)
    {
        InstrumentType = InstrumentTypeEnum.Axe;
        Level = level;
        IsCanSold = false;
        switch (level)
        {
            case 1:
                MaxDurability = 50;
                HitCount = 10;
                TimeChop = 5;
                Price = 50;
                break;
            case 2:
                MaxDurability = 55;
                HitCount = 8;
                TimeChop = 4;
                Price = 200;
                break;
            case 3:
                MaxDurability = 60;
                HitCount = 6;
                TimeChop = 3;
                Price = 500;
                break;
            case 4:
                MaxDurability = 65;
                HitCount = 4;
                TimeChop = 2;
                Price = 900;
                break;
            case 5:
                MaxDurability = 70;
                HitCount = 2;
                TimeChop = 1;
                Price = 1400;
                break;
        }
        Durability = MaxDurability;

        Id = (int)Enum.Parse(typeof(ItemIdsEnum),
                             $"Axe_Level_{level}");
    }

    private void ChopTree()
    {
        tree.ChoppingTree(HitCount);
        Durability--;
    }

    public override void UseItem()
    {
        if (tree == null) return;

        if (tree.IsCanChoppingTree())
        {
            UIController.GetInstance().ProgressBar(TimeChop, ChopTree);
        }
    }

    public override GameObject Updating(GameObject obj, GameObject prefab)
    {
        Transform startPoint = Camera.main.transform;
        RaycastHit hit;

        if (Physics.Raycast(startPoint.position,
                           startPoint.forward,
                           out hit,
                           MechConstants.MAX_DISTANCE_FOR_USING_ITEM))
        {
            Transform hitObj = hit.transform;

            if (hitObj.CompareTag(TagConstants.TREE))
            {
                tree = hitObj.GetComponent<PlantController>();
            }
            else
            {
                tree = null;
                UIController.GetInstance().StopProgressBar();
            }
        }
        else
        {
            tree = null;
            UIController.GetInstance().StopProgressBar();
        }

        return base.Updating(obj, prefab);
    }
}
