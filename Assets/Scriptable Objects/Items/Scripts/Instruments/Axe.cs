using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Instrument Object", menuName = "Inventory System/Items/Axe")]
public class Axe : Instrument 
{
    private PlantController tree = null;

    public int _hitCount;
    public int _timeChop;

    public override void Init()
    {
        _durability = _maxDurability;
        _amount = 1;
    }

    public Axe(int level, int durability)
    {
        //_instrumentType = InstrumentTypeEnum.Axe;
        //_level = level;
        //_isCanSold = false;
        //switch (level)
        //{
        //    case 1:
        //        _maxDurability = 50;
        //        _hitCount = 10;
        //        _timeChop = 5;
        //        _price = 50;
        //        break;
        //    case 2:
        //        _maxDurability = 55;
        //        _hitCount = 8;
        //        _timeChop = 4;
        //        _price = 200;
        //        break;
        //    case 3:
        //        _maxDurability = 60;
        //        _hitCount = 6;
        //        _timeChop = 3;
        //        _price = 500;
        //        break;
        //    case 4:
        //        _maxDurability = 65;
        //        _hitCount = 4;
        //        _timeChop = 2;
        //        _price = 900;
        //        break;
        //    case 5:
        //        _maxDurability = 70;
        //        _hitCount = 2;
        //        _timeChop = 1;
        //        _price = 1400;
        //        break;
        //}
        _durability = _maxDurability;

        //_id = (int)Enum.Parse(typeof(ItemIdsEnum),
        //                     $"Axe_Level_{level}");
    }

    private void ChopTree()
    {
        tree.ChoppingTree(_hitCount);
        _durability--;
    }

    public override void UseItem()
    {
        if (tree == null) return;

        if (tree.IsCanChoppingTree())
        {
            UIController.GetInstance().ProgressBar(_timeChop, ChopTree);
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
